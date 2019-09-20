using System.Collections.Generic;
using System;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System.Linq;
using Pomelo.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace SamuraiApp.UI
{
    class Program
    {
        private static AppDbContext _context = new AppDbContext();
    
        static void Main(string[] args)
        {
            GetAllSamurais();
            // PrePopulateSamuraisAndBattles();
            //JoinBattleAndSamurai();
            // EnlistSamuraiIntoABattle();
            // EnlistSamuraiIntoABattleUntracked();
            //AddNewSamuraiViaDisconnectedBattleObject();
            //GetSamuraiWithBattles();
            //GetBattlesForSamuraiInMemory();
            //RemoveJoinBetweenSamuraiAndBattleSimple();
            //RemoveBattleFromSamurai();
            //RemoveBattleFromSamuraiWhenDisconnected();
            //AddNewSamuraiWithSecretIdentity();
            //AddSecretIdentityUsingSamuraiId();
            //AddSecretIdentityToExistingSamurai();
            //EditASecretIdentity();
            // ReplaceASecretIdentity();
            //ReplaceASecretIdentityNotTracked();
            //ReplaceSecretIdentityNotInMemory();
            // CreateSamuraiWithBetterName();
            //  ReplaceBetterName();
            // CreateAndFixUpNullBetterName();

            //  RetrieveScalarResult();
            //FilterWithScalarResult();
            //SortWithScalar();
            //SortWithoutReturningScalar();
            //RetrieveBattleDays();
            //RetrieveBattleDaysWithoutDbFunction();

            // _context.SamuraiBattleStats.ToList();
        }

        private static void GetStats()
        {
            var stats = _context.SamuraiBattleStats.AsNoTracking().ToList();
        }
        private static void Filter()
        {
            var stats = _context.SamuraiBattleStats.Where(s => s.SamuraiId == 2).AsNoTracking().ToList();
        }
        private static void Project()
        {
            var stats = _context.SamuraiBattleStats.AsNoTracking().Select(s => new { s.Name, s.NumberOfBattles }).ToList();
        }
        private static void RetrieveYearUsingDbBuiltInFunction()
        {
           var battles=_context.Battles
                .Select(b=>new { b.Name, b.StartDate.Year }).ToList();
        }

        private static void RetrieveScalarResult()
        {
            var samurais = _context.Samurais
                .Select(s => new
                {
                    s.Name,
                    EarliestBattle = AppDbContext.EarliestBattleFoughtBySamurai(s.Id)
                })
                .ToList();
        }
        private static void FilterWithScalarResult()
        {
            var samurais = _context.Samurais
                    .Where(s => EF.Functions.Like(AppDbContext.EarliestBattleFoughtBySamurai(s.Id), "%Battle%"))
                    .Select(s => new
                    {
                        s.Name,
                        EarliestBattle = AppDbContext.EarliestBattleFoughtBySamurai(s.Id)
                    })
                   .ToList();
        }
        private static void SortWithScalar()
        {
            var samurais = _context.Samurais
                 .OrderBy(s => AppDbContext.EarliestBattleFoughtBySamurai(s.Id))
                 .Select(s => new { s.Name, EarliestBattle = AppDbContext.EarliestBattleFoughtBySamurai(s.Id) })
                 .ToList();
        }
        private static void SortWithoutReturningScalar()
        {
            var samurais = _context.Samurais
                 .OrderBy(s => AppDbContext.EarliestBattleFoughtBySamurai(s.Id))
                 .ToList();
        }
        private static void RetrieveBattleDays()
        {
            var battles = _context.Battles.Select(b => new { b.Name, Days = AppDbContext.DaysInBattle(b.StartDate, b.EndDate) }).ToList();
        }

        private static void RetrieveBattleDaysWithoutDbFunction()
        {
            var battles = _context.Battles.Select(
                b => new {
                    b.Name,
                    Days = DateDiffDaysPlusOne(b.StartDate, b.EndDate)
                }
                ).ToList();
        }
        private static int DateDiffDaysPlusOne(DateTime start, DateTime end)
        {
            return (int)end.Subtract(start).TotalDays + 1;
        }
    private static void CreateSamuraiWithBetterName()
        {
            var samurai = new Samurai
            {
                Name = "Jack le Black",
                BetterName = PersonFullName.Create("Jack", "Black")
            };

            var person=  PersonFullName.Create("bla","nla");
            samurai.BetterName= person;

            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void CreateAndFixUpNullBetterName()
        {
            _context.Samurais.Add(new Samurai { Name = "bob"});
            _context.SaveChanges();
            _context = new AppDbContext();
            
            var persistedSamurai = _context.Samurais.FirstOrDefault(s => s.Name == "Chrisjen");
            
            if (persistedSamurai is null) { return; }
            if (persistedSamurai.BetterName.IsEmpty())
            {
                persistedSamurai.BetterName = null;
            }
        }
    
        private static void ReplaceBetterName()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == "Ruronin");
            //workaround for the second problem
            _context.Entry(samurai).Reference(s => s.BetterName).TargetEntry.State = EntityState.Detached;
            samurai.BetterName = PersonFullName.Create("Shohreh", "Aghdashloo");
            _context.Samurais.Update(samurai);
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateBetterName()
        {
           var samurai = _context.Samurais.FirstOrDefault(s => s.BetterName.SurName == "Black");
        //    samurai.BetterName.GivenName = "Jill";
           _context.SaveChanges();
        }

        private static void GetAllSamurais()
        {
            var allsamurais = _context.Samurais.ToList();
        }
        private static void CreateSamurai()
        {
            var samurai = new Samurai { Name = "Ronin" };
            _context.Samurais.Add(samurai);
            _context.Entry(samurai).Property("Created").CurrentValue = DateTime.Now;
            _context.Entry(samurai).Property("LastModified").CurrentValue = DateTime.Now;
            _context.SaveChanges();
        }
        private static void ReplaceSecretIdentityNotInMemory()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.SecretIdentity != null);
            samurai.SecretIdentity = new SecretIdentity { RealName = "Bobbie Draper" };
            _context.SaveChanges(); 
        } 
        private static void ReplaceASecretIdentityNotTracked()
        {
            Samurai samurai;
            using (var separateOperation = new AppDbContext())
            {
                samurai = separateOperation.Samurais.Include(s => s.SecretIdentity)
                                           .FirstOrDefault(s => s.Id == 1);
            }
            samurai.SecretIdentity = new SecretIdentity { RealName = "Sampson" };
            _context.Samurais.Attach(samurai);
            //this will fail...EF Core tries to insert a duplicate samuraiID FK
            _context.SaveChanges();
        }
        private static void ReplaceASecretIdentity()
        {
            var samurai = _context.Samurais.Include(s => s.SecretIdentity)
                                  .FirstOrDefault(s => s.Id == 1);
            samurai.SecretIdentity = new SecretIdentity { RealName = "Sampson" };
            _context.SaveChanges();
        }
        private static void GetSamuraiWithBattles()
        {
            var samuraiWithBattles = 
                _context.Samurais
                .Include(s => s.SamuraiBattles)
                .ThenInclude(sb => sb.Battle).FirstOrDefault(s => s.Id == 1);

            var battle = samuraiWithBattles.SamuraiBattles.First().Battle;

            var allTheBattles = new List<Battle>();

            foreach (var samuraiBattle in samuraiWithBattles.SamuraiBattles)
            {
                allTheBattles.Add(samuraiBattle.Battle);
            }
        }
        private static void JoinBattleAndSamurai()
        {
            var sbJoin = new SamuraiBattle { SamuraiId = 1, BattleId = 3 };
            
            //as SamuraiBattle is a join entity we don't have a Dbset and in this case 
            //the context itself handles the query 
            _context.Add(sbJoin);
            _context.SaveChanges();
        }             
        private static void EditASecretIdentity()
        {
            var samurai = _context.Samurais.Include(s => s.SecretIdentity)
                                  .FirstOrDefault(s => s.Id == 1);
            samurai.SecretIdentity.RealName = "T'Challa";
            _context.SaveChanges();
        }
        private static void AddSecretIdentityToExistingSamurai()
        {
            Samurai samurai;
            using (var separateOperation = new AppDbContext())
            {
                samurai = _context.Samurais.Find(2);
            }
            samurai.SecretIdentity = new SecretIdentity { RealName = "Julia" };
            _context.Samurais.Attach(samurai);
            _context.SaveChanges();
        }
        private static void AddSecretIdentityUsingSamuraiId()
        {
            //Note: SamuraiId 1 does not have a secret identity yet!
            var identity = new SecretIdentity { SamuraiId = 1,  };
            _context.Add(identity);
            _context.SaveChanges();
        }   
        private static void AddNewSamuraiWithSecretIdentity()
        {
            var samurai = new Samurai { Name = "Jina Ujichika" };
            samurai.SecretIdentity = new SecretIdentity { RealName = "Julie" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void RemoveBattleFromSamuraiWhenDisconnected()
        {
            //Goal:Remove join between Shichirōji(Id=3) and Battle of Okehazama (Id=1)
            Samurai samurai;
            using (var separateOperation = new AppDbContext())
            {
                samurai = separateOperation.Samurais.Include(s => s.SamuraiBattles)
                                                    .ThenInclude(sb => sb.Battle)
                                           .SingleOrDefault(s => s.Id == 3);
            }
            var sbToRemove = samurai.SamuraiBattles.SingleOrDefault(sb => sb.BattleId == 1);
            samurai.SamuraiBattles.Remove(sbToRemove);
            //_context.Attach(samurai);
            //_context.ChangeTracker.DetectChanges();
            _context.Remove(sbToRemove);
            _context.SaveChanges();
        }
        private static void RemoveBattleFromSamurai()
        {
            //Goal:Remove join between Shichirōji(Id=3) and Battle of Okehazama (Id=1)
            var samurai = _context.Samurais.Include(s => s.SamuraiBattles)
                                           .ThenInclude(sb => sb.Battle)
                                  .SingleOrDefault(s => s.Id == 3);
             var sbToRemove = samurai.SamuraiBattles.SingleOrDefault(sb => sb.BattleId == 1);
             samurai.SamuraiBattles.Remove(sbToRemove); //remove via List<T>
             //_context.Remove(sbToRemove); //remove using DbContext
             _context.ChangeTracker.DetectChanges(); //here for debugging
             _context.SaveChanges();
        }
        private static void RemoveJoinBetweenSamuraiAndBattleSimple()
        {
            var join = new SamuraiBattle { BattleId = 1, SamuraiId = 8 };
            _context.Remove(join);
            _context.SaveChanges();
        }
        private static void EnlistSamuraiIntoABattle()
        {
            var battle = _context.Battles.Find(1);

            battle.SamuraiBattles.Add(new SamuraiBattle{SamuraiId = 3});
            _context.SaveChanges();
        }
        private static void EnlistSamuraiIntoABattleUntracked()
        {
            Battle battle;
            
            using (var separateOperation= new AppDbContext())
            {
                battle = separateOperation.Battles.Find(1);
            } 

            battle.SamuraiBattles.Add(new SamuraiBattle{SamuraiId = 2});
            _context.Battles.Attach(battle);
            _context.ChangeTracker.DetectChanges();//show debugging info before operation is done
            _context.SaveChanges();
        }
        private static void AddNewSamuraiViaDisconnectedBattleObject()
        {
            Battle battle;
            
            using (var separateOperation= new AppDbContext())
            {
                battle = separateOperation.Battles.Find(1);
            } 
            var newSamurai = new Samurai { Name = "SampsonSan" };

            battle.SamuraiBattles.Add(new SamuraiBattle{ Samurai = newSamurai});
            
            _context.Battles.Attach(battle);
            // _context.ChangeTracker.DetectChanges();//show debugging info before operation is done
            _context.SaveChanges();
        }       
        private static void AddSamuraiShadowProperty()
        {
            var samurai = new Samurai { Name="Bushido" };

            _context.Samurais.Add(samurai);

            var timestamp = DateTime.Now;
            // we use the property to add the shadow properties, we can't use linq as both aren't real property of our classes
            _context.Entry(samurai).Property("Created").CurrentValue = timestamp;
            _context.Entry(samurai).Property("LastModified").CurrentValue= timestamp;
            _context.SaveChanges();           
       }
        private static void RetrieveSamuraisCreatedInPastWeek()
        {
            var oneWeekAgo = DateTime.Now.AddDays(-7);

            var newSamurais = _context.Samurais
                                .Where(s => EF.Property<DateTime>(s,"Created") >= oneWeekAgo)
                                .ToList();
                                
            //objectwith the field of shadow property
            var newSamuraisObject = _context.Samurais
                                .Where(s => EF.Property<DateTime>(s,"Created") >= oneWeekAgo)
                                .Select(s => new { s.Id, s.Name, Created=EF.Property<DateTime>(s,"Created")})
                                .ToList();                  
        }
        private static void CreateThenEditSamuraiWithQuote()
        {
            var samurai = new Samurai { Name = "Ronin" };
            var quote = new Quote { Text = "Aren't I MARVELous?" };
            samurai.Quotes.Add(quote);
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
            quote.Text += " See what I did there?";
            _context.SaveChanges();
        }
        private static void PrePopulateSamuraisAndBattles()
        {
            //working on DbContext that figures out what to do (where to insert)
            _context.AddRange(
                new Samurai {Name = "Kikuchiyo" },
                new Samurai {Name = "Kambei Shimada" },
                new Samurai {Name = "Shichiroji" },
                new Samurai {Name = "Katsushiro Okamoto" },
                new Samurai {Name = "Heihachi Hayashida" },
                new Samurai {Name = "Gorobei Katayama" }
            );

                //working on DbSet Battles
              _context.Battles.AddRange(
                new Battle {Name = "Battle of Okehazama", StartDate = new DateTime(1560,05,10) , EndDate = new DateTime(1561,05,10)},
                new Battle {Name = "Battle of Shiroyama",StartDate = new DateTime(1562,05,10) , EndDate = new DateTime(1563,05,10) },
                new Battle {Name = "Siege of Osaka",StartDate = new DateTime(1564,05,10) , EndDate = new DateTime(1565,05,10) },
                new Battle {Name = "Boshin War",StartDate = new DateTime(1570,05,10) , EndDate = new DateTime(1571,05,10) }
            );

            _context.SaveChanges();
        }
    }
}
