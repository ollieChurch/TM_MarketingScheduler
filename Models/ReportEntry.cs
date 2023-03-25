namespace MarketingScheduler.Models
{
    public class ReportEntry
    {
        public string Day { get; set; }
        public string Date { get; set; }
        public List<string> CustomerNames { get; set; }

        public ReportEntry(string day, string date, List<string> customerNames)
        {
            this.Day = day;
            this.Date = date;
            this.CustomerNames = customerNames;
        }
    }
}