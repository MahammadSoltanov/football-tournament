using GameTournament.ExtraThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTournament.MVVM.Models
{
    public class Match : ObservableObject
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public int Goals1
        {
            get { return _goals1; }
            set 
            { 
                if (_goals1 != value)
                {
                    _goals1 = value;
                    OnPropertyChanged();
                }                
            }
        }
        private int _goals1;
        public int Goals2
        {
            get { return _goals2; }
            set
            {
                if (_goals2 != value)
                {
                    _goals2 = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _goals2;
        public bool Score1IsEditable
        {
            get { return _score1IsEditable; }
            set
            {
                if (_score1IsEditable != value)
                {
                    _score1IsEditable = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _score1IsEditable;
        public bool Score2IsEditable
        {
            get { return _score2IsEditable; }
            set
            {
                if (_score2IsEditable != value)
                {
                    _score2IsEditable = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _score2IsEditable;
    }
}
