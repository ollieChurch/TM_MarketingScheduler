using MarketingScheduler.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MarketingScheduler.Models
{
    public class Customer
    {
        [BsonId]
        [BsonRepresentation(BsonType.Binary)]
        [BsonElement("_id")]
        public Guid Id { get; set; }

        [BsonElement("_name")]
        public string Name { get; set; }

        [BsonElement("_frequency")]
        public Frequency Frequency { get; set; }

        [BsonIgnore]
        public string? FrequencyName => Enum.GetName(typeof(Frequency), Frequency);

        [BsonElement("_frequencyDetails")]
        public List<int>? FrequencyDetails { get; set; }

        public bool ShouldSendMarketing(DateTime date)
        {
            switch (Frequency)
            {
                case Frequency.Daily:
                    return true;
                case Frequency.Weekly:
					return FrequencyDetails != null && FrequencyDetails.Contains((int)date.DayOfWeek);
                case Frequency.Monthly:
					return FrequencyDetails != null && FrequencyDetails.Contains(date.Day);
                default:
					return false;
            }
        }

        public Customer(Guid id, string name, Frequency frequency, List<int>? frequencyDetails)
        {
            this.Id = id;
            this.Name = name;
            this.Frequency = frequency;
            this.FrequencyDetails = frequencyDetails;
        }
    }
}