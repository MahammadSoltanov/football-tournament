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
        private BindingList<Player> _reservePlayers = new BindingList<Player>();
        private BindingList<Team> _reserveTeams = new BindingList<Team>();
        #endregion

        #region ViewModelProperties        
        public BindingList<Player> Winners { get; set; }
        public BindingList<State> Statistics { get; set; }
        public BindingList<Player> AllPlayers { get; set; }
        public BindingList<Match> Matches { get; set; }
        public BindingList<Team> AllTeams { get; set; }
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
            DataAccess access = new DataAccess();
            AllPlayers = new BindingList<Player>(access.GetPlayers());
            AllTeams = new BindingList<Team>(access.GetTeams());

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
            //Setting visibilities to default ones
            StatisticsTableVisibility = Visibility.Hidden;
            PlayAgainButtonVisibility = Visibility.Hidden;
            MatchesListBoxVisibility = Visibility.Hidden;

            //Changing End button's activity
            _canEnd = true;
            EndTournamentCommand.RaiseCanExecuteChanged();

            //Clearing tournament and setting start button's activity
            PlayersInTournament.Clear();
            StartTournamentCommand.RaiseCanExecuteChanged();
            Statistics.Clear();
            Matches.Clear();

            //Filling comboboxes with values that were deleted before
            foreach (Player player in _reservePlayers)
            {
                player.OpponentCount = 0;
                AllPlayers.Add(player);
            }

            foreach (Team team in _reserveTeams)
                AllTeams.Add(team);

            //Clearing reserves so it will not stack data for other play again loop
            _reservePlayers.Clear();
            _reserveTeams.Clear();

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
            foreach (Match match in Matches)
            {
                if (match.Goals1 > match.Goals2)
                    foreach (State state in Statistics)
                        if (state.PlayerInGame.Id == match.Player1.Id)
                            state.Points += 3;

                if (match.Goals1 < match.Goals2)
                    foreach (State state in Statistics)
                        if (state.PlayerInGame.Id == match.Player2.Id)
                            state.Points += 3;


                if (match.Goals1 == match.Goals2)
                {
                    foreach (State state in Statistics)
                    {
                        if (state.PlayerInGame.Id == match.Player1.Id)
                            state.Points += 1;

                        if (state.PlayerInGame.Id == match.Player2.Id)
                            state.Points += 1;
                    }
                }
            }

            List<int> PointsList = new List<int>();
            List<int> SumList = new List<int>();

            foreach (State state in Statistics)
            {
                PointsList.Add(state.Points);
                SumList.Add(state.Points + state.AverageGoals);
            }

            if (PointsList.Distinct().Count() == 1)
            {
                if (SumList.Distinct().Count() == 1)
                {
                    MessageBox.Show("There is no winner in this tournament");
                    Player winner = new Player() { WonTournamentNumber = _tourNumber, Name = "Nobody won" };
                    Winners.Add(winner);
                }

                else
                {
                    int winnerIndex = SumList.IndexOf(SumList.Max());
                    if (SumList.Distinct().Count() != SumList.Count)
                    {
                        bool maxExist2 = false;
                        var dupBase2 = SumList.GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key);
                        List<int> dupSums = new List<int>(dupBase2);

                        foreach (int dup in dupBase2)
                            if (dup == SumList.Max())
                                maxExist2 = true;


                        if (maxExist2)
                        {
                            MessageBox.Show("There is no winner in this tournament");
                            Player winner = new Player() { WonTournamentNumber = _tourNumber, Name = "Nobody won" };
                            Winners.Add(winner);
                        }

                        else
                        {
                            MessageBox.Show($"The winner is {Statistics[winnerIndex].PlayerInGame.Name}");
                            Statistics[winnerIndex].PlayerInGame.WonTournamentNumber = _tourNumber;
                            Winners.Add(Statistics[winnerIndex].PlayerInGame);
                        }
                    }

                    else
                    {
                        MessageBox.Show($"The winner is {Statistics[winnerIndex].PlayerInGame.Name}");
                        Statistics[winnerIndex].PlayerInGame.WonTournamentNumber = _tourNumber;
                        Winners.Add(Statistics[winnerIndex].PlayerInGame);
                    }
                }
            }

            else
            {
                if (PointsList.Distinct().Count() != PointsList.Count)
                {
                    bool maxExist = false;
                    var dupBase = PointsList.GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key);
                    List<int> dupPoints = new List<int>(dupBase);

                    foreach (int dup in dupPoints)
                        if (dup == PointsList.Max())
                            maxExist = true;

                    if (maxExist)
                    {
                        if (SumList.Distinct().Count() == 1)
                        {
                            MessageBox.Show("There is no winner in this tournament");
                            Player winner = new Player() { WonTournamentNumber = _tourNumber, Name = "Nobody won" };
                            Winners.Add(winner);
                        }

                        else
                        {
                            int winnerIndex = SumList.IndexOf(SumList.Max());
                            if (SumList.Distinct().Count() != SumList.Count)
                            {
                                bool maxExist2 = false;
                                var dupBase2 = SumList.GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key);
                                List<int> dupSums = new List<int>(dupBase2);

                                foreach (int dup in dupBase2)
                                    if (dup == SumList.Max())
                                        maxExist2 = true;

                                if (maxExist2)
                                {
                                    MessageBox.Show("There is no winner in this tournament");
                                    Player winner = new Player() { WonTournamentNumber = _tourNumber, Name = "Nobody won" };
                                    Winners.Add(winner);
                                }

                                else
                                {
                                    MessageBox.Show($"The winner is {Statistics[winnerIndex].PlayerInGame.Name}");
                                    Statistics[winnerIndex].PlayerInGame.WonTournamentNumber = _tourNumber;
                                    Winners.Add(Statistics[winnerIndex].PlayerInGame);
                                }
                            }

                            else
                            {
                                MessageBox.Show($"The winner is {Statistics[winnerIndex].PlayerInGame.Name}");
                                Statistics[winnerIndex].PlayerInGame.WonTournamentNumber = _tourNumber;
                                Winners.Add(Statistics[winnerIndex].PlayerInGame);
                            }
                        }
                    }

                    else
                    {
                        MessageBox.Show($"The winner is {Statistics[PointsList.IndexOf(PointsList.Max())].PlayerInGame.Name}");
                        Statistics[PointsList.IndexOf(PointsList.Max())].PlayerInGame.WonTournamentNumber = _tourNumber;
                        Winners.Add(Statistics[PointsList.IndexOf(PointsList.Max())].PlayerInGame);
                    }
                }

                else
                {
                    MessageBox.Show($"The winner is {Statistics[PointsList.IndexOf(PointsList.Max())].PlayerInGame.Name}");
                    Statistics[PointsList.IndexOf(PointsList.Max())].PlayerInGame.WonTournamentNumber = _tourNumber;
                    Winners.Add(Statistics[PointsList.IndexOf(PointsList.Max())].PlayerInGame);
                }
            }


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

            foreach (Player player1 in PlayersInTournament)
            {
                foreach (Player player2 in PlayersInTournament)
                {
                    if (player1 != player2)
                    {
                        if (player1.OpponentCount < PlayersInTournament.Count - 1 &&
                            player2.OpponentCount < PlayersInTournament.Count - 1)
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

            Matches.ListChanged += Matches_ListChanged;

            foreach (Player player in PlayersInTournament)
            {
                State state = new State() { PlayerInGame = player };
                Statistics.Add(state);
            }

            MatchesListBoxVisibility = Visibility.Visible;
        }

        private void Matches_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                int changedIndex = e.NewIndex;

                if (Matches[changedIndex].Goals1 > 0 && Matches[changedIndex].Score1IsEditable == false)
                {
                    Matches[changedIndex].Score1IsEditable = true;
                    foreach (State state in Statistics)
                    {
                        if (state.PlayerInGame.Id == Matches[changedIndex].Player2.Id)
                            state.AverageGoals -= Matches[changedIndex].Goals1;

                        if (state.PlayerInGame.Id == Matches[changedIndex].Player1.Id)
                            state.AverageGoals += Matches[changedIndex].Goals1;
                    }
                }

                if (Matches[changedIndex].Goals2 > 0 && Matches[changedIndex].Score2IsEditable == false)
                {
                    foreach (State state in Statistics)
                    {
                        if (state.PlayerInGame.Id == Matches[changedIndex].Player1.Id)
                            state.AverageGoals -= Matches[changedIndex].Goals2;

                        if (state.PlayerInGame.Id == Matches[changedIndex].Player2.Id)
                            state.AverageGoals += Matches[changedIndex].Goals2;
                    }
                    Matches[changedIndex].Score2IsEditable = true;
                }
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
            _reservePlayers.Add(SelectedPlayer);
            _reserveTeams.Add(SelectedTeam);
            AllPlayers.Remove(SelectedPlayer);
            AllTeams.Remove(SelectedTeam);
            if (PlayersInTournament.Count == 2)
                StartTournamentCommand.RaiseCanExecuteChanged();
        }

        private bool CanAddToTournament()
        {
            if (SelectedPlayer == null || SelectedTeam == null)
                return false;

            else return true;
        }
        #endregion
    }
}
