using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace SevenScience;

public partial class Main : Node
{
	public static Action OnCardCountChanged;
	
	private readonly List<ScienceContainer> _scienceCalculatorContainers = new();
	
	/// <summary>
	/// The science cards in the containers to calculate the score with wild cards in the recursive function
	/// <see cref="Maths.CalculateScienceScore"/>
	/// </summary>
	private readonly System.Collections.Generic.Dictionary<EScienceSymbol, int> _scienceCardsInContainersTemp = new();

	private Label _totalScoreText;

	public override void _Ready()
	{
		_totalScoreText = GetNode<Label>("CalculatorUI/Label");
		_totalScoreText.Text = "0";

		FindComponents();
	}

	private void FindComponents()
	{
		Array<Node> children = GetChild(0).GetChildren();

		foreach (Node node in children)
		{
			if (node is ScienceContainer container)
			{
				_scienceCalculatorContainers.Add(container);
			}
		}
	}

	public override void _EnterTree()
	{
		OnCardCountChanged += CardCountChanged;
	}

	public override void _ExitTree()
	{
		OnCardCountChanged -= CardCountChanged;
	}

	private void CardCountChanged()
	{
		_scienceCardsInContainersTemp.Clear();

		// Get the science cards in the containers
		foreach (ScienceContainer item in _scienceCalculatorContainers)
		{
			_scienceCardsInContainersTemp[item.ScienceSymbol] = item.ScienceCount;
		}

		int result;

		// If there are no wild cards, calculate the score without wild cards
		// else calculate the score with wild cards
		if (_scienceCardsInContainersTemp[EScienceSymbol.Wild] == 0)
		{
			result = Maths.CalculateScienceScoreNoWild(_scienceCardsInContainersTemp);
		}
		else
		{
			Maths.Reset();
			result = Maths.CalculateScienceScore(_scienceCardsInContainersTemp, _scienceCardsInContainersTemp[EScienceSymbol.Wild]);
		}

		// Set the total score text
		_totalScoreText.Text = result.ToString();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			GetTree().Quit();
		}
	}
}
