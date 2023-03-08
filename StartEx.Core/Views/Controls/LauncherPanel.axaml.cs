using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using StartEx.Core.Models;

namespace StartEx.Core.Views.Controls;

public partial class LauncherPanel : Canvas {
	public static readonly StyledProperty<double> GridSizeProperty =
		AvaloniaProperty.Register<LauncherView, double>(nameof(GridSize), 128);

	public double GridSize {
		get => GetValue(GridSizeProperty);
		set => SetValue(GridSizeProperty, value);
	}

	private readonly Dictionary<LauncherViewItem, Control> itemsMap = new();

	public LauncherPanel() {
		InitializeComponent();
	}

	//protected override Size MeasureOverride(Size availableSize) {
	//	var availableWidth = double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width;
	//	var availableHeight = double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height;
	//	var gridSize = GridSize;
	//	return new Size(
	//		Math.Floor(availableWidth / gridSize) * gridSize,
	//		Math.Floor(availableHeight / gridSize) * gridSize);
	//}

	/// <summary>
	/// 从items中选择能排列的项目，把剩余的项目返回
	/// </summary>
	/// <param name="items"></param>
	/// <param name="finalSize"></param>
	/// <returns></returns>
	public IList<LauncherViewItem> ArrangeItems(IList<LauncherViewItem> items, Size finalSize) {
		if (items.Count == 0) {
			return items;
		}

		var gridSize = GridSize;
		var itemRects = new List<Rect>();
		var addedControls = new HashSet<Control>();
		var remainingItems = new List<LauncherViewItem>();

		for (var i = 0; i < items.Count; i++) {
			var item = items[i];

			var itemWidth = item.HorizontalSpan * gridSize;
			var itemHeight = item.VerticalSpan * gridSize;

			if (itemWidth > finalSize.Width || itemHeight > finalSize.Height) {
				remainingItems.Add(item);
				continue;  // 无法显示，直接跳过
			}

			var point = new Point();
			var isDone = false;

			while (point.Y < finalSize.Height - itemHeight) {
				isDone = false;
				while (point.X < finalSize.Width - itemWidth) {
					var itemRect = new Rect(point, new Size(itemWidth, itemHeight));
					var canFitIn = itemRects.All(r => !r.Intersects(itemRect));
					if (!canFitIn) {
						point = new Point(point.X + gridSize, point.Y);
						continue;
					}

					itemRects.Add(itemRect);
					var addedControl = ArrangeItem(item, itemRect);
					if (addedControl != null) {
						addedControls.Add(addedControl);
					}

					isDone = true;
					break;
				}

				if (isDone) {
					break;
				}

				point = new Point(0, point.Y + gridSize);
			}

			// 如果已经排不下了，就不再继续排了
			if (!isDone) {
				for (; i < items.Count; i++) {
					remainingItems.Add(items[i]);
				}

				break;
			}
		}

		for (var i = Children.Count - 1; i >= 0; i--) {
			var control = Children[i];
			if (!addedControls.Contains(control)) {
				Children.RemoveAt(i);
				foreach (var pair in itemsMap) {
					if (pair.Value == control) {
						itemsMap.Remove(pair.Key);
					}
				}
			}
		}

		return remainingItems;
	}

	private Control? ArrangeItem(LauncherViewItem item, Rect itemRect) {
		if (!itemsMap.TryGetValue(item, out var control)) {
			foreach (var dataTemplate in DataTemplates.Where(dataTemplate => dataTemplate.Match(item))) {
				control = dataTemplate.Build(item);
				if (control == null) {
					break;
				}

				itemsMap.Add(item, control);
				Children.Add(control);
				break;
			}
		}

		if (control == null) {
			return null;
		}

		control.SetValue(LeftProperty, itemRect.X);
		control.SetValue(TopProperty, itemRect.Y);
		control.SetValue(WidthProperty, itemRect.Width);
		control.SetValue(HeightProperty, itemRect.Height);
		return control;
	}
}
