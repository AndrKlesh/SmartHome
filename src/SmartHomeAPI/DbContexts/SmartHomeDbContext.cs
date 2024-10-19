using Microsoft.EntityFrameworkCore;
using SmartHomeAPI.Entities;

namespace SmartHomeAPI.DbContexts;

public class SmartHomeDbContext : DbContext
{
	public DbSet<TopicDomain> Topics { get; set; } = null!;
	public DbSet<MeasureDomain> Measurements { get; set; } = null!;

	protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
	{
		_ = optionsBuilder.UseNpgsql("Host=localhost;Database=SmartHomeDb;Username=postgres;Password=postgres");
	}
}
