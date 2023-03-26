using System.ComponentModel.DataAnnotations;
using MarketingScheduler.Common;
using MarketingScheduler.Models;
using MarketingScheduler.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketingScheduler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class  MarketingSchedulerController : ControllerBase
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

        [HttpGet("allCustomers")]
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

            foreach (Customer customer in newCustomers)
            {
                if (customer.Frequency == Frequency.Weekly || customer.Frequency == Frequency.Monthly)
                {
                    if (customer.FrequencyDetails == null)
                    {
                        return BadRequest("Frequency details are required for weekly or monthly frequency");
                    }

                    if (customer.Frequency == Frequency.Weekly && customer.FrequencyDetails.Exists(x => x < 0 || x > 6))
                    {
                        return BadRequest("For weekly frequency, frequencyDetails must contain integers from 0 (Sunday) to 6 (Saturday)");
                    }

                    if (customer.Frequency == Frequency.Monthly && customer.FrequencyDetails.Exists(x => x < 1 || x > 28))
                    {
                        return BadRequest("For monthly frequency, frequencyDetails must contain integers from 1 (1st) to 28 (28th)");
                    }
                }
            }

            try
            {
                var customers = await _customerService.AddCustomersAsync(newCustomers);
                var report = _calendarService.GenerateReport(90, customers);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while adding customers");
                return StatusCode(500, "An error occured while adding all customers");
            }
        }

        [HttpGet("report")]
        public async Task<ActionResult<List<ReportEntry>>> GetReport([FromQuery][Range(1, int.MaxValue)] int numberOfDays)
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
    }
}