using System.ComponentModel.DataAnnotations;
using MarketingScheduler.Common;
using MarketingScheduler.Models;
using MarketingScheduler.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketingScheduler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MarketingSchedulerController : ControllerBase
    {
        private readonly ILogger<MarketingSchedulerController> _logger;
        private readonly CustomerService _customerService;
        private readonly CalendarService _calendarService;

        public MarketingSchedulerController(ILogger<MarketingSchedulerController> logger, CustomerService customerService, CalendarService calendarService)
        {
            _logger = logger;
            _customerService = customerService;
            _calendarService = calendarService;
        }

        [HttpGet("customers")]
        public async Task<ActionResult<List<Customer>>> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while getting all customers");
                return StatusCode(500, "An error occured while getting all customers");
            }
        }

        [HttpPost("addCustomers")]
        public async Task<ActionResult<List<ReportEntry>>> AddCustomers([FromBody] List<Customer> newCustomers)
        {
            if (newCustomers == null || !newCustomers.Any())
            {
                return BadRequest("At least one customer must be provided.");
            }

            foreach (Customer customer in newCustomers.Where(x => x.Frequency == Frequency.Weekly || x.Frequency == Frequency.Monthly))
            {
                var userInputValidation = _customerService.ValidateFrequencyInput(customer.Frequency, customer.FrequencyDetails);
                if (userInputValidation != "validated")
                {
                    return BadRequest(userInputValidation);
                }
            }

            try
            {
                var iscustomerUpdated = await _customerService.AddCustomersAsync(newCustomers);
                var customers = await _customerService.GetAllCustomersAsync();
                var report = _calendarService.GenerateReport(90, customers);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while adding customers");
                return StatusCode(500, "An error occured while adding all customers");
            }
        }

        [HttpGet("{numberOfDays}/report")]
        public async Task<ActionResult<List<ReportEntry>>> GetReport([FromRoute][Range(1, int.MaxValue)] int numberOfDays)
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                var report = _calendarService.GenerateReport(numberOfDays, customers);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while generating report");
                return StatusCode(500, "An error occured while generating report");
            }
        }

        [HttpPatch("{customerId}/updatePreference")]
        public async Task<ActionResult<Customer>> UpdateCustomerPreference([FromRoute] Guid customerId, [FromQuery][Required] Frequency frequency, [FromBody] List<int>? frequencyDetails)
        {
            var userInputValidation = _customerService.ValidateFrequencyInput(frequency, frequencyDetails);
            if (userInputValidation != "validated")
            {
                return BadRequest(userInputValidation);
            }

            try
            {
                var updatedCustomer = await _customerService.UpdateCustomerPreferenceAsync(customerId, frequency, frequencyDetails);
                return Ok(updatedCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occured while updating preferences for customer {customerId}");
                return StatusCode(500, $"An error occured while updating preferences for customer {customerId}");
            }
        }

        [HttpDelete("{customerId}/delete")]
        public async Task<ActionResult<string>> DeleteCustomer([FromRoute] Guid customerId)
        {
            try
            {
                var deletedResponse = await _customerService.DeleteCustomerAsync(customerId);
                return Ok(deletedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting customer {customerId}");
                return StatusCode(500, $"An error occured while deleting customer {customerId}");
            }
        }

        // This end point was added for quality of life during testing. It would be removed before production.
        [HttpDelete("deleteAll")]
        public async Task<ActionResult<string>> DeleteAllCustomers()
        {
            try
            {
                var deletedResponse = await _customerService.EmptyDatabaseAsync();
                return Ok(deletedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting all customers from the database");
                return StatusCode(500, "An error occured while deleting all customers");
            }
        }
    }
}