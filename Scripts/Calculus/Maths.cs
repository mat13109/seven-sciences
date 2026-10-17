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

    /// <summary>
    /// Whether the Aganice power is on: the player may replace one symbol of their city by the symbol of their choice
    /// </summary>
    public static bool AganiceEnabled { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Calculate the score without wild cards
    /// </summary>
    /// <param name="scienceCards"></param>
    /// <returns></returns>
    public static int CalculateScienceScoreNoWild(IReadOnlyDictionary<EScienceSymbol, int> scienceCards)
    {
        int[] ownedSymbols = {
            scienceCards[EScienceSymbol.Compass],
            scienceCards[EScienceSymbol.Tablet],
            scienceCards[EScienceSymbol.Cogwheel],
        };

        EEndGamePower powers = EEndGamePower.None;

        if (AganiceEnabled)
        {
            powers |= EEndGamePower.Aganice;
        }

        if (PidgeonCount > 0 && PidgeonAvailableCards.Sum() > 0)
        {
            powers |= EEndGamePower.Pidgeon;
        }

        if (GroupPlusCount > 0)
        {
            powers |= EEndGamePower.GroupPlus;
        }

        return BestScore(ownedSymbols, new int[3], powers);
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
    /// The end of game powers that change the symbols
    /// </summary>
    [Flags]
    private enum EEndGamePower
    {
        None = 0,
        Aganice = 1,
        Pidgeon = 2,
        GroupPlus = 4,
    }

    /// <summary>
    /// Best score reachable by resolving the <paramref name="powersLeft"/> in any order and with any choice
    /// </summary>
    /// <param name="ownedSymbols">Compass, Tablet and Cogwheel in the city of the player (cards and wild cards)</param>
    /// <param name="gainedSymbols">Compass, Tablet and Cogwheel gained by Pidgeon and GroupPlus.
    /// They count for the score and for the type owned the most, but are not part of the city so Aganice cannot replace them</param>
    /// <param name="powersLeft">The powers still to resolve</param>
    private static int BestScore(int[] ownedSymbols, int[] gainedSymbols, EEndGamePower powersLeft)
    {
        if (powersLeft == EEndGamePower.None)
        {
            return ScoreOf(AddCounts(ownedSymbols, gainedSymbols));
        }

        int best = 0;

        if (powersLeft.HasFlag(EEndGamePower.Aganice))
        {
            EEndGamePower others = powersLeft & ~EEndGamePower.Aganice;

            // The replacement is optional
            best = Math.Max(best, BestScore(ownedSymbols, gainedSymbols, others));

            for (int removed = 0; removed < ownedSymbols.Length; removed++)
            {
                if (ownedSymbols[removed] == 0)
                {
                    continue;
                }

                for (int added = 0; added < ownedSymbols.Length; added++)
                {
                    if (added == removed)
                    {
                        continue;
                    }

                    int[] replaced = (int[])ownedSymbols.Clone();
                    replaced[removed]--;
                    replaced[added]++;
                    best = Math.Max(best, BestScore(replaced, gainedSymbols, others));
                }
            }
        }

        if (powersLeft.HasFlag(EEndGamePower.Pidgeon))
        {
            EEndGamePower others = powersLeft & ~EEndGamePower.Pidgeon;

            // More symbols never lower the score, so Pidgeon always gains as many as it can
            int picks = Math.Min(PidgeonCount, PidgeonAvailableCards.Sum());

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
                    best = Math.Max(best, BestScore(ownedSymbols, AddCounts(gainedSymbols, pidgeon), others));
                }
            }
        }

        if (powersLeft.HasFlag(EEndGamePower.GroupPlus))
        {
            EEndGamePower others = powersLeft & ~EEndGamePower.GroupPlus;
            int[] symbols = AddCounts(ownedSymbols, gainedSymbols);
            int mostOwned = symbols.Max();

            // Without any symbol there is no "most owned" type to add to
            if (mostOwned == 0)
            {
                best = Math.Max(best, BestScore(ownedSymbols, gainedSymbols, others));
            }

            // The stacked symbols all go to the same type: once the first one is added, that type is the only one in the lead.
            // On a tie the player picks the best type among the tied ones
            for (int i = 0; mostOwned > 0 && i < symbols.Length; i++)
            {
                if (symbols[i] != mostOwned)
                {
                    continue;
                }

                int[] gained = (int[])gainedSymbols.Clone();
                gained[i] += GroupPlusCount;
                best = Math.Max(best, BestScore(ownedSymbols, gained, others));
            }
        }

        return best;
    }

    private static int[] AddCounts(int[] first, int[] second) => new[] { first[0] + second[0], first[1] + second[1], first[2] + second[2] };

    /// <summary>
    /// Score of the given Compass, Tablet and Cogwheel counts:
    /// the square of each count plus the value of each complete group of three different symbols
    /// </summary>
    private static int ScoreOf(int[] cardsCount) => cardsCount.Sum(count => count * count) + (GroupValue * cardsCount.Min());

    #endregion
}
