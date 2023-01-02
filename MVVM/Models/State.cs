using GameTournament.ExtraThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTournament.MVVM.Models
{
    public class State : ObservableObject
    {
        public Player PlayerInGame { get; set; }

        private int _averageGoals;
        public int AverageGoals
        {
            get { return _averageGoals; }
            set
            {
                _averageGoals = value;
                OnPropertyChanged();
            }
        }

        private int _points;
        public int Points
        {
            get { return _points; }
            set
            {
                _points = value;
                OnPropertyChanged();
            }
        }
    }
}