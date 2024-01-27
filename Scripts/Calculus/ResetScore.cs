using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace SevenScience;

public partial class ResetScore : Button
{
	private Button _resetScoreButton;
	public CheckButton _upgradeGroupButton;

	public override void _Ready()
	{
		_resetScoreButton = GetNode<Button>(".");
		_upgradeGroupButton = GetNode<CheckButton>("/root/Main/CalculatorUI/HBoxContainer/UpgradeLotsButton");

		_resetScoreButton.Pressed += Reset;
	}

	public override void _ExitTree()
	{
		_resetScoreButton.Pressed -= Reset;
	}

	/// <summary>
	/// Reinitializes all value and button's state
	/// Used to reset the score
	/// </summary>
	private void Reset()
	{
		// Reset the upgrade group toggle button
		_upgradeGroupButton.ToggleMode = false;
		_upgradeGroupButton.ToggleMode = true;
		
		foreach (ScienceContainer item in GetNode<Main>("/root/Main")._scienceCalculatorContainers)
		{
			item.ScienceCount = 0;
		}
		Main.OnCardCountChanged?.Invoke();
	}
}
