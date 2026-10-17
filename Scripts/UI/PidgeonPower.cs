using Godot;

namespace SevenScience;

/// <summary>
/// The Pidgeon power: how many times it is stacked and how many green cards of each symbol the neighbors have.
/// Feeds <see cref="Maths"/> and triggers a recalculation when one of the counters changes
/// </summary>
public partial class PidgeonPower : VBoxContainer
{
	[Export] private StackCounter _stackCounter;
	[Export] private StackCounter _compassCounter;
	[Export] private StackCounter _tabletCounter;
	[Export] private StackCounter _cogwheelCounter;

	public override void _Ready()
	{
		_stackCounter.CountChanged += OnCountChanged;
		_compassCounter.CountChanged += OnCountChanged;
		_tabletCounter.CountChanged += OnCountChanged;
		_cogwheelCounter.CountChanged += OnCountChanged;
	}

	public override void _ExitTree()
	{
		_stackCounter.CountChanged -= OnCountChanged;
		_compassCounter.CountChanged -= OnCountChanged;
		_tabletCounter.CountChanged -= OnCountChanged;
		_cogwheelCounter.CountChanged -= OnCountChanged;
	}

	/// <summary>
	/// Puts every counter back to 0
	/// </summary>
	public void Reset()
	{
		_stackCounter.Count = 0;
		_compassCounter.Count = 0;
		_tabletCounter.Count = 0;
		_cogwheelCounter.Count = 0;
	}

	private void OnCountChanged(int _)
	{
		Maths.PidgeonCount = _stackCounter.Count;
		Maths.PidgeonAvailableCards[(int)EScienceSymbol.Compass] = _compassCounter.Count;
		Maths.PidgeonAvailableCards[(int)EScienceSymbol.Tablet] = _tabletCounter.Count;
		Maths.PidgeonAvailableCards[(int)EScienceSymbol.Cogwheel] = _cogwheelCounter.Count;

		Main.OnCardCountChanged?.Invoke();
	}
}
