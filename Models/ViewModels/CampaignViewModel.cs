using AlgoBotBackend.Migrations.DAL;

namespace AlgoBotBackend.Models.ViewModels
{
    public class CampaignViewModel
    {
        public string Name { get; set; }
        public Firm Firm { get; set; }
        public ReferalSystem ReferalSystem { get; set; }
        public int? ProcentScore { get; set; }
        public int? Score { get; set; }
        public int CountUsers { get; set; }
        public int ScoreSumm { get; set; }
    }
}
