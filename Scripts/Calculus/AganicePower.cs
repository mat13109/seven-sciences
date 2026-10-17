using Godot;

namespace SevenScience;

/// <summary>
/// The Aganice power toggle: lets the calculator replace one symbol of the city by the best one
/// </summary>
public partial class AganicePower : CheckButton
{
	public override void _Ready() => Toggled += OnToggled;

	public override void _ExitTree() => Toggled -= OnToggled;

	private static void OnToggled(bool isToggled)
	{
		Maths.AganiceEnabled = isToggled;
		Main.OnCardCountChanged?.Invoke();
	}
}
