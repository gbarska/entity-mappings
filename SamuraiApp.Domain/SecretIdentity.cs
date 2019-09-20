namespace SamuraiApp.Domain
{
    public class SecretIdentity
    {
        public int Id { get; set; }
        public string RealName { get; set; }
        //since we use the convention wich is naming the foreign key with the name of  the entity followed by Id
        //EF recognizes the relation no mapping is required in this case

        //if named the property with something else we would need to explicit mapp
        public int SamuraiId { get; set; }

        //as the secret is the Dependent class the SamuraiId is required, since a secret
        //doesn't exist without a samurai
    }
}
