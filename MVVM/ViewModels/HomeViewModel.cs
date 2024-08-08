using GameTournament.Helpers;
using GameTournament.MVVM.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;


namespace GameTournament.MVVM.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        private Store _storeHVM;

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
            get { return _NewTeamName; }
            set
            {
                _NewTeamName = value;
                OnPropertyChanged();
                AddTeamCommand.RaiseCanExecuteChanged();
            }
        }
        private string _NewTeamName;
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


        #region TeamMethods
        private void AddTeam()
        {
            if (DoesTeamExist())
            {
                MessageBox.Show("This team already exists.");
            }

            var newTeam = new Team() { Name = NewTeamName, Id = TeamsList.Last().Id + 1 };
            TeamsList.Add(newTeam);
            try
            {
                var _dataAccess = new DataAccess();
                _dataAccess.AddTeam(newTeam.Name);
                _storeHVM.OnTeamAdded(newTeam);
                NewTeamName = "";
                MessageBox.Show("Team is added successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CanAddTeam()
        {
            if (string.IsNullOrWhiteSpace(NewTeamName))
            {
                return false;
            }

            return true;
        }

        private bool DoesTeamExist()
        {
            foreach (Team team in TeamsList)
            {
                if (team.Name.ToLower().Equals(NewTeamName.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region PlayerMethods
        private void AddPlayer()
        {
            if (DoesPlayerExist())
            {
                MessageBox.Show("This player already exists.");
            }

            Player NewPlayer = new Player() { Name = NewPlayerName, Id = PlayersList.Last().Id + 1 };
            PlayersList.Add(NewPlayer);

            try
            {
                var _dataAccess = new DataAccess();
                _dataAccess.AddPlayer(NewPlayer.Name);
                _storeHVM.OnPlayerAdded(NewPlayer);
                NewPlayerName = "";
                MessageBox.Show("Player is added successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CanAddPlayer()
        {
            if (string.IsNullOrWhiteSpace(NewPlayerName))
            {
                return false;
            }

            return true;
        }

        private bool DoesPlayerExist()
        {
            foreach (Player player in PlayersList)
            {
                if (player.Name.ToLower().Equals(NewPlayerName.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}