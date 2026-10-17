using Xunit;

namespace SevenScience.Tests;

public class PidgeonTests : MathsTestBase
{
    [Fact]
    public void NoStackGainsNothingEvenWithCardsAvailable()
    {
        SetPidgeon(0, 5, 5, 5);

        Assert.Equal(1, Score(1, 0, 0));
    }

    [Fact]
    public void NoCardsAvailableGainsNothing()
    {
        SetPidgeon(3, 0, 0, 0);

        Assert.Equal(1, Score(1, 0, 0));
    }

    [Fact]
    public void GainsASymbolFromAnAvailableCard()
    {
        SetPidgeon(1, 1, 0, 0);

        Assert.Equal(4, Score(1, 0, 0));  // 2 / 0 / 0
    }

    [Fact]
    public void AGivenCardCanOnlyBeUsedOnce()
    {
        SetPidgeon(3, 1, 0, 0);

        Assert.Equal(4, Score(1, 0, 0));  // a single compass card, not 4 / 0 / 0
    }

    [Fact]
    public void CannotGainMoreSymbolsThanStacks()
    {
        SetPidgeon(1, 10, 10, 10);

        Assert.Equal(1, Score(0, 0, 0));
    }

    [Fact]
    public void ChoosesTheBestSymbol()
    {
        SetPidgeon(1, 1, 1, 1);

        Assert.Equal(9, Score(2, 0, 0));  // compass: 3 / 0 / 0, better than 2 / 1 / 0
    }

    [Fact]
    public void ChoosesTheSymbolThatCompletesATrio()
    {
        SetPidgeon(1, 0, 0, 1);

        Assert.Equal(10, Score(1, 1, 0));
    }

    [Fact]
    public void SpreadsTheSymbolsWhenTriosAreWorthIt()
    {
        SetPidgeon(3, 1, 1, 1);

        Assert.Equal(10, Score(0, 0, 0));  // 1 / 1 / 1
    }

    [Fact]
    public void ConcentratesTheSymbolsWhenSquaresAreWorthIt()
    {
        SetPidgeon(2, 2, 2, 2);

        Assert.Equal(4, Score(0, 0, 0));  // 2 / 0 / 0 beats 1 / 1 / 0
    }

    [Fact]
    public void CombinesWithWildCards()
    {
        SetPidgeon(2, 0, 1, 1);

        Assert.Equal(10, Score(0, 0, 0, 1));  // wild on the compass, then tablet and cogwheel
    }

    [Fact]
    public void GroupPlusCanBeResolvedBeforePidgeon()
    {
        Maths.GroupPlusCount = 1;
        SetPidgeon(3, 2, 0, 1);

        // GroupPlus first: 0 / 2 / 1, then Pidgeon adds two compasses and a cogwheel for 2 / 2 / 2 = 26.
        // The other order only gets 21 (3 / 1 / 2)
        Assert.Equal(26, Score(0, 1, 1));
    }

    [Fact]
    public void GroupPlusCanBeResolvedAfterPidgeon()
    {
        Maths.GroupPlusCount = 1;
        SetPidgeon(2, 0, 0, 2);

        // Pidgeon first: 0 / 0 / 2, then GroupPlus makes 0 / 0 / 3 = 9.
        // The other order has no symbol for GroupPlus to go on and only gets 4
        Assert.Equal(9, Score(0, 0, 0));
    }
}
