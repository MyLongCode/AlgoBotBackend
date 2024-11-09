using CsvHelper.Configuration.Attributes;

namespace AlgoBotBackend.Models.CSV
{
    public class PaymentCSV
    {
        [Index(5)]
        public float Amount { get; set; } = 0;
        [Index(6)]
        public int StudentId { get; set; }
        [Index(22)]
        public string CourseName { get; set; }

    }
}
