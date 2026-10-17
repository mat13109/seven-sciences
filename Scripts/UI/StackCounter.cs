using System;
using Godot;

namespace SevenScience;

/// <summary>
/// Minus / count / plus stepper for a power that can be stacked up to <see cref="_max"/> times
/// </summary>
public partial class StackCounter : HBoxContainer
{
	/// <summary>
	/// How many times the power is currently stacked, 0 means the power is off
	/// </summary>
	public int Count
	{
		get => _count;
		set
		{
			int clamped = Math.Clamp(value, 0, _max);

			if (_count == clamped)
			{
				return;
			}

			_count = clamped;
			UpdateDisplay();
			CountChanged?.Invoke(_count);
		}
	}

	public event Action<int> CountChanged;

	[Export] private int _max = 1;
	[Export] private Button _minusButton;
	[Export] private Button _plusButton;
	[Export] private Label _countLabel;

	private int _count;

	public override void _Ready()
	{
		_minusButton.Pressed += OnMinusPressed;
		_plusButton.Pressed += OnPlusPressed;
		UpdateDisplay();
	}

	public override void _ExitTree()
	{
		_minusButton.Pressed -= OnMinusPressed;
		_plusButton.Pressed -= OnPlusPressed;
	}

	private void OnMinusPressed() => Count--;

	private void OnPlusPressed() => Count++;

	private void UpdateDisplay()
	{
		_countLabel.Text = _count.ToString();
		_minusButton.Disabled = _count <= 0;
		_plusButton.Disabled = _count >= _max;
	}
}
