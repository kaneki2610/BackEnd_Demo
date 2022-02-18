using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendSession2.Service
{
    public interface ICacheService
    {
        Task<string> GetCacheValueAsync(string key);

         Task SetCacheValueAsync(string key, string value);

         Task<bool> SetCacheListAsync<T>(string key, List<T> value);

         Task<List<T>> GetCacheListAsync<T>(string key);

         Task<bool> DeleteKeyWithByPrefix(string prefix);

    }
}
