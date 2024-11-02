﻿namespace AlgoBotBackend.Migrations.DAL
{
	public class User
	{
		public int Id { get; set; }
		public string Role { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public string FullName { get; set; }
		public int? IdInAlgo { get; set; }
	}
}
