using System;
using System.Collections.Generic;
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

	[Export] private StackCounter _groupPlusCounter;

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

		_groupPlusCounter.CountChanged += OnGroupPlusCountChanged;
	}

	public override void _EnterTree() => OnCardCountChanged += CardCountChanged;

    public override void _ExitTree()
    {
        OnCardCountChanged -= CardCountChanged;
        _groupPlusCounter.CountChanged -= OnGroupPlusCountChanged;
    }

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

	private void OnGroupPlusCountChanged(int count)
	{
		Maths.GroupPlusCount = count;
		CardCountChanged();
	}

	private void CardCountChanged()
	{
		_scienceCardsInContainersTemp.Clear();

		// Get the science cards in the containers
		foreach (ScienceContainer item in ScienceCalculatorContainers)
		{
			_scienceCardsInContainersTemp[item.ScienceSymbol] = item.ScienceCount;
		}

		// There are at most 3 wild cards, so trying every assignment stays cheap
		Maths.Reset();
		int result = Maths.CalculateScienceScore(_scienceCardsInContainersTemp, _scienceCardsInContainersTemp[EScienceSymbol.Wild]);

		// Set the total score text
		_totalScoreText.Text = result.ToString();
	}

	#endregion
}
