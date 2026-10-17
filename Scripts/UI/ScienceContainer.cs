using Godot;

namespace SevenScience;

public partial class ScienceContainer : HBoxContainer
{
	/// <summary>
	/// The current count of science symbols in this container
	/// </summary>
	public int ScienceCount
	{
		get => _scienceCount;
		set
		{
			if (_scienceCount == value)
			{
				return;
			}

			_scienceCount = value;
			_scienceCountLabel.Text = _scienceCount.ToString();
			UpdateButtons();
			Main.OnCardCountChanged?.Invoke();
		}
	}

	public EScienceSymbol ScienceSymbol => _scienceSymbol;

	[Export] private Texture2D _texture;
	[Export] private EScienceSymbol _scienceSymbol;
	[Export] private bool _isLimited;
	[Export] private int _max;

	private static readonly Color DisabledSignColor = new(1f, 1f, 1f, 0.25f);

	private TextureRect _textureRect;
	private Button _minusButton;
	private Button _plusButton;
	private int _scienceCount;
	private Label _scienceCountLabel;

	public override void _Ready()
	{
		_textureRect = GetNode<TextureRect>("TextureRect");
		_textureRect.Texture = _texture;

		_scienceCountLabel = GetNode<Label>("TextureRect/Label");
		_scienceCountLabel.Text = "0";

		_minusButton = GetNode<Button>("MinusButton");
		_plusButton = GetNode<Button>("PlusButton");

		_minusButton.Pressed += Minus;
		_plusButton.Pressed += Plus;

		UpdateButtons();
	}

	public override void _ExitTree()
	{
		_minusButton.Pressed -= Minus;
		_plusButton.Pressed -= Plus;
	}

	/// <summary>
	/// Disables the minus button at 0 and the plus button at the max
	/// </summary>
	private void UpdateButtons()
	{
		SetButtonEnabled(_minusButton, _scienceCount > 0);
		SetButtonEnabled(_plusButton, !_isLimited || _scienceCount < _max);
	}

	/// <summary>
	/// The sign is a label child of the button, so it has to be dimmed by hand to look disabled
	/// </summary>
	private static void SetButtonEnabled(Button button, bool isEnabled)
	{
		button.Disabled = !isEnabled;
		button.GetNode<Label>("Label").Modulate = isEnabled ? Colors.White : DisabledSignColor;
	}

	private void Plus()
	{
		// If the count is limited and the count is already at the max, return
		if (_isLimited && ScienceCount >= _max)
		{
			return;
		}

		ScienceCount++;
	}

	private void Minus()
	{
		// If the count is already at 0, return
		if (ScienceCount > 0)
		{
			ScienceCount--;
		}
	}
}
