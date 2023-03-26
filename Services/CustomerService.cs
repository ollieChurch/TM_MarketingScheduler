using MarketingScheduler.Common;
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

        public async Task<Customer> UpdateCustomerPreferenceAsync(Guid id, Frequency frequency, List<int>? frequencyDetails)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
            var update = Builders<Customer>.Update
                .Set(c => c.Frequency, frequency)
                .Set(c => c.FrequencyDetails, frequencyDetails);

            var updateResponse = await _customers.UpdateOneAsync(filter, update);

            if (updateResponse.ModifiedCount == 1)
            {
                var updatedCustomer = await _customers.Find(x => x.Id == id).FirstOrDefaultAsync();
                return updatedCustomer != null ? updatedCustomer : throw new Exception("updated customer could not be found");
            }
            else
            {
                throw new Exception("customer preference update failed");
            }
        }

        public string ValidateFrequencyInput(Frequency frequency, List<int>? frequencyDetails)
        {
            if (frequency == Frequency.Weekly || frequency == Frequency.Monthly)
            {
                if (frequencyDetails == null || !frequencyDetails.Any())
                {
                    return "Frequency details are required for weekly or monthly frequency";
                }

                if (frequency == Frequency.Weekly && frequencyDetails.Exists(x => x < 0 || x > 6))
                {
                    return "For weekly frequency, frequencyDetails must contain integers from 0 (Sunday) to 6 (Saturday)";
                }

                if (frequency == Frequency.Monthly && frequencyDetails.Exists(x => x < 1 || x > 28))
                {
                    return "For monthly frequency, frequencyDetails must contain integers from 1 (1st) to 28 (28th)";
                }
            }

            return "validated";
        }

        public async Task<string> DeleteCustomerAsync(Guid id)
        {
            var deleteResult = await _customers.DeleteOneAsync(x => x.Id == id);
            if (deleteResult.DeletedCount == 1)
            {
                return $"Customer {id} was deleted";
            }
            else 
            {
                throw new Exception("Customer delete failed");
            }
        }

        // This end point was added for quality of life during testing. It would be removed before production.
        public async Task<string> EmptyDatabaseAsync()
        {
            await _customers.DeleteManyAsync(x => true);
            return "Database emptied";
        }
    }
}