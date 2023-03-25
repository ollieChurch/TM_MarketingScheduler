using MarketingScheduler.Common;
using MarketingScheduler.Models;

namespace MarketingScheduler.Services
{
    public class CalendarService 
    {
        public List<DateTime> GetDates(int numberOfDays)
        {
            var dates = new List<DateTime>();

            for (var i = 0; i < numberOfDays; i++)
            {
                dates.Add(DateTime.Today.AddDays(i + 1));
            }

            return dates;
        }

        public List<ReportEntry> GenerateReport(int numberOfDays, List<Customer> customers)
        {
            var dates = GetDates(numberOfDays);
            var report = new List<ReportEntry>();

            foreach (var date in dates)
            {
                var customersToInclude = customers
                    .Where(x => x.Frequency switch
                    {
                        Frequency.Daily => true,
                        Frequency.Weekly => x.FrequencyDetails != null && x.FrequencyDetails.Contains((int)date.DayOfWeek),
                        Frequency.Monthly => x.FrequencyDetails != null && x.FrequencyDetails.Contains(date.Day),
                        _ => false
                    })
                    .Select(x => x.Name)
                    .ToList();

                report.Add(new ReportEntry(
                    date.DayOfWeek.ToString(),
                    date.ToShortDateString(),
                    customersToInclude
                ));
            }

            return report;
        }
    }
}