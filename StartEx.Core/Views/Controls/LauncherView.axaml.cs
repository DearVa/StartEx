using System;
using System.Collections.Generic;
using Avalonia;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
using StartEx.Core.Models;

namespace StartEx.Core.Views.Controls;

/// <summary>
/// APP����������ͼ����ͼ�ڰ���һ��APP�б�ͨ�����ɸ�<see cref="LauncherPanel"/>չʾ
/// </summary>
public partial class LauncherView : Grid {
	public static readonly StyledProperty<ObservableCollection<LauncherViewItem>> ItemsProperty =
		AvaloniaProperty.Register<LauncherView, ObservableCollection<LauncherViewItem>>(nameof(Items), new ObservableCollection<LauncherViewItem>());

	public ObservableCollection<LauncherViewItem> Items {
		get => GetValue(ItemsProperty);
		set => SetValue(ItemsProperty, value);
	}

	private readonly ObservableCollection<LauncherPanel> panels = new();

	public LauncherView() {
		InitializeComponent();

		LauncherPanelsItemsControl.Items = panels;
		panels
			.ToObservableChangeSet()
			.Transform(p => p.Width)
			.Bind(out var snackBarItems)
			.Subscribe();
		SnakeBarItemsControl.Items = snackBarItems;

		SizeChanged += OnSizeChanged;
	}

	private void OnSizeChanged(object? sender, SizeChangedEventArgs e) {
		ArrangeItems();
	}

	private void ArrangeItems() {
		var panelsIndex = 0;
		IList<LauncherViewItem> remainingItems = Items;
		var viewport = LauncherPanelsItemsControl.Scroll!.Viewport;

		while (true) {
			LauncherPanel panel;
			if (panelsIndex == panels.Count) {
				panels.Add(panel = new LauncherPanel());
			} else {
				panel = panels[panelsIndex];
			}
			
			panel.Width = viewport.Width;
			panel.Height = viewport.Height;

			var remainingCount = remainingItems.Count;
			remainingItems = panel.ArrangeItems(remainingItems, viewport);
			if (remainingItems.Count == remainingCount || remainingCount == 0) {
				for (var i = panels.Count - 1; i > panelsIndex; i--) {
					panels.RemoveAt(i);
				}

				break;
			}

			panelsIndex++;
		}
	}
}