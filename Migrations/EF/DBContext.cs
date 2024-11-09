using Microsoft.EntityFrameworkCore;
using AlgoBotBackend.Migrations.DAL;
using System.ComponentModel;
using System.Data;
using System.Numerics;
using System.Collections.Generic;

namespace AlgoBotBackend.Migrations.EF
{
    public class DBContext : DbContext
    {

        public virtual DbSet<BotUser> BotUsers { get; set; } = null!;
		public virtual DbSet<User> Users { get; set; } = null!;
		public virtual DbSet<Firm> Firms { get; set; } = null!;
		public virtual DbSet<AdvertisingСampaign> AdvertisingСampaigns { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            Database.EnsureCreated();
		}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.EnableSensitiveDataLogging();
        }

    }
}