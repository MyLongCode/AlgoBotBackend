namespace AlgoBotBackend.Migrations.DAL
{
	public class User
	{
		public int Id { get; set; }
		public string Role { get; set; }
		public string Login { get; set; }
        public string? ReferalUsername { get; set; }
        public string Password { get; set; }
		public string FullName { get; set; }
		public string? PhoneNumber { get; set; }
        public string? ChildAge { get; set; } = null!;
        public string? ChildName { get; set; } = null!;
        public int? StageReg { get; set; }
        public int Score { get; set; } = 0;
        public List<Payment> Payments { get; set; } = new List<Payment>();
    }
}
