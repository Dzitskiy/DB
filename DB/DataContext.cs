using DB1.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB1
{
	public class DataContext: DbContext
	{
		public DbSet<Client> clients { get; set; }
		public DbSet<Deposit> deposits { get; set; }

		public DataContext()
        {
            Database.EnsureCreated();
        }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=password;Database=otus");
			
			base.OnConfiguring(optionsBuilder);
		}
	}
}
