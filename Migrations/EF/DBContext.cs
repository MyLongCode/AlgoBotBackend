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

        public virtual DbSet<BotUser> Users { get; set; } = null!;

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.EnableSensitiveDataLogging();
        }

    }
}