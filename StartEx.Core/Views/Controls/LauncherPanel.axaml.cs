using System.Collections.Generic;
using System.Linq;
using Avalonia;
using StartEx.Core.Models;
using StartEx.PhysicsEngine;
using StartEx.PhysicsEngine.DataTypes;

namespace StartEx.Core.Views.Controls;

public partial class LauncherPanel : PhysicsPanel {
	public LauncherPanel() {
		InitializeComponent();
	}

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

		var scale = Scale;
		var totalSpan = finalSize / scale;

		var placedItemsRects = new List<Rect>();
		var addedItems = new HashSet<LauncherViewItem>();
		var remainingItems = new List<LauncherViewItem>();

		for (var i = 0; i < items.Count; i++) {
			var item = items[i];

			if (item.Size.X > totalSpan.Width) {
				// TODO: 此项目太宽，那就直接让一行都显示此项目
				// var itemRect = new Rect(itemPoint, new Size(item.HorizontalSpan, item.VerticalSpan));
			}

			// 这一项左上角的位置
			var itemPoint = new Point();

			while (itemPoint.Y < totalSpan.Height - item.Size.Y) {
				while (itemPoint.X < totalSpan.Width - item.Size.X) {
					var itemRect = new Rect(itemPoint, new Size(item.Size.X, item.Size.Y));

					if (placedItemsRects.Any(r => r.Intersects(itemRect))) {
						// 如果有任何相交的，那就说明这个地方不能放，继续往右
						itemPoint = new Point(itemPoint.X + 1, itemPoint.Y);
						continue;
					}

					// 能放下了
					placedItemsRects.Add(itemRect);
					addedItems.Add(item);
					if (!Items.Contains(item)) {
						Items.Add(item);
					}

					item.TargetPosition = new Vector3(itemRect.X, itemRect.Y, 0);
					goto NextItem;
				}

				itemPoint = new Point(0, itemPoint.Y + 1);
			}

			// 如果已经排不下了，就不再继续排了
			for (; i < items.Count; i++) {
				remainingItems.Add(items[i]);
			}

			break;

		NextItem:;
		}

		for (var i = Items.Count - 1; i >= 0; i--) {
			var control = Items[i];
			if (!addedItems.Contains(control)) {  // 移除多余的项目
				Items.RemoveAt(i);
			}
		}

		return remainingItems;
	}

	protected override void ItemsChanged() {
		
	}
}
