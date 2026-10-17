using Xunit;

namespace SevenScience.Tests;

public class AganiceTests : MathsTestBase
{
    [Fact]
    public void DoesNothingWhenOff()
        => Assert.Equal(5, Score(2, 1, 0));

    [Theory]
    [InlineData(2, 1, 0, 10)]  // a compass becomes a cogwheel: 1 / 1 / 1
    [InlineData(2, 2, 0, 13)]  // 2 / 1 / 1
    [InlineData(1, 1, 0, 4)]   // 2 / 0 / 0, better than keeping 1 / 1 / 0 (2)
    [InlineData(3, 0, 0, 9)]   // replacing is optional: 2 / 1 / 0 would only be 5
    [InlineData(5, 1, 1, 37)]  // the tablet becomes a compass: 6 / 0 / 1 beats keeping 5 / 1 / 1 (34)
    public void ReplacesTheSymbolThatGivesTheMostPoints(int compass, int tablet, int cogwheel, int expected)
    {
        Maths.AganiceEnabled = true;

        Assert.Equal(expected, Score(compass, tablet, cogwheel));
    }

    [Fact]
    public void NeedsASymbolToReplace()
    {
        Maths.AganiceEnabled = true;

        Assert.Equal(0, Score(0, 0, 0));
    }

    [Fact]
    public void WorksWithAristote()
    {
        UseAristote();
        Maths.AganiceEnabled = true;

        Assert.Equal(16, Score(2, 2, 0));  // 2 / 1 / 1: 6 + one trio (10)
    }

    [Fact]
    public void WildCardsAreNotMadeBetterByAganice()
    {
        Maths.AganiceEnabled = true;

        Assert.Equal(1, Score(0, 0, 0, 1));
        Assert.Equal(10, Score(0, 0, 0, 3));
    }

    [Fact]
    public void WorksTogetherWithGroupPlus()
    {
        Maths.AganiceEnabled = true;
        Maths.GroupPlusCount = 1;

        // The tablet becomes a compass (3 / 0 / 0), then GroupPlus adds a compass: 4 / 0 / 0 = 16
        Assert.Equal(16, Score(2, 1, 0));
    }

    [Fact]
    public void CannotReplaceTheSymbolsGainedByPidgeon()
    {
        Maths.AganiceEnabled = true;
        SetPidgeon(2, 2, 0, 0);

        // 1 / 0 / 0 owned + 2 / 0 / 0 gained: only the owned compass can change, the best is keeping everything (3 / 0 / 0 = 9)
        Assert.Equal(9, Score(1, 0, 0));
    }
}
