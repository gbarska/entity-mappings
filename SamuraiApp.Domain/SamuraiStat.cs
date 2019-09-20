
namespace SamuraiApp.Domain
{
    public class SamuraiStat: DbView
    {
        public int SamuraiId { get; private set; }
        public string Name { get; private set; }
        public int NumberOfBattles { get; private set; }
        // public string EarliestBattle { get; private set; }
    }
}
