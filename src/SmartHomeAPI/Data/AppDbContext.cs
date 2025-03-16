#pragma warning disable CA1515

using Microsoft.EntityFrameworkCore;
using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Data;

/// <summary>
/// Контекст базы данных для SmartHomeAPI
/// </summary>
public sealed class AppDbContext (DbContextOptions<AppDbContext> options) : DbContext(options)
{
	/// <summary>
	/// Таблица измерений
	/// </summary>
	public DbSet<MeasureDomain> Measurements { get; set; } = null!;

	/// <summary>
	/// Таблица подписок на mqtt-топики
	/// </summary>
	public DbSet<SubscriptionDomain> Subscriptions { get; set; } = null!;

	protected override void OnModelCreating (ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		_ = modelBuilder?.Entity<MeasureDomain>()
			.HasKey(m => new { m.MeasurementId, m.Timestamp });

		_ = modelBuilder.Entity<MeasureDomain>()
			.Property(m => m.Value)
			.IsRequired()
			.HasMaxLength(255);

		_ = modelBuilder.Entity<SubscriptionDomain>()
			.HasKey(s => s.MeasurementId);

		_ = modelBuilder.Entity<SubscriptionDomain>()
			.Property(s => s.Description)
			.IsRequired()
			.HasMaxLength(500);

		_ = modelBuilder.Entity<SubscriptionDomain>()
			.Property(s => s.Unit)
			.HasMaxLength(50);

		_ = modelBuilder.Entity<SubscriptionDomain>()
			.Property(s => s.MqttTopic)
			.IsRequired()
			.HasMaxLength(255);

		_ = modelBuilder.Entity<SubscriptionDomain>()
			.Property(s => s.ConverterName)
			.HasDefaultValue("default")
			.HasMaxLength(100);
	}
}
