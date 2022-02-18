using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendSession2.Service
{
    public class MemoryClassService : ICacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly int _port = 6379;
        private readonly string _host = "localhost";
        public MemoryClassService(IConnectionMultiplexer i)
        {
            _connectionMultiplexer = i;
        }

        public async Task<List<T>> GetCacheListAsync<T>(string key)
        {
            if(string.IsNullOrEmpty(key))
            {
                return null;
            }
            var db = _connectionMultiplexer.GetDatabase();
            string value = db.StringGet(key);
            if (value == null) return null;
            List<T> list = JsonConvert.DeserializeObject<List<T>>(value);
            return list;
        }
        public async Task<bool> SetCacheListAsync<T>(string key, List<T> value)
        {
            bool result = true;
            if(string.IsNullOrEmpty(key) || value == null)
            {
                result = false;
            } else
            {
                string json = JsonConvert.SerializeObject(value);
                var db = _connectionMultiplexer.GetDatabase();
                bool status = db.StringSet(key, json);
                if(status)
                {
                    await db.KeyExpireAsync(key, new System.TimeSpan(0,1,0,0));
                }
            }
            return result;
        }

        public async Task<bool> DeleteKeyWithByPrefix(string prefix)
        {
            bool result = true;
            if(string.IsNullOrEmpty(prefix))
            {
                result = false;
            } else
            {
                var db = _connectionMultiplexer.GetDatabase();
                IServer iServer = _connectionMultiplexer.GetServer("localhost:6379");
                var keys = iServer.Keys(pattern: $"*{prefix}*");
                foreach(var key in keys)
                {
                    
                    await db.KeyDeleteAsync(key);
                }
            }
            return result;
        }

        public async Task<string> GetCacheValueAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public async Task SetCacheValueAsync(string key, string value)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.StringSetAsync(key, value);
        }
    }
}
