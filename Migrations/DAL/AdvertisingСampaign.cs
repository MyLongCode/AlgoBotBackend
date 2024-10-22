namespace AlgoBotBackend.Migrations.DAL
{
	public class AdvertisingСampaign
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int FirmId { get; set; }
		public Firm Firm { get; set; }
		public ReferalSystem ReferalSystem { get; set; }
		public int? ProcentScore { get; set; }
		public int? Score { get; set; }
	}
}
