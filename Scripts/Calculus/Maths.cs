﻿using System.Collections.Generic;
using System.Linq;

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
    /// The science symbol scores in the recursive function
    /// <see cref="CalculateScienceScore"/>
    /// </summary>
    private static readonly Dictionary<EScienceSymbol, int> s_scienceSymbolScoresTemp = new();

    /// <summary>
    /// The value of a group of three different science symbol
    /// <see cref="CalculateScienceScore"/>
    /// </summary>
    public static int GroupValue { get; set; } = 7;

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

        // Group bonus: X per symbol group
        int[] cardsCount = {
            scienceCards[EScienceSymbol.Compass],
            scienceCards[EScienceSymbol.Tablet],
            scienceCards[EScienceSymbol.Cogwheel],
        };
        result += GroupValue * cardsCount.Min();

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
}
