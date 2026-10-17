using Xunit;

namespace SevenScience.Tests;

/// <summary>
/// Compares <see cref="Maths"/> with <see cref="ReferenceScorer"/> on a lot of combinations of cards and powers
/// </summary>
public class BruteForceTests : MathsTestBase
{
    private static readonly int[] s_groupValues = { 7, 10 };

    [Fact]
    public void MatchesTheReferenceOnEverySmallCombination()
    {
        List<string> mismatches = new();

        foreach (int groupValue in s_groupValues)
        for (int compass = 0; compass <= 3; compass++)
        for (int tablet = 0; tablet <= 3; tablet++)
        for (int cogwheel = 0; cogwheel <= 3; cogwheel++)
        for (int wild = 0; wild <= 2; wild++)
        for (int groupPlus = 0; groupPlus <= 2; groupPlus++)
        for (int stacks = 0; stacks <= 2; stacks++)
        for (int cardsC = 0; cardsC <= 2; cardsC++)
        for (int cardsT = 0; cardsT <= 2; cardsT++)
        for (int cardsG = 0; cardsG <= 2; cardsG++)
        {
            Check(mismatches, groupValue, compass, tablet, cogwheel, wild, groupPlus, stacks, cardsC, cardsT, cardsG);
        }

        Assert.True(mismatches.Count == 0, $"{mismatches.Count} mismatches, first ones:\n{string.Join("\n", mismatches.Take(5))}");
    }

    [Fact]
    public void MatchesTheReferenceOnRandomRealisticCombinations()
    {
        // Fixed seed: a failure can be reproduced
        Random random = new(42);
        List<string> mismatches = new();

        for (int i = 0; i < 3000; i++)
        {
            Check(
                mismatches,
                s_groupValues[random.Next(s_groupValues.Length)],
                random.Next(0, 11),
                random.Next(0, 11),
                random.Next(0, 11),
                random.Next(0, 4),
                random.Next(0, 6),
                random.Next(0, 4),
                random.Next(0, 11),
                random.Next(0, 11),
                random.Next(0, 11));
        }

        Assert.True(mismatches.Count == 0, $"{mismatches.Count} mismatches, first ones:\n{string.Join("\n", mismatches.Take(5))}");
    }

    private static void Check(List<string> mismatches, int groupValue, int compass, int tablet, int cogwheel, int wild, int groupPlus, int stacks, int cardsC, int cardsT, int cardsG)
    {
        Maths.GroupValue = groupValue;
        Maths.GroupPlusCount = groupPlus;
        SetPidgeon(stacks, cardsC, cardsT, cardsG);

        int actual = Score(compass, tablet, cogwheel, wild);
        int expected = ReferenceScorer.Score(compass, tablet, cogwheel, wild, groupValue, groupPlus, stacks, new[] { cardsC, cardsT, cardsG });

        if (actual != expected)
        {
            mismatches.Add($"cards {compass}/{tablet}/{cogwheel} + {wild} wild, group value {groupValue}, GroupPlus {groupPlus}, "
                + $"Pidgeon {stacks} stacks on {cardsC}/{cardsT}/{cardsG} cards: got {actual}, expected {expected}");
        }
    }
}
