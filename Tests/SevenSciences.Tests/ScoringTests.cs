using Xunit;

namespace SevenScience.Tests;

public class ScoringTests : MathsTestBase
{
    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(1, 0, 0, 1)]
    [InlineData(2, 0, 0, 4)]
    [InlineData(3, 0, 0, 9)]
    [InlineData(4, 0, 0, 16)]
    [InlineData(0, 4, 0, 16)]
    [InlineData(0, 0, 4, 16)]
    public void IdenticalSymbolsScoreTheirSquare(int compass, int tablet, int cogwheel, int expected)
        => Assert.Equal(expected, Score(compass, tablet, cogwheel));

    [Theory]
    [InlineData(1, 1, 1, 10)]  // 3 + one trio (7)
    [InlineData(2, 2, 2, 26)]  // 12 + two trios
    [InlineData(2, 2, 1, 16)]  // 9 + one trio
    [InlineData(3, 1, 0, 10)]  // no trio without the third symbol
    [InlineData(4, 3, 2, 43)]  // 29 + two trios
    [InlineData(10, 10, 10, 370)]
    public void TriosAddSevenPointsEach(int compass, int tablet, int cogwheel, int expected)
        => Assert.Equal(expected, Score(compass, tablet, cogwheel));

    [Theory]
    [InlineData(1, 1, 1, 13)]  // 3 + one trio (10)
    [InlineData(2, 2, 2, 32)]  // 12 + two trios
    [InlineData(2, 2, 1, 19)]
    [InlineData(3, 1, 0, 10)]  // Aristote only counts complete trios
    public void AristoteMakesTriosWorthTen(int compass, int tablet, int cogwheel, int expected)
    {
        UseAristote();

        Assert.Equal(expected, Score(compass, tablet, cogwheel));
    }

    [Fact]
    public void TriosAreWorthSevenByDefault()
        => Assert.Equal(7, Maths.GroupValue);

    [Fact]
    public void SymbolOrderDoesNotMatter()
        => Assert.Equal(Score(5, 2, 1), Score(1, 5, 2));
}
