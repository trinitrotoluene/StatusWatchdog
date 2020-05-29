using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServiceWatchdog.Api.Services.Entities;

namespace ServiceWatchdog.Api.Services
{
    public class WatchdogContext : DbContext
    {
        public DbSet<ServiceModel> Services { get; set; }

        public DbSet<IncidentModel> Incidents { get; set; }

        public DbSet<IncidentUpdateModel> IncidentUpdates { get; set; }

        public DbSet<KeyValueModel> KeyValueStore { get; set; }

        public DbSet<MetricModel> Metrics { get; set; }

        public DbSet<MetricEntryModel> MetricEntries { get; set; }

        private readonly string _connectionString;

        public WatchdogContext(IConfiguration config)
        {
            string username = config["PG_USERNAME"];
            string password = config["PG_PASSWORD"];
            string database = config["PG_DATABASE"];
            string host = config["PG_HOST"];

            _connectionString = $"Host={host};Database={database};Username={username};Password={password}";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var service = builder.Entity<ServiceModel>();

            service.HasKey(x => x.Id);
            service.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            service.HasIndex(x => x.Slug)
                .IsUnique();
            service.Property(x => x.Slug)
                .HasMaxLength(32)
                .IsRequired();

            service.Property(x => x.DisplayName)
                .HasMaxLength(32)
                .IsRequired();

            service.Property(x => x.Description)
                .HasMaxLength(32)
                .IsRequired();

            service.Property(x => x.Status)
                .IsRequired();

            service.Property(x => x.CreatedAt)
                .HasDefaultValueSql("timezone('utc', now())")
                .IsRequired();

            service.HasMany(x => x.Incidents)
                .WithOne(x => x.Service)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            var incident = builder.Entity<IncidentModel>();

            incident.HasKey(x => x.Id);
            incident.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            incident.Property(x => x.Title)
                .HasMaxLength(128)
                .IsRequired();

            incident.Property(x => x.CreatedAt)
                .HasDefaultValueSql("timezone('utc', now())")
                .IsRequired();

            incident.Property(x => x.ResolvedAt)
                .HasDefaultValue(null)
                .IsRequired(false);

            incident.Property(x => x.State)
                .IsRequired();

            incident.Property(x => x.CausedStatus)
                .IsRequired();

            incident.Property(x => x.MostRecentUpdateId)
                .IsRequired();

            incident.HasMany(x => x.Updates)
                .WithOne(x => x.Incident)
                .HasForeignKey(x => x.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            var incidentUpdate = builder.Entity<IncidentUpdateModel>();

            incidentUpdate.HasKey(x => x.Id);
            incidentUpdate.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            incidentUpdate.Property(x => x.Message)
                .HasMaxLength(2048)
                .IsRequired();

            incidentUpdate.Property(x => x.State)
                .IsRequired();

            incidentUpdate.Property(x => x.CreatedAt)
                .HasDefaultValueSql("timezone('utc', now())")
                .IsRequired();

            var keyValueStore = builder.Entity<KeyValueModel>();

            keyValueStore.HasKey(x => x.Key);
            keyValueStore.Property(x => x.Key)
                .IsRequired();

            keyValueStore.Property(x => x.Value)
                .HasMaxLength(8192)
                .IsRequired();

            var metric = builder.Entity<MetricModel>();

            metric.HasKey(x => x.Id);
            metric.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            metric.Property(x => x.Name)
                .IsRequired();

            metric.Property(x => x.Displayed)
                .HasDefaultValue(false)
                .IsRequired();

            metric.HasOne(x => x.Service)
                .WithMany(x => x.Metrics)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            var metricEntry = builder.Entity<MetricEntryModel>();

            metricEntry.HasKey(x => x.Id);
            metricEntry.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            metricEntry.Property(x => x.Value)
                .IsRequired();

            metricEntry.Property(x => x.Tag)
                .IsRequired();
        }
    }
}
