using Godot;

namespace SevenScience;

public partial class CalculatorButton : Button
{
	[Export] private string _defaultText;
	
	private Label _label;
	
	public override void _Ready()
	{
		_label = GetNode<Label>("Label");
		_label.Text = _defaultText;
	}
}
