using Xunit;

namespace SevenScience.Tests;

public class GroupPlusTests : MathsTestBase
{
    [Fact]
    public void NoStackChangesNothing()
        => Assert.Equal(10, Score(3, 1, 0));

    [Fact]
    public void AddsASymbolToTheTypeOwnedTheMost()
    {
        Maths.GroupPlusCount = 1;

        Assert.Equal(10, Score(2, 1, 0));  // 3 / 1 / 0
        Assert.Equal(17, Score(0, 3, 1));  // 0 / 4 / 1, works whatever the symbol is
    }

    [Fact]
    public void OnATieOneOfTheTiedTypesGetsIt()
    {
        Maths.GroupPlusCount = 1;

        Assert.Equal(13, Score(2, 2, 0));  // 3 / 2 / 0
    }

    [Fact]
    public void StackedSymbolsAllGoToTheSameType()
    {
        Maths.GroupPlusCount = 2;

        Assert.Equal(17, Score(2, 1, 0));  // 4 / 1 / 0 and not 3 / 2 / 0
    }

    [Fact]
    public void FiveStacksOnATripleTie()
    {
        Maths.GroupPlusCount = 5;

        Assert.Equal(45, Score(1, 1, 1));  // 6 / 1 / 1: 38 + one trio
    }

    [Fact]
    public void NothingIsAddedWithoutAnySymbol()
    {
        Maths.GroupPlusCount = 3;

        Assert.Equal(0, Score(0, 0, 0));
    }

    [Fact]
    public void WorksWithAristote()
    {
        UseAristote();
        Maths.GroupPlusCount = 1;

        Assert.Equal(16, Score(1, 1, 1));  // 2 / 1 / 1: 6 + one trio (10)
    }

    [Fact]
    public void WildCardsAreAssignedBeforeTheBonus()
    {
        Maths.GroupPlusCount = 1;

        // Wild on the compass makes 2 / 0 / 0, then the bonus makes 3 / 0 / 0
        Assert.Equal(9, Score(1, 0, 0, 1));
    }
}
