using System.Collections.Generic;
using System.Threading.Tasks;
using BackendSession2.Core.Models;

namespace BackendSession2.Core.Repositories
{
    public interface IAddressRepository
    {
        Task<List<ProvinceModel>> getProvinces();
        Task<ProvinceModel> createProvince(ProvinceModel addressItem);
        Task<ProvinceModel> getProvinceById(string id);
        Task<ProvinceModel> updateProvince(ProvinceModel addressItem);
        Task<int> deleteProvince(string id);
        string testAPI();
    }
}

