using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevenScience;

public static class Maths
{
    #region Members

    /// <summary>
    /// The results of the recursive function
    /// <see cref="CalculateScienceScore"/>
    /// </summary>
    private static readonly List<int> s_resultsTemp = new();
    
    /// <summary>
    /// The science symbol scores in the recursive function
    /// <see cref="CalculateScienceScore"/>
    /// </summary>
    private static readonly Dictionary<EScienceSymbol, int> s_scienceSymbolScoresTemp = new();

    #endregion

    #region Public Methods

    /// <summary>
    /// Calculate the score without wild cards
    /// </summary>
    /// <param name="scienceCards"></param>
    /// <returns></returns>
    public static int CalculateScienceScoreNoWild(IReadOnlyDictionary<EScienceSymbol, int> scienceCards)
    {
        s_scienceSymbolScoresTemp.Clear();

        // Calculate the score for each science symbol
        foreach (KeyValuePair<EScienceSymbol, int> item in scienceCards.Where(item => item.Key != EScienceSymbol.Wild))
        {
            s_scienceSymbolScoresTemp[item.Key] = item.Value * item.Value;
        }

        int result = s_scienceSymbolScoresTemp.Sum(item => item.Value);

        // Group bonus: 7 per symbol group
        int[] cardsCount = new int[]
        {
            scienceCards[EScienceSymbol.Compass],
            scienceCards[EScienceSymbol.Tablet],
            scienceCards[EScienceSymbol.Cogwheel],
        };
        result += 7 * cardsCount.Min();

        return result;
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
            return -1;
        }

        for (int x = 0; x < (int)EScienceSymbol.Count - 1; x++)
        {
            // Create a new dictionary as a copy of the original one
            Dictionary<EScienceSymbol, int> supposedScienceCardsTemp = new(scienceSymbolScores);
            supposedScienceCardsTemp[(EScienceSymbol)x]++;

            s_resultsTemp.Add(CalculateScienceScoreNoWild(supposedScienceCardsTemp));
            CalculateScienceScore(supposedScienceCardsTemp, wildCount - 1);
        }

        // Return the highest score from the results list
        return s_resultsTemp.Max();
    }

    public static void Reset()
    {
        s_resultsTemp.Clear();
    }

    #endregion
}