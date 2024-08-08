using System.Collections.Generic;
using System.Linq;

namespace GameTournament.Services
{
    public class ResultsService
    {
        //Checks if there is a winnner based on list of numeric values
        public bool HasWinner(List<int> list)
        {
            int maxValue = list.Max();
            bool hasDuplicateMax = list.Count(x => x == maxValue) > 1;

            if (hasDuplicateMax)
            {
                return false;
            }

            return true;
        }

        public int DetermineWinnerIndex(List<int> pointsList, List<int> pointsWithGoalsList)
        {
            int winnerIndex = -1;

            if (HasWinner(pointsList))
            {
                winnerIndex = pointsList.IndexOf(pointsList.Max());

            }
            else if (HasWinner(pointsWithGoalsList))
            {
                winnerIndex = pointsWithGoalsList.IndexOf(pointsWithGoalsList.Max());
            }

            return winnerIndex;
        }


    }
}
