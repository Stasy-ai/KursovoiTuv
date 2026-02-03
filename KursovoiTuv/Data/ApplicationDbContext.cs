// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using KursovoiTuv.Models;

namespace KursovoiTuv.Data
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<Order> Orders { get; set; }
		public DbSet<User> Users { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				// Простая строка подключения
				string connectionString = @"Server=(localdb)\mssqllocaldb;Database=ProductionOrdersDB;Trusted_Connection=True;TrustServerCertificate=True;";

				optionsBuilder.UseSqlServer(connectionString);
			}
		}

		// БЕЗ метода OnModelCreating - EF Core сам разберется
	}
}