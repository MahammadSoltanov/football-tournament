using GameTournament.MVVM.Models;
using System.ComponentModel;

namespace GameTournament.Helpers
{
    public static class Mapper
    {
        public static BindingList<Player> GetAllPlayers()
        {
            var access = new DataAccess();

            return new BindingList<Player>(access.GetPlayers());
        }

        public static BindingList<Team> GetAllTeams()
        {
            var access = new DataAccess();

            return new BindingList<Team>(access.GetTeams());
        }
    }
}
