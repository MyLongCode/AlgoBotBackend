using AlgoBotBackend.Migrations.DAL;

namespace AlgoBotBackend.Models
{
    public class CreateFirmModel
    {
        public string Name { get; set; }
        public ReferalSystem DefaultReferalSystem { get; set; }
    }
}
