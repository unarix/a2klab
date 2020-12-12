using a2klab.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace a2klab.Services
{
    public interface ICosmosDBService
    {
        Task<bool> AddAsync(Promocion item);
        Task DeleteAsync(string id);
        Task<IEnumerable<Promocion>> GetAllAsync(string queryString);
        Task<Promocion> GetAsync(string id);
        Task<Promocion> UpdateAsync(Promocion item);
    }
}