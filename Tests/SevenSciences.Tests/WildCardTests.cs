using Xunit;

namespace SevenScience.Tests;

public class WildCardTests : MathsTestBase
{
    [Theory]
    [InlineData(0, 0, 0, 1, 1)]
    [InlineData(0, 0, 0, 2, 4)]   // both on one symbol
    [InlineData(0, 0, 0, 3, 10)]  // one of each beats 9 for three identical
    [InlineData(2, 0, 0, 1, 9)]   // makes a 3 instead of a 2 and a 1
    [InlineData(1, 1, 0, 1, 10)]  // completes a trio
    [InlineData(2, 2, 0, 3, 31)]  // 3 / 2 / 2: 17 + two trios
    public void WildCardsTakeTheBestSymbol(int compass, int tablet, int cogwheel, int wild, int expected)
        => Assert.Equal(expected, Score(compass, tablet, cogwheel, wild));

    [Fact]
    public void WildCardsFavorTriosWithAristote()
    {
        UseAristote();

        Assert.Equal(13, Score(0, 0, 0, 3));
    }

    [Fact]
    public void ScoringTwiceGivesTheSameResult()
        => Assert.Equal(Score(2, 2, 0, 3), Score(2, 2, 0, 3));

    [Fact]
    public void AnEarlierHigherScoreDoesNotLeakIntoTheNextOne()
    {
        Score(2, 2, 2, 3);

        Assert.Equal(1, Score(0, 0, 0, 1));
    }
}
