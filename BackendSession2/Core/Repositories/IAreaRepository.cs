using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackendSession2.Core.Models;

namespace BackendSession2.Core.Repositories
{
    public interface IAreaRepository
    {
        Task<List<AreaModel>> getAreas(string type, string parentId);
        Task<AreaModel> insertArea(AreaModel area);
        Task<AreaModel> getAreaById(Guid id);
        Task<AreaModel> updateArea(AreaModel area);
        Task<int> deleteArea(Guid id, string type, string code);
        Task<int> deleteById(string code, string type, Guid id);
    }
}