using MarketingScheduler.Common;
using MarketingScheduler.Models;
using MarketingScheduler.Services;
using Xunit;

namespace MarketingScheduler.Tests
{
    public class CalendarServiceTests
    {
        [Fact]
        public void GetDates_Returns_Correct_Number_Of_Dates()
        {
            // Arrange
            var calendarService = new CalendarService();
            var numberOfDays = 5;
            var expectedDates = new List<DateTime>
            {
                DateTime.Today.AddDays(1),
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(3),
                DateTime.Today.AddDays(4),
                DateTime.Today.AddDays(5)
            };

            // Act
            var actualDates = calendarService.GetDates(numberOfDays);

            // Assert
            Assert.Equal(expectedDates, actualDates);
        }

        [Fact]
        public void GenerateReport_Returns_Correct_Report()
        {
            // Arrange
            var calendarService = new CalendarService();
            var numberOfDays = 5;
            var customers = new List<Customer>
            {
                new Customer (Guid.NewGuid(), "Customer A", Frequency.Daily, null ),
                new Customer (Guid.NewGuid(), "Customer B", Frequency.Never, null)
            };
            var expectedReport = new List<ReportEntry>
            {
                new ReportEntry(DateTime.Today.AddDays(1).DayOfWeek.ToString(), DateTime.Today.AddDays(1).ToShortDateString(), new List<string> { "Customer A" }),
                new ReportEntry(DateTime.Today.AddDays(2).DayOfWeek.ToString(), DateTime.Today.AddDays(2).ToShortDateString(), new List<string> { "Customer A" }),
                new ReportEntry(DateTime.Today.AddDays(3).DayOfWeek.ToString(), DateTime.Today.AddDays(3).ToShortDateString(), new List<string> { "Customer A" }),
                new ReportEntry(DateTime.Today.AddDays(4).DayOfWeek.ToString(), DateTime.Today.AddDays(4).ToShortDateString(), new List<string> { "Customer A"}),
                new ReportEntry(DateTime.Today.AddDays(5).DayOfWeek.ToString(), DateTime.Today.AddDays(5).ToShortDateString(), new List<string> { "Customer A"})
            };

            // Act
            var actualReport = calendarService.GenerateReport(numberOfDays, customers);

            // Assert
            Assert.Equal(expectedReport.Count(), actualReport.Count());
            
            for (var i=0; i < expectedReport.Count(); i ++)
            {
                Assert.Equal(expectedReport[i].Day, actualReport[i].Day);
                Assert.Equal(expectedReport[i].Date, actualReport[i].Date);
                Assert.Equal(expectedReport[i].CustomerNames, actualReport[i].CustomerNames);
            }
        }
    }
}
