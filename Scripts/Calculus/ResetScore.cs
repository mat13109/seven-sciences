using Godot;

namespace SevenScience;

public partial class ResetScore : Button
{
	private Button _resetScoreButton;
	[Export] private CheckButton _upgradeGroupButton;
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
		// Reset the upgrade group toggle button
		_upgradeGroupButton.ToggleMode = false;
		_upgradeGroupButton.ToggleMode = true;

		foreach (ScienceContainer item in _main.ScienceCalculatorContainers)
		{
			item.ScienceCount = 0;
		}

		Main.OnCardCountChanged?.Invoke();
	}
}
