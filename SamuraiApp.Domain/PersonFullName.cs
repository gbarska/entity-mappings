using System.Collections.Generic;

namespace SamuraiApp.Domain
{
    public class PersonFullName
    {
        public static PersonFullName Create(string givenName, string surName)
        {
            return new PersonFullName(givenName, surName);
        }
          //this method was created to overcome the known problem with owned types,
        //OWNED TYPES cannot be null at persistency time, otherwise EF will throw an error
        public static PersonFullName Empty()
        {
            return new PersonFullName("", "");
        }
        private PersonFullName(string givenName, string surName)
        {
            SurName = surName;
            GivenName = givenName;
        }
          //private parameterless constructor REQUIRED to be used by EF with reflection 
        private PersonFullName() { }

        //this class is a value object, it doesn't have a Id key for  EF
        //setters are private to ensure data is sent through constructors
        //it is handled by EF as an owned type
        public string SurName { get; private set; }
        public string GivenName { get; private set; }
        public string FullName => $"{GivenName} {SurName}";
        public string FullNameReverse => $"{SurName}, {GivenName}";

         public bool IsEmpty()
        {
            return SurName == "" & GivenName == "";
        }

        public override bool Equals(object obj)
        {
            var name = obj as PersonFullName;
            return name != null &&
                   SurName == name.SurName &&
                   GivenName == name.GivenName;
        }

         public override int GetHashCode()
        {
            var hashCode = -1052426677;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SurName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(GivenName);
            return hashCode;
        }

         public static bool operator ==(PersonFullName name1, PersonFullName name2)
        {
            return EqualityComparer<PersonFullName>.Default.Equals(name1, name2);
        }

        public static bool operator !=(PersonFullName name1, PersonFullName name2)
        {
            return !(name1 == name2);
        }
    }
}