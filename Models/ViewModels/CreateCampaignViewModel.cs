using AlgoBotBackend.Migrations.DAL;

namespace AlgoBotBackend.Models.ViewModels
{
    public class CreateCampaignViewModel
    {
        public string Name { get; set; }
        public int FirmId { get; set; }
        public ReferalSystem ReferalSystem { get; set; }
        public string? Distribution { get; set; }
        public ScoreType ScoreType { get; set; }
        public int Summ { get; set; }
    }
}
