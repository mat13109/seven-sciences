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
        private set
        {
            if (_scienceCount == value)
            {
                return;
            }

            _scienceCount = value;
            _scienceCountLabel.Text = _scienceCount.ToString();
            Main.OnCardCountChanged?.Invoke();
        }
    }

    public EScienceSymbol ScienceSymbol => _scienceSymbol;

    [Export] private Texture2D _texture;
    [Export] private EScienceSymbol _scienceSymbol;
    [Export] private bool _isLimited;
    [Export(PropertyHint.Range, "0,5,")] private int _max;

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
    }

    public override void _ExitTree()
    {
        _minusButton.Pressed -= Minus;
        _plusButton.Pressed -= Plus;
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
