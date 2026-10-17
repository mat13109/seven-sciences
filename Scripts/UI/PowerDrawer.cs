using Godot;

namespace SevenScience;

/// <summary>
/// Bottom drawer holding the leader power toggles.
/// Always peeks from the bottom of the screen; tapping or swiping up on its handle pulls it up,
/// the close button or a swipe down on the handle puts it back.
/// </summary>
public partial class PowerDrawer : PanelContainer
{
	[Export] private Button _closeButton;
	[Export] private Control _grabHandle;
	[Export] private ScrollContainer _scroll;
	[Export] private GridContainer _grid;
	/// <summary>
	/// Height of the strip still visible when the drawer is closed, keep it in sync with the bottom offset of the calculator UI
	/// </summary>
	[Export] private float _peekHeight = 100f;
	/// <summary>
	/// Space always left above the open drawer
	/// </summary>
	[Export] private float _topGap = 80f;
	[Export] private float _swipeThreshold = 60f;
	[Export] private float _slideDuration = 0.25f;

	private Tween _tween;
	private float _drawerHeight;
	private bool _isOpen;
	private bool _isDragging;
	private bool _hasSwiped;
	private float _dragStartY;

	public override void _Ready()
	{
		_drawerHeight = GetCombinedMinimumSize().Y;
		_closeButton.Visible = false;
		UpdateLayout();
		GetViewport().SizeChanged += UpdateLayout;

		_closeButton.Pressed += Close;
		_grabHandle.GuiInput += OnGrabHandleInput;
	}

	public override void _ExitTree()
	{
		GetViewport().SizeChanged -= UpdateLayout;
		_closeButton.Pressed -= Close;
		_grabHandle.GuiInput -= OnGrabHandleInput;
	}

	/// <summary>
	/// Lays the powers out on two columns when they fit the width, otherwise on one,
	/// then sizes the drawer to its content without ever exceeding the screen height (the powers scroll instead)
	/// </summary>
	private void UpdateLayout()
	{
		Vector2 viewportSize = GetViewportRect().Size;

		_grid.Columns = 1;
		float rowWidth = 0f;

		foreach (Node child in _grid.GetChildren())
		{
			if (child is Control row)
			{
				rowWidth = Mathf.Max(rowWidth, row.GetCombinedMinimumSize().X);
			}
		}

		float chromeWidth = GetCombinedMinimumSize().X - _grid.GetCombinedMinimumSize().X;
		float twoColumnsWidth = (2f * rowWidth) + _grid.GetThemeConstant("h_separation") + chromeWidth;
		_grid.Columns = viewportSize.X >= twoColumnsWidth ? 2 : 1;

		ApplyDrawerHeight();
		// Safety net in case the minimum sizes were not fully refreshed yet
		Callable.From(ApplyDrawerHeight).CallDeferred();
	}

	private void ApplyDrawerHeight()
	{
		_tween?.Kill();

		_scroll.CustomMinimumSize = Vector2.Zero;
		float chromeHeight = GetCombinedMinimumSize().Y - _scroll.GetCombinedMinimumSize().Y;
		float maxScrollHeight = GetViewportRect().Size.Y - _topGap - chromeHeight;
		float scrollHeight = Mathf.Min(_grid.GetCombinedMinimumSize().Y, maxScrollHeight);
		_scroll.CustomMinimumSize = new Vector2(0f, scrollHeight);

		_drawerHeight = chromeHeight + scrollHeight;
		OffsetTop = _isOpen ? -_drawerHeight : -_peekHeight;
		OffsetBottom = _isOpen ? 0f : _drawerHeight - _peekHeight;
	}

	private void Open()
	{
		_isOpen = true;
		_closeButton.Visible = true;
		Slide(-_drawerHeight, 0f);
	}

	private void Close()
	{
		_isOpen = false;
		_closeButton.Visible = false;
		Slide(-_peekHeight, _drawerHeight - _peekHeight);
	}

	private void Slide(float top, float bottom)
	{
		_tween?.Kill();
		_tween = CreateTween().SetParallel().SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
		_tween.TweenProperty(this, "offset_top", top, _slideDuration);
		_tween.TweenProperty(this, "offset_bottom", bottom, _slideDuration);
	}

	/// <summary>
	/// A tap toggles the drawer, a swipe up opens it and a swipe down closes it
	/// </summary>
	private void OnGrabHandleInput(InputEvent @event)
	{
		switch (@event)
		{
			case InputEventMouseButton { ButtonIndex: MouseButton.Left } button:
				if (button.Pressed)
				{
					_isDragging = true;
					_hasSwiped = false;
					_dragStartY = button.Position.Y;
				}
				else if (_isDragging)
				{
					_isDragging = false;

					if (!_hasSwiped)
					{
						if (_isOpen)
						{
							Close();
						}
						else
						{
							Open();
						}
					}
				}

				break;
			case InputEventMouseMotion motion when _isDragging && !_hasSwiped:
				float travelled = motion.Position.Y - _dragStartY;

				if (travelled < -_swipeThreshold)
				{
					_hasSwiped = true;
					Open();
				}
				else if (travelled > _swipeThreshold)
				{
					_hasSwiped = true;
					Close();
				}

				break;
		}
	}
}
