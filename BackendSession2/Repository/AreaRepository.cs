using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendSession2.Core.Models;
using BackendSession2.Core.Repositories;
using Microsoft.Extensions.Logging;
using Couchbase;
using Couchbase.N1QL;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.Core;
using StackExchange.Redis;
using BackendSession2.Service;

namespace BackendSession2.Repository
{
    public class AreaRepository : IAreaRepository
    {
        private readonly ILogger _logger;
        private readonly IBucket _bucket;
        private readonly string _bucketName = "AreaBucket";
        private readonly ICacheService _cacheService;
        private const string _province = "province";
        private const string _district = "district";
        private const string _areaKeyGetList = "areaKeyGetList";
        private const string _areaPrefix = "area";

        public AreaRepository(IBucketProvider bucketProvider, ILogger<AddressRepository> logger, ICacheService redis)
        {
            _bucket = bucketProvider.GetBucket(_bucketName);
            _logger = logger;
            _cacheService = redis;
        }

        public async Task<AreaModel> insertArea(AreaModel model)
        {
            var key = Guid.NewGuid();
            //model.Created = DateTime.UtcNow;
            model.Id = key;
            var document = new Document<AreaModel>
            {
                Content = model,
                Id = key.ToString()
            };
            var result = await _bucket.InsertAsync(document);
            await _cacheService.DeleteKeyWithByPrefix(_areaPrefix);
            return result.Success ? model : null;
        }

        public async Task<List<AreaModel>> getAreas(string type, string parentId)
        {
            if(string.IsNullOrEmpty(parentId))
            {
                var _keyRedisProvince = _areaKeyGetList + "_" + type;
                var _listRedisProvince = await _cacheService.GetCacheListAsync<AreaModel>(_keyRedisProvince);
                if(_listRedisProvince != null)
                {
                    return _listRedisProvince;
                } else
                {
                    List<AreaModel> result = await getListAll(type, parentId);
                    if(result != null)
                    {
                        bool value = await _cacheService.SetCacheListAsync<AreaModel>(_keyRedisProvince, result);
                        Console.WriteLine($"status save cache list province: " + value);
                    }
                    return result;
                }
            } else
            {
                var _keyRedisDistrictWard = _areaKeyGetList + "_" + type + "_" + parentId;
                var _listRedis = await _cacheService.GetCacheListAsync<AreaModel>(_keyRedisDistrictWard);
                if(_listRedis != null)
                {
                    return _listRedis;
                } else
                {
                    List<AreaModel> result = await getListAll(type, parentId);
                    if (result != null)
                    {
                        bool value = await _cacheService.SetCacheListAsync<AreaModel>(_keyRedisDistrictWard, result);
                        Console.WriteLine($"status save cache: " + value);
                    }
                    return result;
                }
            }
           
        }

        public async Task<List<AreaModel>> getListAll(string type, string parentId)
        {
            var statement = $"SELECT area. * FROM {_bucketName} AS area "
                           + $"WHERE type = $_type "
                           + $"AND parentId = $_parentId";
            Console.WriteLine("hung: " + statement);
            var query = new QueryRequest(statement)
                        .AddNamedParameter("$_type", type)
                        .AddNamedParameter("$_parentId", parentId)
                        .ScanConsistency(ScanConsistency.RequestPlus);
            var result = await _bucket.QueryAsync<AreaModel>(query);
            return result.Rows;
        }

        public async Task<AreaModel> updateArea(AreaModel area)
        {
            var addressModel = await this.getAreaById(area.Id);
            if (addressModel == null) return null;
            else area = area.MixBaseObject(addressModel);
            var document = new Document<AreaModel>
            {
                Content = area,
                Id = area.Id.ToString()
            };
            var result = await _bucket.UpsertAsync(document);
            await _cacheService.DeleteKeyWithByPrefix(_areaPrefix);
            return result.Success ? area : null;
        }

        public async Task<AreaModel> getAreaById(Guid id)
        {
            var statement = $"SELECT bucketname. * FROM {_bucketName} AS bucketname WHERE bucketname.id = $_id";
            var query = new QueryRequest(statement).AddNamedParameter("_id", id).ScanConsistency(ScanConsistency.RequestPlus);
            var result = await _bucket.QueryAsync<AreaModel>(query);
            return result.Rows.FirstOrDefault();
        }

        public async Task<int> deleteArea(Guid id, string type, string code)
        {
            var addressModel = await getAreaById(id);
            if (addressModel == null) return 0;
            if (type == _province || type == _district)
            {
                int deleteChild = await deleteById(code, type, id);
                if(deleteChild == 0) return 0;

                int deleteParent = await deleteById(code, "other", id);
                if(deleteParent == 0) return 0;
                return 1;
            } else
            {
                int value = await deleteById(code, "other", id);
                if (value == 0) return 0;
                return 1;
            }
        }

        public async Task<int> deleteById(string code, string type, Guid id)
        {
            var statement = "";
            if(type == _province)
            {
                statement = $"DELETE FROM {_bucketName} WHERE provinceId = $_code";
            } else if(type == _district)
            {
                statement = $"DELETE FROM {_bucketName} WHERE parentId = $_code";
            } else
            {
                statement = $"DELETE FROM {_bucketName} WHERE id = $_id";
            }

            var queryValue = new QueryRequest(statement);
            if (type == _province || type == _district)
            {
                 queryValue.AddNamedParameter("_code", code);
            } else
            {
                queryValue.AddNamedParameter("_id", id);
            }
            var result = await _bucket.QueryAsync<dynamic>(queryValue);
            await _cacheService.DeleteKeyWithByPrefix(_areaPrefix);
            return result.Success ? 1 : 0;
        }
    }
}