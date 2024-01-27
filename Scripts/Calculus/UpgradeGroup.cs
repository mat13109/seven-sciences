using Godot;
using System;
using System.Diagnostics;

namespace SevenScience;

public partial class UpgradeGroup : CheckButton
{
	private CheckButton _upgradeGroupButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_upgradeGroupButton = GetNode<CheckButton>(".");
	}

	/// <summary>
	/// Trigger when the check button is pressed
	/// </summary>
	/// <param name="isToggle">If the group upgrade leader's card is active</param>
	private void _on_toggled(bool isToggle)
	{
		if (isToggle)
		{
			Maths.GroupValue = 10;
		}
		else
		{
			Maths.GroupValue = 7;
		}
		Main.OnCardCountChanged?.Invoke();
	}
}
