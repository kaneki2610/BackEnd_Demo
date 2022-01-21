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

namespace BackendSession2.Repository
{
    public class AreaRepository : IAreaRepository
    {
        private readonly ILogger _logger;
        private readonly IBucket _bucket;
        private readonly string _bucketName = "AreaBucket";

        public AreaRepository(IBucketProvider bucketProvider, ILogger<AddressRepository> logger)
        {
            _bucket = bucketProvider.GetBucket(_bucketName);
            _logger = logger;
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
            return result.Success ? model : null;
        }

        public async Task<List<AreaModel>> getAreas(string type, string parentId)
        {
            var statement = $"SELECT area. * FROM {_bucketName} AS area "
                            + $"WHERE type = $_type "
                            + $"AND parentId = $_parentId";
            Console.WriteLine("hung: " + statement);
            var query = new QueryRequest(statement)
                        .AddNamedParameter("$_type", type)
                        .AddNamedParameter("$_parentId", parentId);
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
            return result.Success ? area : null;
        }

        public async Task<AreaModel> getAreaById(Guid id)
        {
            var statement = $"SELECT bucketname. * FROM {_bucketName} AS bucketname WHERE bucketname.id = $_id";
            var query = new QueryRequest(statement).AddNamedParameter("_id", id);
            var result = await _bucket.QueryAsync<AreaModel>(query);
            return result.Rows.FirstOrDefault();
        }

        public async Task<int> deleteArea(Guid id, string type, string code)
        {
            var addressModel = await getAreaById(id);
            if (addressModel == null) return 0;
            if (type == "province" || type == "district")
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
            if(type == "province")
            {
                statement = $"DELETE FROM {_bucketName} WHERE provinceId = $_code";
            } else if(type == "district")
            {
                statement = $"DELETE FROM {_bucketName} WHERE parentId = $_code";
            } else
            {
                statement = $"DELETE FROM {_bucketName} WHERE id = $_id";
            }

            var queryValue = new QueryRequest(statement);
            if (type == "province" || type == "district")
            {
                 queryValue.AddNamedParameter("_code", code);
            } else
            {
                queryValue.AddNamedParameter("_id", id);
            }
            var result = await _bucket.QueryAsync<dynamic>(queryValue);
            return result.Success ? 1 : 0;
        }
    }
}