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
    public class AddressRepository : IAddressRepository
    {
        private readonly ILogger _logger;
        private readonly IBucket _bucket;
        private readonly string _bucketName = "BackEndSession1Bucket";

        public AddressRepository(IBucketProvider bucketProvider, ILogger<AddressRepository> logger)
        {
            _bucket = bucketProvider.GetBucket(_bucketName);
            _logger = logger;
        }

        public string testAPI()
        {
            return "HUNGTM Handsome 261";
        }

        public async Task<ProvinceModel> createProvince(ProvinceModel model)
        {
            var key = Guid.NewGuid().ToString();
            model.Created = DateTime.UtcNow;
            model.Id = key;
            var document = new Document<ProvinceModel>
            {
                Content = model,
                Id = key
            };
            var result = await _bucket.InsertAsync(document);
            return result.Success ? model : null;
        }

        public async Task<List<ProvinceModel>> getProvinces()
        {
            var statement = $"SELECT bucketname. * FROM {_bucketName} AS bucketname";
            var query = new QueryRequest(statement);

            var result = await _bucket.QueryAsync<ProvinceModel>(query);

            return result.Rows;
        }

        public async Task<ProvinceModel> updateProvince(ProvinceModel addressItem)
        {
            var addressModel = await this.getProvinceById(addressItem.Id);
            if (addressModel == null) return null;
            else addressItem = addressItem.MixBaseObject(addressModel);
            var document = new Document<ProvinceModel>
            {
                Content = addressItem,
                Id = addressItem.Id
            };
            var result = await _bucket.UpsertAsync(document);
            return result.Success ? addressItem : null;
        }

        public async Task<ProvinceModel> getProvinceById(string id)
        {
            var statement = $"SELECT bucketname. * FROM {_bucketName} AS bucketname WHERE bucketname.id = $_id";
            var query = new QueryRequest(statement).AddNamedParameter("_id", id);
            var result = await _bucket.QueryAsync<ProvinceModel>(query);
            return result.Rows.FirstOrDefault();
        }

        public async Task<int> deleteProvince(string id)
        {
            var addressModel = await this.getProvinceById(id);
            if (addressModel == null) return 0;
            var statement = $"DELETE * FROM {_bucketName} WHERE id = $_id";
            var query = new QueryRequest(statement).AddNamedParameter("_id", id);
            var result = await _bucket.QueryAsync<dynamic>(query);
            return result.Success ? 1 : 0;
        }
    }
}