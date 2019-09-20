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
            
            // PrePopulateSamuraisAndBattles();          
            
            // JoinBattleAndSamurai();
            
            // AddSamuraiShadowProperty();
            
            // RetrieveSamuraisCreatedInPastWeek();
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
                                .where(s => EF.Property<DateTime>(s,"Created") >= oneWeekAgo)
                                .ToList();
                                
            //objectwith the field of shadow property
            var newSamuraisObject = _context.Samurais
                                .where(s => EF.Property<DateTime>(s,"Created") >= oneWeekAgo)
                                .Select(s => new { s.Id, s.Name, Created=EF.Property<DateTime>(s,"Created")})
                                .ToList();                  
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
