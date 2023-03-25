using MarketingScheduler.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MarketingScheduler.Services
{
    public class CustomerService
    {
        private readonly IMongoCollection<Customer> _customers;

        public CustomerService(IOptions<DatabaseSettings> databaseSettings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _customers = database.GetCollection<Customer>(databaseSettings.Value.CollectionName);
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var customers = await _customers.Find(x => true).ToListAsync();
            return customers;
        }

        public async Task<List<Customer>> AddCustomersAsync(List<Customer> newCustomers)
        {
            foreach (Customer customer in newCustomers)
            {
                customer.Id = Guid.NewGuid();
            }

            await _customers.InsertManyAsync(newCustomers);
            var updatedCustomers = await GetAllCustomersAsync();

            return updatedCustomers;
        }
    }
}