using a2klab.BusinessLogic;
using a2klab.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace a2klab.Services
{
    public class CosmosDBService: ICosmosDBService
    {
        private static Container _container;

        public CosmosDBService(CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<bool> AddAsync(Promocion item)
        {
            try
            {
                var req = await _container.CreateItemAsync<Promocion>(item);

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
            await _container.DeleteItemAsync<Promocion>(id, new PartitionKey(id));
        }

        public async Task<Promocion> GetAsync(string id)
        {
            try
            {
                ItemResponse<Promocion> response = await _container.ReadItemAsync<Promocion>(id, new PartitionKey(id));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Promocion>> GetAllAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<Promocion>(new QueryDefinition(queryString));

            List<Promocion> results = new List<Promocion>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Promocion> UpdateAsync(Promocion entity)
        {
            try
            {
                var item = await GetAsync(entity.Id);
                if(item != null)
                {
                    return await _container.UpsertItemAsync<Promocion>(entity, new PartitionKey(entity.Id));
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