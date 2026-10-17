using System;
using System.Collections.Generic;
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

    private static readonly int[] s_noSymbols = new int[3];

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

    /// <summary>
    /// How many times the Pidgeon power is stacked: each one gains a symbol from a green card of a neighbor
    /// </summary>
    public static int PidgeonCount { get; set; }

    /// <summary>
    /// For each of Compass, Tablet and Cogwheel, the number of green cards in the neighbors' cities that carry it.
    /// A card can only be used once, so this caps how many times Pidgeon can gain each symbol
    /// </summary>
    public static int[] PidgeonAvailableCards { get; } = new int[3];

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

        // More symbols never lower the score, so Pidgeon always gains as many as it can
        int picks = Math.Min(PidgeonCount, PidgeonAvailableCards.Sum());
        int best = 0;

        // Try every way to share the picks between the three symbols
        for (int compass = 0; compass <= Math.Min(picks, PidgeonAvailableCards[0]); compass++)
        {
            for (int tablet = 0; tablet <= Math.Min(picks - compass, PidgeonAvailableCards[1]); tablet++)
            {
                int cogwheel = picks - compass - tablet;

                if (cogwheel > PidgeonAvailableCards[2])
                {
                    continue;
                }

                int[] pidgeon = { compass, tablet, cogwheel };

                // The end of game powers can be resolved in either order, keep the best one
                best = Math.Max(best, ScoreWithGroupPlus(AddCounts(cardsCount, pidgeon), s_noSymbols));
                best = Math.Max(best, ScoreWithGroupPlus(cardsCount, pidgeon));
            }
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
    private static int[] AddCounts(int[] first, int[] second) => new[] { first[0] + second[0], first[1] + second[1], first[2] + second[2] };

    /// <summary>
    /// Score once GroupPlus has added its symbols to the type owned the most in <paramref name="cardsCount"/>,
    /// then <paramref name="gainedAfter"/> is added on top (the symbols gained by Pidgeon after GroupPlus)
    /// </summary>
    private static int ScoreWithGroupPlus(int[] cardsCount, int[] gainedAfter)
    {
        int mostOwned = cardsCount.Max();

        // Without any symbol there is no "most owned" type to add to
        if (GroupPlusCount == 0 || mostOwned == 0)
        {
            return ScoreOf(AddCounts(cardsCount, gainedAfter));
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

            int[] counts = AddCounts(cardsCount, gainedAfter);
            counts[i] += GroupPlusCount;
            best = Math.Max(best, ScoreOf(counts));
        }

        return best;
    }

    private static int ScoreOf(int[] cardsCount) => cardsCount.Sum(count => count * count) + (GroupValue * cardsCount.Min());

    #endregion
}
