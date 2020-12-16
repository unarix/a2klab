using a2klab.BusinessLogic;
using a2klab.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace a2klab.Services
{
    public class UsersDBService: IUsersDBService
    {
        private static Container _container;

        public UsersDBService(CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<bool> AddAsync(UserDB item)
        {
            try
            {
                var req = await _container.CreateItemAsync<UserDB>(item);

                if(req.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }

                return false;

            }
            catch (CosmosException ex)
            {

                return false;
            }            
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<UserDB>(id, new PartitionKey(id));
        }

        public async Task<UserDB> GetAsync(string id)
        {
            try
            {
                ItemResponse<UserDB> response = await _container.ReadItemAsync<UserDB>(id, new PartitionKey(id));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {

                throw;
            }
        }

        public async Task<IEnumerable<UserDB>> GetAllAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<UserDB>(new QueryDefinition(queryString));

            List<UserDB> results = new List<UserDB>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<UserDB> UpdateAsync(UserDB entity)
        {
            try
            {
                var item = await GetAsync(entity.Id);
                if(item != null)
                {
                    return await _container.UpsertItemAsync<UserDB>(entity, new PartitionKey(entity.Id));
                }

                return null;
            }
            catch (CosmosException ex)
            {

                throw;
            }             
        }
    }
}