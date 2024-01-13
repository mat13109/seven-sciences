using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace SevenScience;

public partial class PerformanceTester : Node
{
	[Export] private bool _isEnabled;
    [Export] private int _iterationCount;
    [Export] private int _wildCount;

    private readonly List<long> _times = new();

    public override void _Process(double delta)
    {
        if (!_isEnabled)
            return;

        GD.Print($"Testing on {_wildCount} wilds");
        GD.Print($"Average over {_iterationCount} runs");

        _isEnabled = false;

        Stopwatch watch = new();

        Dictionary<EScienceSymbol, int> scienceSymbolScores = new Dictionary<EScienceSymbol, int>
        {
            { EScienceSymbol.Cogwheel, 0 },
            { EScienceSymbol.Compass, 0 },
            { EScienceSymbol.Tablet, 0 },
        };

        for (int i = 0; i < _iterationCount; i++)
        {
            watch.Start();

            Maths.CalculateScienceScore(scienceSymbolScores, _wildCount);

            watch.Stop();

            _times.Add(watch.ElapsedMilliseconds);

            watch.Reset();
        }

        GD.Print($"Average time: {_times.Average()}ms");
        GetTree().Quit();
    }
}
