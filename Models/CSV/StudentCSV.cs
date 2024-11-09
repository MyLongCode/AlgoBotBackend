using CsvHelper.Configuration.Attributes;

namespace AlgoBotBackend.Models.CSV
{
    public class StudentCSV
    {
        [Index(0)]
        public int StudentId { get; set; }
        [Index(7)]
        public string Phonenumber { get; set; }
    }
}
