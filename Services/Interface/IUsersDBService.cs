using a2klab.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace a2klab.Services
{
    public interface IUsersDBService
    {
        Task<bool> AddAsync(UserDB item);
        Task DeleteAsync(string id);
        Task<IEnumerable<UserDB>> GetAllAsync(string queryString);
        Task<UserDB> GetAsync(string id);
        Task<UserDB> UpdateAsync(UserDB item);
    }
}