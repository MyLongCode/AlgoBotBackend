using AlgoBotBackend.Migrations.DAL;

namespace AlgoBotBackend.Models.CSV
{
    public class StudentPaymentsCSV
    {
        public int Amount { get; set; }
        public string PhoneNumber { get;set; }
        public AdvertisingСampaign Campaign { get; set; }
    }
}
