using GameTournament.MVVM.Models;
using System;

namespace GameTournament.ExtraThings
{
    public class Store
    {
        public event Action<Team> TeamAdded;
        public event Action<Player> PlayerAdded;

        public void OnTeamAdded(Team team)
        {
            TeamAdded?.Invoke(team);
        }

        public void OnPlayerAdded(Player player)
        {
            PlayerAdded?.Invoke(player);
        }
    }
}
