using Microsoft.EntityFrameworkCore;
using System.IO;

namespace BlendoBot.Module.UserTimeZone.Database;

internal class UserTimeZoneDbContext : DbContext {
	private UserTimeZoneDbContext(DbContextOptions<UserTimeZoneDbContext> options) : base(options) { }
	public DbSet<UserSetting> UserSettings { get; set; }

	public static UserTimeZoneDbContext Get(UserTimeZone module) {
		DbContextOptionsBuilder<UserTimeZoneDbContext> optionsBuilder = new();
		optionsBuilder.UseSqlite($"Data Source={Path.Combine(module.FilePathProvider.GetDataDirectoryPath(module), "usertimezone.db")}");
		UserTimeZoneDbContext dbContext = new(optionsBuilder.Options);
		dbContext.Database.EnsureCreated();
		return dbContext;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<UserSetting>().HasKey(s => new { s.UserId });
		modelBuilder.Entity<UserSetting>().Property("timeZone").HasColumnName("TimeZone");
	}
}
