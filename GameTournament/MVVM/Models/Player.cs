namespace GameTournament.MVVM.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Team TeamInTournament { get; set; }
        public int OpponentCount { get; set; }
        public int WonTournamentNumber { get; set; }
    }
}
