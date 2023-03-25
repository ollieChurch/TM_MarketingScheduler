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
        public async Task<ActionResult<List<ReportEntry>>> GetReport([FromQuery] int numberOfDays)
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