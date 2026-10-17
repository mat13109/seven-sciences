namespace SevenScience.Tests;

/// <summary>
/// Puts the static settings of <see cref="Maths"/> back to their defaults around every test
/// </summary>
public abstract class MathsTestBase : IDisposable
{
    /// <summary>
    /// The value of a trio before any power touches it, read once before any test changes it
    /// </summary>
    private static readonly int s_defaultGroupValue = Maths.GroupValue;

    protected MathsTestBase() => ResetMaths();

    public void Dispose() => ResetMaths();

    /// <summary>
    /// Score of the given cards with the current settings of <see cref="Maths"/>, the way <c>Main</c> computes it
    /// </summary>
    protected static int Score(int compass, int tablet, int cogwheel, int wild = 0)
    {
        Dictionary<EScienceSymbol, int> cards = new()
        {
            { EScienceSymbol.Compass, compass },
            { EScienceSymbol.Tablet, tablet },
            { EScienceSymbol.Cogwheel, cogwheel },
            { EScienceSymbol.Wild, wild },
        };

        Maths.Reset();
        return Maths.CalculateScienceScore(cards, wild);
    }

    /// <summary>
    /// Turns the Aristote power on
    /// </summary>
    protected static void UseAristote() => Maths.GroupValue = 10;

    protected static void SetPidgeon(int stacks, int compass, int tablet, int cogwheel)
    {
        Maths.PidgeonCount = stacks;
        Maths.PidgeonAvailableCards[(int)EScienceSymbol.Compass] = compass;
        Maths.PidgeonAvailableCards[(int)EScienceSymbol.Tablet] = tablet;
        Maths.PidgeonAvailableCards[(int)EScienceSymbol.Cogwheel] = cogwheel;
    }

    private static void ResetMaths()
    {
        Maths.GroupValue = s_defaultGroupValue;
        Maths.GroupPlusCount = 0;
        Maths.PidgeonCount = 0;
        Maths.AganiceEnabled = false;
        Array.Clear(Maths.PidgeonAvailableCards);
    }
}
