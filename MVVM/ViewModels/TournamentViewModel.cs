using GameTournament.Helpers;
using GameTournament.MVVM.Models;
using GameTournament.Services;
using System.ComponentModel;
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
        private readonly ResultsService _resultsService;
        private readonly TournamentManagerService _tournamentManagerService;
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
            AllPlayers = Mapper.GetAllPlayers();
            AllTeams = Mapper.GetAllTeams();

            Statistics = new BindingList<State>();
            Winners = new BindingList<Player>();
            AddToTournamentCommand = new RelayCommand(AddToTournament, CanAddToTournament);
            StartTournamentCommand = new RelayCommand(StartTournament, CanStartTournament);
            EndTournamentCommand = new RelayCommand(EndTournament, CanEndTournament);
            PlayAgainCommand = new RelayCommand(PlayAgain);
            PlayersInTournament = new BindingList<Player>();
            Matches = new BindingList<Match>();

            _resultsService = new ResultsService();
            _tournamentManagerService = new TournamentManagerService(Winners, Statistics, Matches, PlayersInTournament);
            _storeTVM.PlayerAdded += _storeTVM_PlayerAdded;
            _storeTVM.TeamAdded += _storeTVM_TeamAdded;
        }

        #region Main Commands
        private void PlayAgain()
        {
            SetDefaultVisibilities();

            //Changing End button's activity
            _canEnd = true;
            EndTournamentCommand.RaiseCanExecuteChanged();
            ClearTournamentInformation();

            AllPlayers = Mapper.GetAllPlayers();
            AllTeams = Mapper.GetAllTeams();

            AddStackPanelVisibility = Visibility.Visible;
        }

        private void EndTournament()
        {
            _tournamentManagerService.IncrementTourNumber();
            _tournamentManagerService.CalculatePoints();

            var points = _tournamentManagerService.GetPointsFromStatistics();
            var pointsWithGoals = _tournamentManagerService.GetPointsWithGoalsFromStatistics();

            var winnerIndex = _resultsService.DetermineWinnerIndex(points, pointsWithGoals);

            if (winnerIndex != -1)
            {
                _tournamentManagerService.DeclareWinner(winnerIndex);
            }
            else
            {
                _tournamentManagerService.NoWinner();
            }

            _canEnd = false;
            PlayAgainButtonVisibility = Visibility.Visible;
            WinnersStackPanelVisibility = Visibility.Visible;
            EndTournamentCommand.RaiseCanExecuteChanged();
        }

        private void StartTournament()
        {
            AddStackPanelVisibility = Visibility.Hidden;
            StatisticsTableVisibility = Visibility.Visible;

            _tournamentManagerService.GenerateRoundRobinMatches();
            Matches.ListChanged += Matches_ListChanged;
            InitializePlayerStatistics();

            MatchesListBoxVisibility = Visibility.Visible;
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
        #endregion


        #region Helper methods
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



        private void InitializePlayerStatistics()
        {
            foreach (Player player in PlayersInTournament)
            {
                State state = new State() { PlayerInGame = player };
                Statistics.Add(state);
            }
        }


        #endregion

        #region EventHandling
        private void Matches_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                int changedIndex = e.NewIndex;

                var changedMatch = Matches[changedIndex];
                var player1 = changedMatch.Player1;
                var player2 = changedMatch.Player2;

                bool result1 = _tournamentManagerService.UpdateScoresAndStatistics(player1.Id, player2.Id, changedMatch.Goals1, changedMatch.Score1IsEditable);
                bool result2 = _tournamentManagerService.UpdateScoresAndStatistics(player2.Id, player1.Id, changedMatch.Goals2, changedMatch.Score2IsEditable);

                changedMatch.Score1IsEditable = result1;
                changedMatch.Score2IsEditable = result2;
            }
        }

        private void _storeTVM_TeamAdded(Team team)
        {
            AllTeams.Add(team);
        }

        private void _storeTVM_PlayerAdded(Player player)
        {
            AllPlayers.Add(player);
        }
        #endregion

        private bool CanEndTournament()
        {
            return _canEnd;
        }

        private bool CanAddToTournament()
        {
            if (SelectedPlayer == null || SelectedTeam == null)
            {
                return false;
            }

            return true;
        }

        private bool CanStartTournament()
        {
            if (PlayersInTournament.Count < 2)
            {
                return false;
            }

            return true;
        }
    }
}
