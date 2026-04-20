using Godot;

namespace SevenScience;

public partial class UpgradeGroup : CheckButton
{
	private const int BaseGroupValue = 7;
	private const int UpgradedGroupValue = 10;

	/// <summary>
	/// Trigger when the check button is pressed
	/// </summary>
	/// <param name="isToggle">If the group upgrade leader's card is active</param>
	private static void _on_toggled(bool isToggle)
	{
		Maths.GroupValue = isToggle ? UpgradedGroupValue : BaseGroupValue;
		Main.OnCardCountChanged?.Invoke();
	}
}
