using GameTournament.ExtraThings;
using GameTournament.MVVM.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace GameTournament.MVVM.ViewModels
{
    public class TournamentViewModel : ObservableObject
    {
        #region Fields
        private Store _storeTVM;
        private int _tourNumber = 0;
        private bool _canEnd = true;
        private Team _selectedTeam;
        private BindingList<Player> _allPlayers;
        private BindingList<Team> _allTeams;
        #endregion

        #region ViewModelProperties        
        public BindingList<Player> Winners { get; set; }
        public BindingList<State> Statistics { get; set; }
        public BindingList<Match> Matches { get; set; }
        public BindingList<Player> PlayersInTournament { get; set; }
        public Player SelectedPlayer
        {
            get { return _selectedPlayer; }
            set
            {
                _selectedPlayer = value;
                AddToTournamentCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }
        private Player _selectedPlayer;
        public Team SelectedTeam
        {
            get { return _selectedTeam; }
            set
            {
                _selectedTeam = value;
                AddToTournamentCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        public BindingList<Player> AllPlayers
        {
            get { return _allPlayers; }
            set
            {
                _allPlayers = value;
                OnPropertyChanged();
            }
        }

        public BindingList<Team> AllTeams
        {
            get { return _allTeams; }
            set
            {
                _allTeams = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand AddToTournamentCommand { get; set; }
        public RelayCommand StartTournamentCommand { get; set; }
        public RelayCommand EndTournamentCommand { get; set; }
        public RelayCommand PlayAgainCommand { get; set; }

        #endregion

        #region ViewProperties

        public Visibility AddStackPanelVisibility
        {
            get { return _addStackPanelVisibility; }
            set
            {
                if (_addStackPanelVisibility != value)
                {
                    _addStackPanelVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        private Visibility _addStackPanelVisibility;

        public Visibility MatchesListBoxVisibility
        {
            get { return _matchesListBoxVisibility; }
            set
            {
                if (_matchesListBoxVisibility != value)
                {
                    _matchesListBoxVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        private Visibility _matchesListBoxVisibility = Visibility.Hidden;

        public Visibility StatisticsTableVisibility
        {
            get { return _statisticsVisibility; }
            set
            {
                if (_statisticsVisibility != value)
                {
                    _statisticsVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        private Visibility _statisticsVisibility = Visibility.Hidden;

        public Visibility PlayAgainButtonVisibility
        {
            get { return _playAgainButtonVisibility; }
            set
            {
                if (_playAgainButtonVisibility != value)
                {
                    _playAgainButtonVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        private Visibility _playAgainButtonVisibility = Visibility.Hidden;

        public Visibility WinnersStackPanelVisibility
        {
            get { return _winnersStackPanelVisibility; }
            set
            {
                if (_winnersStackPanelVisibility != value)
                {
                    _winnersStackPanelVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        private Visibility _winnersStackPanelVisibility = Visibility.Hidden;

        #endregion

        public TournamentViewModel(Store store)
        {
            _storeTVM = store;
            AllPlayers = GetAllPlayers();
            AllTeams = GetAllTeams();

            Statistics = new BindingList<State>();
            Winners = new BindingList<Player>();
            AddToTournamentCommand = new RelayCommand(AddToTournament, CanAddToTournament);
            StartTournamentCommand = new RelayCommand(StartTournament, CanStartTournament);
            EndTournamentCommand = new RelayCommand(EndTournament, CanEndTournament);
            PlayAgainCommand = new RelayCommand(PlayAgain);
            PlayersInTournament = new BindingList<Player>();
            Matches = new BindingList<Match>();
            _storeTVM.PlayerAdded += _storeTVM_PlayerAdded;
            _storeTVM.TeamAdded += _storeTVM_TeamAdded;
        }

        #region MVVM and Commands
        private void PlayAgain()
        {
            SetDefaultVisibilities();

            //Changing End button's activity
            _canEnd = true;
            EndTournamentCommand.RaiseCanExecuteChanged();
            ClearTournamentInformation();

            AllPlayers = GetAllPlayers();
            AllTeams = GetAllTeams();

            AddStackPanelVisibility = Visibility.Visible;
        }

        private void _storeTVM_TeamAdded(Team team)
        {
            AllTeams.Add(team);
        }

        private void _storeTVM_PlayerAdded(Player player)
        {
            AllPlayers.Add(player);
        }

        private void EndTournament()
        {
            CalculatePoints();
            DetermineWinner();

            _canEnd = false;
            PlayAgainButtonVisibility = Visibility.Visible;
            WinnersStackPanelVisibility = Visibility.Visible;
            EndTournamentCommand.RaiseCanExecuteChanged();
        }

        private bool CanEndTournament()
        {
            return _canEnd;
        }

        private void StartTournament()
        {
            _tourNumber++;
            AddStackPanelVisibility = Visibility.Hidden;
            StatisticsTableVisibility = Visibility.Visible;

            GenerateRoundRobinMatches();
            Matches.ListChanged += Matches_ListChanged;
            InitializePlayerStatistics();

            MatchesListBoxVisibility = Visibility.Visible;
        }

        private void Matches_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                int changedIndex = e.NewIndex;

                var changedMatch = Matches[changedIndex];
                var player1 = changedMatch.Player1;
                var player2 = changedMatch.Player2;

                bool result1 = UpdateScoresAndStatistics(player1.Id, player2.Id, changedMatch.Goals1, changedMatch.Score1IsEditable);
                bool result2 = UpdateScoresAndStatistics(player2.Id, player1.Id, changedMatch.Goals2, changedMatch.Score2IsEditable);

                changedMatch.Score1IsEditable = result1;
                changedMatch.Score2IsEditable = result2;
            }
        }

        private bool CanStartTournament()
        {
            if (PlayersInTournament.Count < 2)
                return false;

            else return true;
        }

        private void AddToTournament()
        {
            SelectedPlayer.TeamInTournament = SelectedTeam;
            PlayersInTournament.Add(SelectedPlayer);
            AllPlayers.Remove(SelectedPlayer);
            AllTeams.Remove(SelectedTeam);

            if (PlayersInTournament.Count == 2)
            {
                StartTournamentCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanAddToTournament()
        {
            if (SelectedPlayer == null || SelectedTeam == null)
            {
                return false;
            }

            else return true;
        }
        #endregion

        #region Winner calculation logic
        private void CalculatePoints()
        {
            foreach (Match match in Matches)
            {
                if (match.Goals1 > match.Goals2)
                {
                    foreach (State state in Statistics)
                    {

                        if (state.PlayerInGame.Id == match.Player1.Id)
                        {
                            state.Points += 3;
                        }
                    }
                }


                if (match.Goals1 < match.Goals2)
                {
                    foreach (State state in Statistics)
                    {

                        if (state.PlayerInGame.Id == match.Player2.Id)
                        {

                            state.Points += 3;
                        }
                    }
                }


                if (match.Goals1 == match.Goals2)
                {
                    foreach (State state in Statistics)
                    {
                        if (state.PlayerInGame.Id == match.Player1.Id)
                        {
                            state.Points += 1;
                        }

                        if (state.PlayerInGame.Id == match.Player2.Id)
                        {
                            state.Points += 1;
                        }
                    }
                }
            }
        }

        //Checks if there is a winnner based on list of numeric values
        bool CheckForWinner(List<int> list, out int winnerIndex)
        {
            winnerIndex = -1;
            if (list.Distinct().Count() == 1)
            {
                return false; // All values are the same, no winner.
            }

            int maxValue = list.Max();
            bool hasDuplicateMax = list.Count(x => x == maxValue) > 1;

            if (hasDuplicateMax)
            {
                return false; // There are duplicates of the max value, no winner.
            }

            winnerIndex = list.IndexOf(maxValue);
            return true; // We have a unique winner.
        }

        void DetermineWinner()
        {
            List<int> PointsList = new List<int>();
            List<int> SumList = new List<int>();

            foreach (State state in Statistics)
            {
                PointsList.Add(state.Points);
                SumList.Add(state.Points + state.AverageGoals);
            }

            int winnerIndex;

            if (CheckForWinner(PointsList, out winnerIndex))
            {
                DeclareWinner(winnerIndex);
            }
            else if (CheckForWinner(SumList, out winnerIndex))
            {
                DeclareWinner(winnerIndex);
            }
            else
            {
                NoWinner();
            }
        }

        void DeclareWinner(int winnerIndex)
        {
            MessageBox.Show($"The winner is {Statistics[winnerIndex].PlayerInGame.Name}");
            Statistics[winnerIndex].PlayerInGame.WonTournamentNumber = _tourNumber;
            Winners.Add(Statistics[winnerIndex].PlayerInGame);
        }

        void NoWinner()
        {
            MessageBox.Show("There is no winner in this tournament");
            Player winner = new Player() { WonTournamentNumber = _tourNumber, Name = "No winner" };
            Winners.Add(winner);
        }
        #endregion

        #region Helper methods
        private bool UpdateScoresAndStatistics(int scorerId, int opponentId, int goals, bool scoreIsEditable)
        {
            if (goals > 0 && !scoreIsEditable)
            {
                scoreIsEditable = true;
                foreach (State state in Statistics)
                {
                    // Remove points from the opponent
                    if (state.PlayerInGame.Id == opponentId)
                    {
                        state.AverageGoals -= goals;
                    }

                    // Add points to the scorer
                    if (state.PlayerInGame.Id == scorerId)
                    {
                        state.AverageGoals += goals;
                    }
                }
            }

            return scoreIsEditable;
        }
        private void SetDefaultVisibilities()
        {
            //Setting visibilities to default ones
            StatisticsTableVisibility = Visibility.Hidden;
            PlayAgainButtonVisibility = Visibility.Hidden;
            MatchesListBoxVisibility = Visibility.Hidden;
        }

        private void ClearTournamentInformation()
        {
            //Clearing tournament and setting start button's activity
            PlayersInTournament.Clear();
            StartTournamentCommand.RaiseCanExecuteChanged();
            Statistics.Clear();
            Matches.Clear();
            AllPlayers.Clear();
            AllTeams.Clear();
        }

        private void GenerateRoundRobinMatches()
        {
            foreach (Player player1 in PlayersInTournament)
            {
                foreach (Player player2 in PlayersInTournament)
                {
                    if (player1 != player2)
                    {
                        if (player1.OpponentCount < PlayersInTournament.Count - 1 && player2.OpponentCount < PlayersInTournament.Count - 1)
                        {
                            player1.OpponentCount++;
                            player2.OpponentCount++;

                            Match oneMatch = new Match();
                            oneMatch.Player1 = player1;
                            oneMatch.Player2 = player2;
                            Matches.Add(oneMatch);
                        }
                    }
                }
            }
        }

        private void InitializePlayerStatistics()
        {
            foreach (Player player in PlayersInTournament)
            {
                State state = new State() { PlayerInGame = player };
                Statistics.Add(state);
            }
        }
        #endregion

        #region Persistence logic
        private BindingList<Player> GetAllPlayers()
        {
            var access = new DataAccess();

            return new BindingList<Player>(access.GetPlayers());
        }

        private BindingList<Team> GetAllTeams()
        {
            var access = new DataAccess();

            return new BindingList<Team>(access.GetTeams());
        }
        #endregion

    }
}
