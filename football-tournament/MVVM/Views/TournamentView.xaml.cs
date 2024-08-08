using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameTournament.MVVM.Views
{
    /// <summary>
    /// Interaction logic for TournamentView.xaml
    /// </summary>
    public partial class TournamentView : UserControl
    {
        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        public TournamentView()
        {
            InitializeComponent();
        }

        private void Score1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void Score2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void Score2_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            txt.SelectAll();
        }

        private void Score1_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            txt.SelectAll();
        }

        private void Score2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = true;
            }

        }

        private void Score1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = true;
            }
        }
    }
}
