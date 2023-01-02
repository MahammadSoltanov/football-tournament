using GameTournament.ExtraThings;
using GameTournament.MVVM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GameTournament.MVVM.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        #region Fields
        private Store _storeHVM;
        #endregion
        #region Properties        
        public BindingList<Player> PlayersList { get; set; }
        public BindingList<Team> TeamsList { get; set; }
        public RelayCommand AddPlayerCommand { get; set; }
        public RelayCommand AddTeamCommand { get; set; }

        public string NewPlayerName
        {
            get { return _newPlayerName; }
            set
            {                
                _newPlayerName = value;
                OnPropertyChanged();
                AddPlayerCommand.RaiseCanExecuteChanged();                
            }
        }
        private string _newPlayerName;

        public string NewTeamName
        {
            get { return _newTeamName; }
            set
            {
                _newTeamName = value;
                OnPropertyChanged();
                AddTeamCommand.RaiseCanExecuteChanged();
            }
        }
        private string _newTeamName;
        #endregion

        public HomeViewModel(Store store)
        {
            _storeHVM = store;
            DataAccess access = new DataAccess();
            PlayersList = new BindingList<Player>(access.GetPlayers());
            TeamsList = new BindingList<Team>(access.GetTeams());
            AddPlayerCommand = new RelayCommand(AddPlayer, CanAddPlayer);
            AddTeamCommand = new RelayCommand(AddTeam, CanAddTeam);
        }

        private void AddTeam()
        {
            bool canAdd = true;
            foreach (Team team in TeamsList)
            {
                if (team.Name.Equals(NewTeamName))
                {
                    canAdd = false;
                }
            }

            if (!canAdd)
                MessageBox.Show("This name exists in list!");

            else
            {
                DataAccess access = new DataAccess();
                Team NewTeam = new Team() { ID = TeamsList.Count + 1, Name = NewTeamName };
                TeamsList.Add(NewTeam);
                NewTeamName = "";
                MessageBox.Show("Team is added successfully");
                access.AddTeam(NewTeam.Name);
                _storeHVM.OnTeamAdded(NewTeam);
            }
        }

        private bool CanAddTeam()
        {
            if (NewTeamName == null || NewTeamName == "")
                return false;

            else return true;
        }

        private void AddPlayer()
        {
            bool canAdd = true;
            foreach(Player player in PlayersList)
            {
                if(player.Name.Equals(NewPlayerName))
                {
                    canAdd = false;
                }
            }

            if(!canAdd)            
                MessageBox.Show("This name exists in list!");            

            else
            {
                DataAccess access = new DataAccess();
                Player NewPlayer = new Player() { ID = PlayersList.Count + 1, Name = NewPlayerName };
                PlayersList.Add(NewPlayer);
                NewPlayerName = "";
                MessageBox.Show("Player is added successfully");
                access.AddPlayer(NewPlayer.Name);
                _storeHVM.OnPlayerAdded(NewPlayer);
            }
        }

        private bool CanAddPlayer()
        {
            if (NewPlayerName == null || NewPlayerName == "")
                return false;

            else return true;
        }
    }
}
