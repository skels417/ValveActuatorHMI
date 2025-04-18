using System.Data.Entity;
using ValveActuatorHMI.Models;

namespace ValveActuatorHMI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=ValveActuatorDb")
        {
            // Отключаем инициализатор при миграциях
            Database.SetInitializer<AppDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Явно конфигурируем таблицу DeviceConfigurations
            modelBuilder.Entity<DeviceConfiguration>()
                .ToTable("DeviceConfigurations")
                .HasKey(d => d.Id);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<DeviceConfiguration> DeviceConfigurations { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<ParameterValue> ParameterValues { get; set; }
    }
}