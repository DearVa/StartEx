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
	/// ��items��ѡ�������е���Ŀ����ʣ�����Ŀ����
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
				// TODO: ����Ŀ̫���Ǿ�ֱ����һ�ж���ʾ����Ŀ
				// var itemRect = new Rect(itemPoint, new Size(item.HorizontalSpan, item.VerticalSpan));
			}

			// ��һ�����Ͻǵ�λ��
			var itemPoint = new Point();

			while (itemPoint.Y < totalSpan.Height - item.Size.Y) {
				while (itemPoint.X < totalSpan.Width - item.Size.X) {
					var itemRect = new Rect(itemPoint, new Size(item.Size.X, item.Size.Y));

					if (placedItemsRects.Any(r => r.Intersects(itemRect))) {
						// ������κ��ཻ�ģ��Ǿ�˵������ط����ܷţ���������
						itemPoint = new Point(itemPoint.X + 1, itemPoint.Y);
						continue;
					}

					// �ܷ�����
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

			// ����Ѿ��Ų����ˣ��Ͳ��ټ�������
			for (; i < items.Count; i++) {
				remainingItems.Add(items[i]);
			}

			break;

		NextItem:;
		}

		for (var i = Items.Count - 1; i >= 0; i--) {
			var control = Items[i];
			if (!addedItems.Contains(control)) {  // �Ƴ��������Ŀ
				Items.RemoveAt(i);
			}
		}

		return remainingItems;
	}

	protected override void ItemsChanged() {
		
	}
}
