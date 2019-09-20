using System;
using System.Collections.Generic;

namespace SamuraiApp.Domain
{
    public class Samurai
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Quote> Quotes { get; set; }
        public PersonFullName BetterName { get; set; }
        //many to many relationship
        public List<SamuraiBattle> SamuraiBattles { get; set; }

        //one to one relationship, since the secret entity doesn't even exists without the samurai
        // samurai is the principal entity and secret is the dependent

        //as the samurai entity is the principal entity the secret identity isn't required
        //ef doens't enforce that by definition we must do that with business logic
        public SecretIdentity SecretIdentity { get; set; }


        //approach to get the grandchild Battles directly
        public List<Battle> Battles()
        {
            var battles = new List<Battle>();

            foreach (var item in SamuraiBattles)
            {
                battles.Add(item.Battle);
            }
            return battles;
        }

        public Samurai()
        {
            Quotes = new List<Quote>();
            SamuraiBattles= new List<SamuraiBattle>();
        }
    }
}
