using System;
using Avalonia;
using StartEx.Core.Extensions;
using StartEx.PhysicsEngine;
using StartEx.PhysicsEngine.DataTypes;

namespace StartEx.Core.Views.Controls; 

public partial class LauncherFolderPanel : PhysicsPanel {
	public static readonly StyledProperty<PixelSize> GridCountProperty =
		AvaloniaProperty.Register<LauncherView, PixelSize>(nameof(GridCount), new PixelSize(3, 3));

	/// <summary>
	/// Íø¸ñÊýÁ¿
	/// </summary>
	public PixelSize GridCount {
		get => GetValue(GridCountProperty);
		set => SetValue(GridCountProperty, value);
	}

	public LauncherFolderPanel() {
		InitializeComponent();
		Scale = 128d / 3d;
		Width = Height = 128d;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
		base.OnPropertyChanged(change);

		if (change.Property == GridCountProperty) {
			var newGridCount = (PixelSize)change.NewValue.NotNull();
			Scale = 128d / Math.Max(newGridCount.Width, newGridCount.Height);
			ArrangeItems();
		}
	}

	protected override void ItemsChanged() {
		ArrangeItems();
	}

	private void ArrangeItems() {
		var gridCount = GridCount;
		for (var y = 0; y < gridCount.Height; y++) {
			for (var x = 0; x < gridCount.Width; x++) {
				var i = y * gridCount.Width + x;
				if (i == Items.Count) {
					return;
				}

				Items[i].TargetPosition = new Vector3(x, y, 0);
			}
		}
	}
}