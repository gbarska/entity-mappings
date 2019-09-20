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
            //overrides the SaveChanges method to populate the shadow props for every entity
            
            ChangeTracker.DetectChanges();
            
            var timeStamp =  DateTime.Now;

            foreach (var entry in ChangeTracker.Entries()
                .Where( e => e.State == EntityState.Added || e.State == EntityState.Modified))
                
            {
                entry.Property("LastModified").CurrentValue= timeStamp;

                if(entry.State == EntityState.Added)
                {
                    entry.Property("Created").CurrentValue= timeStamp;
                }
            }
            return base.SaveChanges();
        }
    }

    // public class ContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    // {
    //       public static readonly LoggerFactory MyConsoleLoggerFactory
    //      = new LoggerFactory(new [] {
    //             new ConsoleLoggerProvider((category, level)
    //              => category == DbLoggerCategory.Database.Command.Name 
    //              && level == LogLevel.Information, true )});
    //     public AppDbContext CreateDbContext(string[] args)
    //     {
    //         var config = new ConfigurationBuilder().Build();
    //         var builder = new DbContextOptionsBuilder<AppDbContext>();
    //         var connString = "Server=localhost;Port=3306;Database=samurai;Uid=gbarska;Pwd=password;";
    //         builder
    //         .UseLoggerFactory(MyConsoleLoggerFactory)
    //         .UseMySql(connString)
    //         .EnableSensitiveDataLogging(true);
            
    //         return new AppDbContext(builder.Options);
    //     }
    // }
}