using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace StartEx.Core.Views.Controls;

public class LauncherPanel : Canvas {
	public static readonly StyledProperty<double> GridSizeProperty =
		AvaloniaProperty.Register<LauncherPanel, double>(nameof(GridSize));

	public double GridSize {
		get => GetValue(GridSizeProperty);
		set => SetValue(GridSizeProperty, value);
	}

	public LauncherPanel() {
		SizeChanged += OnSizeChanged;
	}

	private void OnSizeChanged(object? sender, SizeChangedEventArgs e) {
		var itemRects = new List<Rect>(VisualChildren.Count - 1);

		foreach (var child in VisualChildren) {
			if (child is not Layoutable layoutable) {
				continue;
			}

			if (layoutable.DesiredSize.Width > e.NewSize.Width || layoutable.DesiredSize.Height > e.NewSize.Height) {
				continue;
			}

			var point = new Point();
			var isDone = false;

			while (point.Y < e.NewSize.Height - layoutable.DesiredSize.Height) {
				isDone = false;
				while (point.X < e.NewSize.Width - layoutable.DesiredSize.Width) {
					var itemRect = new Rect(point, new Size(layoutable.DesiredSize.Width, layoutable.DesiredSize.Height));
					var canFitIn = itemRects.All(r => !r.Intersects(itemRect));
					if (!canFitIn) {
						point = new Point(point.X + GridSize, point.Y);
						continue;
					}

					layoutable.SetValue(LeftProperty, point.X);
					layoutable.SetValue(TopProperty, point.Y);
					itemRects.Add(itemRect);
					isDone = true;
					break;
				}

				if (isDone) {
					break;
				}

				point = new Point(0, point.Y + GridSize);
			}

			// 如果已经排不下了，就不再继续排了
			if (!isDone) {
				break;
			}
		}
	}
}

public class LauncherPanelItemsCollection {
	private readonly List<Control> items = new();


}