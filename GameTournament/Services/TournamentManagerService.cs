using GameTournament.MVVM.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace GameTournament.Services
{
    public class TournamentManagerService
    {
        private int _tourNumber = 1;

        public int TourNumber
        {
            get { return _tourNumber; }
        }

        public BindingList<Player> Winners { get; set; }
        public BindingList<State> Statistics { get; set; }
        public BindingList<Match> Matches { get; set; }
        public BindingList<Player> PlayersInTournament { get; set; }

        public TournamentManagerService(BindingList<Player> winners, BindingList<State> statistics, BindingList<Match> matches, BindingList<Player> playersInTournament)
        {
            Winners = winners;
            Statistics = statistics;
            Matches = matches;
            PlayersInTournament = playersInTournament;
        }


        #region Calculations and updations
        public void IncrementTourNumber()
        {
            _tourNumber++;
        }
        public void CalculatePoints()
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


                else if (match.Goals1 < match.Goals2)
                {
                    foreach (State state in Statistics)
                    {

                        if (state.PlayerInGame.Id == match.Player2.Id)
                        {

                            state.Points += 3;
                        }
                    }
                }


                else if (match.Goals1 == match.Goals2)
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
        public bool UpdateScoresAndStatistics(int scorerId, int opponentId, int goals, bool scoreIsEditable)
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

        public void DeclareWinner(int winnerIndex)
        {
            MessageBox.Show($"The winner is {Statistics[winnerIndex].PlayerInGame.Name}");
            Statistics[winnerIndex].PlayerInGame.WonTournamentNumber = _tourNumber;
            Winners.Add(Statistics[winnerIndex].PlayerInGame);
        }

        public void NoWinner()
        {
            MessageBox.Show("There is no winner in this tournament");
            Player winner = new Player() { WonTournamentNumber = _tourNumber, Name = "No winner" };
            Winners.Add(winner);
        }

        public void GenerateRoundRobinMatches()
        {
            foreach (Player p1 in PlayersInTournament)
            {
                foreach (Player p2 in PlayersInTournament)
                {
                    if (p1.Id != p2.Id)
                    {
                        int totalPlayersInTournament = PlayersInTournament.Count - 1;
                        if (p1.OpponentCount < totalPlayersInTournament && p2.OpponentCount < totalPlayersInTournament)
                        {
                            p1.OpponentCount++;
                            p2.OpponentCount++;

                            Match oneMatch = new Match();
                            oneMatch.Player1 = p1;
                            oneMatch.Player2 = p2;
                            Matches.Add(oneMatch);
                        }
                    }
                }
            }
        }
        #endregion

        public List<int> GetPointsFromStatistics()
        {
            var points = Statistics.Select(s => s.Points).ToList();

            return points;
        }

        public List<int> GetPointsWithGoalsFromStatistics()
        {
            var pointsWithGoals = Statistics.Select(s => s.PointsWithGoals).ToList();

            return pointsWithGoals;
        }

    }
}
