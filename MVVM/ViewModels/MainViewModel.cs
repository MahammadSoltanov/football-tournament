using GameTournament.ExtraThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTournament.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        #region Fieds
        private Store _store = new Store();
        #endregion

        #region Properties        
        public HomeViewModel HomeVM { get; set; }
        public TournamentViewModel TournamentVM { get; set; }
        public RelayCommand GoToHomeCommand { get; set; }
        public RelayCommand GoToTournamentCommand { get; set; }
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set 
            { 
                _currentView = value;
                OnPropertyChanged();
            }
        }
        #endregion


        public MainViewModel()
        {
            HomeVM = new HomeViewModel(_store);
            TournamentVM = new TournamentViewModel(_store);
            CurrentView = HomeVM;
            GoToHomeCommand = new RelayCommand(GoToHome, CanGoToHome);
            GoToTournamentCommand = new RelayCommand(GoToTournament, CanGoToTournament);
        }

        private void GoToTournament()
        {
            if(CurrentView != TournamentVM)
                CurrentView = TournamentVM;
        }

        private bool CanGoToTournament()
        {
            return true;
        }

        private void GoToHome()
        {
            if(CurrentView != HomeVM)
                CurrentView = HomeVM;
        }

        private bool CanGoToHome()
        {
            return true;
        }
    }
}
