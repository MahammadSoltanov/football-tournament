using GameTournament.Services;
using System.Collections.Generic;
using Xunit;

namespace GameTournament.Tests
{
    public class ResultsServiceTests
    {
        private readonly ResultsService resultsService;

        public ResultsServiceTests()
        {
            resultsService = new ResultsService();
        }

        #region TestData

        #region HasWinner
        public static IEnumerable<object[]> GetRepeatingList =>
        new List<object[]>
        {
             new object[] {new List<int>{ 7, 7, 7, 7, 7, 7, 7 } },
             new object[] {new List<int>{ 0, 0, 0, 0, 0, 0, 0 } },
             new object[] {new List<int>{ -1, -1, -1, -1, -1, -1, -1 } },
        };

        public static IEnumerable<object[]> GetDuplicateMaxList =>
        new List<object[]>
        {
            new object[] {new List<int>{ 1, 2, 3, 4, 5, 7, 7 } },
            new object[] {new List<int>{ 1, 2, 3, 4, 4, 5, 7, 7 } },
            new object[] {new List<int>{ 1, 2, 3, 6, 6, 4, 5 } },
        };

        public static IEnumerable<object[]> GetUniqueMaxList =>
        new List<object[]>
        {
            new object[] {new List<int>{ 1, 2, 3, 4, 5, 6, 7 } },
            new object[] {new List<int>{ 1, 2, 3, 4, 8, 5 } },
            new object[] {new List<int>{ 1, 2, 3, 6, 6, 9, 5 } },
        };
        #endregion

        #region DetermineWinnerIndex
        //object format is: points, pointsWithGoals, expectedWinnerIndex
        public static IEnumerable<object[]> StatisticsList =>
        new List<object[]>
        {
            new object[] {
                new List<int>{ 1, 2, 3, 4, 5,},
                new List<int>{ 1, 2, 3, 4, 5},
                4
            },
            new object[] {
                new List<int>{ 1, 2, 3, 4, 4,},
                new List<int>{ 1, 2, 3, 5, 4},
                3
            },
            new object[] {
                new List<int>{ 1, 2, 3, 4, 4,},
                new List<int>{ 1, 2, 3, 4, 4},
                -1
            },
        };
        #endregion
        #endregion


        [Theory]
        [MemberData(nameof(GetRepeatingList))]
        public void HasWinner_RepeatingValuesShouldReturnFalse(List<int> list)
        {
            var doesWinnerExist = resultsService.HasWinner(list);

            Assert.False(doesWinnerExist);
        }


        [Theory]
        [MemberData(nameof(GetDuplicateMaxList))]
        public void HasWinner_RepeatingMaximumDuplicateValuesShouldReturnFalse(List<int> list)
        {
            var doesWinnerExist = resultsService.HasWinner(list);

            Assert.False(doesWinnerExist);
        }

        [Theory]
        [MemberData(nameof(GetUniqueMaxList))]
        public void HasWinner_UniqueMaximumValueShouldReturnTrue(List<int> list)
        {
            var doesWinnerExist = resultsService.HasWinner(list);

            Assert.True(doesWinnerExist);
        }


        [Theory]
        [MemberData(nameof(StatisticsList))]
        public void DetermineWinnerIndex_RepeatingMaximumDuplicateValuesShouldReturnNegativeOne(List<int> points, List<int> pointsWithGoals, int expectedWinnerIndex)
        {
            var actualWinnerIndex = resultsService.DetermineWinnerIndex(points, pointsWithGoals);

            Assert.Equal(expectedWinnerIndex, actualWinnerIndex);
        }
    }
}
