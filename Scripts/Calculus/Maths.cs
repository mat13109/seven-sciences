using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SevenScience;

public static class Maths
{
    #region Members

    /// <summary>
    /// The results of the recursive function
    /// <see cref="CalculateScienceScore"/>
    /// </summary>
    private static int s_resultsTemp;

    /// <summary>
    /// The value of a group of three different science symbol
    /// <see cref="CalculateScienceScore"/>
    /// </summary>
    public static int GroupValue { get; set; } = 7;

    /// <summary>
    /// How many times the GroupPlus power is stacked: each one adds a science symbol
    /// of the type the player owns the most of
    /// </summary>
    public static int GroupPlusCount { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Calculate the score without wild cards
    /// </summary>
    /// <param name="scienceCards"></param>
    /// <returns></returns>
    public static int CalculateScienceScoreNoWild(IReadOnlyDictionary<EScienceSymbol, int> scienceCards)
    {
        int[] cardsCount = {
            scienceCards[EScienceSymbol.Compass],
            scienceCards[EScienceSymbol.Tablet],
            scienceCards[EScienceSymbol.Cogwheel],
        };

        int mostOwned = cardsCount.Max();

        // Without any symbol there is no "most owned" type to add to
        if (GroupPlusCount == 0 || mostOwned == 0)
        {
            return ScoreOf(cardsCount);
        }

        // The stacked symbols all go to the same type: once the first one is added, that type is the only one in the lead.
        // On a tie the player picks the best type among the tied ones
        int best = 0;

        for (int i = 0; i < cardsCount.Length; i++)
        {
            if (cardsCount[i] != mostOwned)
            {
                continue;
            }

            cardsCount[i] += GroupPlusCount;
            best = Mathf.Max(best, ScoreOf(cardsCount));
            cardsCount[i] -= GroupPlusCount;
        }

        return best;
    }

    /// <summary>
    /// Recursive function to calculate the score with wild cards
    /// </summary>
    /// <param name="scienceSymbolScores"></param>
    /// <param name="wildCount"></param>
    /// <returns></returns>
    public static int CalculateScienceScore(Dictionary<EScienceSymbol, int> scienceSymbolScores, int wildCount)
    {
        if (wildCount == 0)
        {
            return CalculateScienceScoreNoWild(scienceSymbolScores);
        }

        for (int x = 0; x < (int)EScienceSymbol.Count - 1; x++)
        {
            // Create a new dictionary as a copy of the original one
            Dictionary<EScienceSymbol, int> supposedScienceCardsTemp = new(scienceSymbolScores);
            supposedScienceCardsTemp[(EScienceSymbol)x]++;

            int temp = CalculateScienceScoreNoWild(supposedScienceCardsTemp);
            if (temp > s_resultsTemp)
            {
                s_resultsTemp = temp;
            }

            CalculateScienceScore(supposedScienceCardsTemp, wildCount - 1);
        }

        // Return the highest score from the results list
        return s_resultsTemp;
    }

    public static void Reset() => s_resultsTemp = 0;

    #endregion

    #region Private Methods

    /// <summary>
    /// Score of the given Compass, Tablet and Cogwheel counts:
    /// the square of each count plus the value of each complete group of three different symbols
    /// </summary>
    private static int ScoreOf(int[] cardsCount) => cardsCount.Sum(count => count * count) + (GroupValue * cardsCount.Min());

    #endregion
}
