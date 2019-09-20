using System.IO.Compression;
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
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        public DbQuery<SamuraiStat> SamuraiBattleStats { get; set; }
        // public DbSet<SamuraiBattle> SamuraiBattle { get; set; }

          public static readonly LoggerFactory MyConsoleLoggerFactory
         = new LoggerFactory(new [] {
                new ConsoleLoggerProvider((category, level)
                 => category == DbLoggerCategory.Database.Command.Name 
                 && level == LogLevel.Information, true )});
      
       [DbFunction(Schema ="dbo")]
        public static string EarliestBattleFoughtBySamurai(int samuraiId)
        {
              throw new Exception();
        }

       
        [DbFunction(Schema = "dbo")]
        public  static int DaysInBattle(DateTime start, DateTime end)
        {
            return (int)end.Subtract(start).TotalDays + 1;
        }
     protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamuraiBattle>()
               .HasKey(s => new { s.SamuraiId, s.BattleId });
            modelBuilder.Entity<Battle>().Property(b => b.StartDate).HasColumnType("Date");
            modelBuilder.Entity<Battle>().Property(b => b.EndDate).HasColumnType("Date");

             //adding a shadow property only handled by EF and NOT presented in the domain class
             //sintax for single entity shadow properties
            // builder.Entity<Samurai>().Property<DateTime>("Created");
             // builder.Entity<Samurai>().Property<DateTime>("LastModified");

             //sintax to add shadow properties to all entities dynamically
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
               if (entityType.ClrType.BaseType != typeof(DbView))
               {
                modelBuilder.Entity(entityType.Name).Property<DateTime>("Created");
                modelBuilder.Entity(entityType.Name).Property<DateTime>("LastModified");   
               }

            }
          
            //builder.Entity<Samurai>().HasOne(i=> i.SecretIdentity).WithOne().HasForeignKey<SecretIdentity>(i=>i.SamuraiFK)
            
            //with this notation we say to EF that the owned types must be placed into a separeted table 
            //and entity will create a new table with samurai fk instead of adding columns in samurai table
             
             //accepts the owned types to be null as it is in a separate tables EF3
             // modelBuilder.Entity<Samurai>().OwnsOne(s => s.BetterName).ToTable("BetterNames");

            //with this approach EF wiill just add the columns into samurai table, problemwith nullable properties
            modelBuilder.Entity<Samurai>().OwnsOne(s => s.BetterName).Property(b => b.GivenName).HasColumnName("GivenName");
            modelBuilder.Entity<Samurai>().OwnsOne(s => s.BetterName).Property(b => b.SurName).HasColumnName("SurName");

            modelBuilder.Query<SamuraiStat>().ToView("SamuraiBattleStats");
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

            var copy = ChangeTracker.Entries()
            .Where(e => (e.State == EntityState.Added || e.State == EntityState.Modified)
                             && !e.Metadata.IsOwned());
          

            // var id =
            //metadata owned types are treated as regular entries to differ one from another
            //must check in metadata

            //problem with workaround alter element inside foreach loop
            try{
          foreach (var entry in copy)
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
                      Console.WriteLine("Name:", entry);
                    //   copy2.
                    }
                     entry.Reference("BetterName").TargetEntry.State = entry.State;

                     
                }
            }
            }
           

            catch(Exception e)
            {
                    //
            }

            // ChangeTracker= 

            return base.SaveChanges();
            
        }
       
    }
}