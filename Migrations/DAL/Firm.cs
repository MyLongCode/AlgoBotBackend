namespace AlgoBotBackend.Migrations.DAL
{
	public class Firm
	{
		public int Id { get; set; }
		public int OwnerId { get; set; }
		public User Owner { get; set; }
		public string Name { get; set; }
		public ReferalSystem DefaultReferalSystem { get; set; }
	}
}
