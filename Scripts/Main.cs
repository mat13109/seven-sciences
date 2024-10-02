using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace SevenScience;

public partial class Main : Node
{
	#region Members

	public readonly List<ScienceContainer> ScienceCalculatorContainers = new();

	/// <summary>
	/// The science cards in the containers to calculate the score with wild cards in the recursive function
	/// <see cref="Maths.CalculateScienceScore"/>
	/// </summary>
	private readonly System.Collections.Generic.Dictionary<EScienceSymbol, int> _scienceCardsInContainersTemp = new();

	private Label _totalScoreText;

	#endregion

	#region Accessors

	public static Action OnCardCountChanged { get; private set; }

	#endregion

	#region Public methods

	public override void _Ready()
	{
		_totalScoreText = GetNode<Label>("CalculatorUI/BottomContainer/Label");
		_totalScoreText.Text = "0";

		FindContainers();
	}

	public override void _EnterTree() => OnCardCountChanged += CardCountChanged;

    public override void _ExitTree() => OnCardCountChanged -= CardCountChanged;

    public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			GetTree().Quit();
		}
	}

	#endregion

	#region Private methods

	private void FindContainers()
	{
		Array<Node> children = GetChild(0).GetChildren();

		foreach (Node node in children)
		{
			if (node is ScienceContainer container)
			{
				ScienceCalculatorContainers.Add(container);
			}
		}
	}

	private void CardCountChanged()
	{
		_scienceCardsInContainersTemp.Clear();

		// Get the science cards in the containers
		foreach (ScienceContainer item in ScienceCalculatorContainers)
		{
			_scienceCardsInContainersTemp[item.ScienceSymbol] = item.ScienceCount;
		}

		int result;

        switch (_scienceCardsInContainersTemp[EScienceSymbol.Wild])
        {
            // If there are no wild cards, calculate the score without wild cards
            // else calculate the score with wild cards
            // but if there's a lot of wilds, just add them all the highest count
            case 0:
                result = Maths.CalculateScienceScoreNoWild(_scienceCardsInContainersTemp);
                break;
            case < 5:
                Maths.Reset();
                result = Maths.CalculateScienceScore(_scienceCardsInContainersTemp, _scienceCardsInContainersTemp[EScienceSymbol.Wild]);
                break;
            default:
                int wildCount = _scienceCardsInContainersTemp[EScienceSymbol.Wild];
                _scienceCardsInContainersTemp.Remove(EScienceSymbol.Wild);
                EScienceSymbol highestSymbol = _scienceCardsInContainersTemp.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
                _scienceCardsInContainersTemp[highestSymbol] += wildCount;
                result = Maths.CalculateScienceScoreNoWild(_scienceCardsInContainersTemp);
                break;
        }

		// Set the total score text
		_totalScoreText.Text = result.ToString();
	}

	#endregion
}
