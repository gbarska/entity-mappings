using System.Security.Authentication.ExtendedProtection;
using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore;
using SamuraiApp.Domain;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace SamuraiApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public AppDbContext(){}

        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        // public DbSet<SamuraiBattle> SamuraiBattle { get; set; }

          public static readonly LoggerFactory MyConsoleLoggerFactory
         = new LoggerFactory(new [] {
                new ConsoleLoggerProvider((category, level)
                 => category == DbLoggerCategory.Database.Command.Name 
                 && level == LogLevel.Information, true )});
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // builder.ApplyConfiguration(new CargoMap());
                builder.Entity<SamuraiBattle>()
                    .HasKey(s => new {s.SamuraiId, s.BattleId });
                builder.Entity<Battle>().Property(b => b.StartDate).HasColumnType("Date");
                builder.Entity<Battle>().Property(b => b.EndDate).HasColumnType("Date");
                
                //adding a shadow property only handled by EF and NOT presented in the domain class
                //sintax for single entity shadow properties
                // builder.Entity<Samurai>().Property<DateTime>("Created");
                // builder.Entity<Samurai>().Property<DateTime>("LastModified");

                //sintax to add shadow properties to all entities dynamically
                foreach (var item in builder.Model.GetEntityTypes())
                {
                    builder.Entity(item.Name).Property<DateTime>("Created");
                    builder.Entity(item.Name).Property<DateTime>("LastModified");
                }


        //builder.Entity<Samurai>().HasOne(i=> i.SecretIdentity).WithOne().HasForeignKey<SecretIdentity>(i=>i.SamuraiFK)
            
            //with this notation we say to EF that the owned types must be placed into a separeted table 
            //and entity will create a new table with samurai fk instead of adding columns in samurai table
             // modelBuilder.Entity<Samurai>().OwnsOne(s => s.BetterName).ToTable("BetterNames");

            //with this approach EF wiill just add the columns into samurai table
            builder.Entity<Samurai>().OwnsOne(s => s.BetterName).Property(b => b.GivenName).HasColumnName("GivenName");
            builder.Entity<Samurai>().OwnsOne(s => s.BetterName).Property(b => b.SurName).HasColumnName("SurName");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {            
                 var connString = "Server=localhost;Port=3306;Database=samurai;Uid=gbarska;Pwd=password;";
               
                 builder
                .UseLoggerFactory(MyConsoleLoggerFactory)
                .UseMySql(connString)
                .EnableSensitiveDataLogging(true);
            
        }

         public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            var timestamp = DateTime.Now;

            foreach (var entry in ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added || e.State == EntityState.Modified)
                             && !e.Metadata.IsOwned())
                             )
            {
                entry.Property("LastModified").CurrentValue = timestamp;
                if (entry.State==EntityState.Added)
                {
                    entry.Property("Created").CurrentValue = timestamp;
                }

                if (entry.Entity is Samurai)
                {
                    if (entry.Reference("BetterName").CurrentValue == null)
                    {
                        entry.Reference("BetterName").CurrentValue = PersonFullName.Empty();
                    }
                    entry.Reference("BetterName").TargetEntry.State = entry.State;
                }
            }
            return base.SaveChanges();
        }
       
    }
}