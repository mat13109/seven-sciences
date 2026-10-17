using Godot;

namespace SevenScience;

public partial class ResetScore : Button
{
	private Button _resetScoreButton;
	[Export] private CheckButton _upgradeGroupButton;
	[Export] private CheckButton _aganiceButton;
	[Export] private StackCounter _groupPlusCounter;
	[Export] private PidgeonPower _pidgeonPower;
    private Main _main;

	public override void _Ready()
	{
		_resetScoreButton = GetNode<Button>(".");
        _main = GetNode<Main>("/root/Main");

		_resetScoreButton.Pressed += Reset;
	}

	public override void _ExitTree() => _resetScoreButton.Pressed -= Reset;

    /// <summary>
	/// Resets all value and button's state
	/// Used to reset the score
	/// </summary>
	private void Reset()
	{
		// Reset the power toggle buttons
		ReleaseToggle(_upgradeGroupButton);
		ReleaseToggle(_aganiceButton);

		_groupPlusCounter.Count = 0;
		_pidgeonPower.Reset();

		foreach (ScienceContainer item in _main.ScienceCalculatorContainers)
		{
			item.ScienceCount = 0;
		}

		Main.OnCardCountChanged?.Invoke();
	}

	/// <summary>
	/// Puts a toggle button back to its unpressed state
	/// </summary>
	private static void ReleaseToggle(CheckButton button)
	{
		button.ToggleMode = false;
		button.ToggleMode = true;
	}
}
