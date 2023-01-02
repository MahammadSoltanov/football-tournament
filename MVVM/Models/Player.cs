using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTournament.MVVM.Models
{
    public class Player 
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Team TeamInTournament { get; set; }
        public int OpponentCount { get; set; }
        public int WonTournamentNumber { get; set; }
    }
}
