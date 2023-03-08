using System.Collections.Generic;
using Avalonia;
using Avalonia.ReactiveUI;
using StartEx.Core.ViewModels;
using System.Collections.ObjectModel;
using StartEx.Core.Models;

namespace StartEx.Core.Views.Controls;

/// <summary>
/// APP启动器的视图，视图内包含一个APP列表，通过若干个<see cref="LauncherPanel"/>展示
/// </summary>
public partial class LauncherView : ReactiveUserControl<LauncherViewModel> {
	public static readonly StyledProperty<ObservableCollection<LauncherPanel>> PanelsProperty =
		AvaloniaProperty.Register<LauncherView, ObservableCollection<LauncherPanel>>(nameof(Panels), new ObservableCollection<LauncherPanel>());

	public ObservableCollection<LauncherPanel> Panels {
		get => GetValue(PanelsProperty);
		set => SetValue(PanelsProperty, value);
	}

	public LauncherView() {
		ViewModel = AvaloniaLocator.Current.GetRequiredService<LauncherViewModel>();
		InitializeComponent();
	}
	
	protected override Size ArrangeOverride(Size finalSize) {
		finalSize = base.ArrangeOverride(finalSize);
		ArrangeItems(finalSize);
		return finalSize;
	}

	private void ArrangeItems(Size finalSize) {
		if (ViewModel == null) {
			return;
		}

		var panelsIndex = 0;
		IList<LauncherViewItem> remainingItems = ViewModel.Items;

		while (true) {
			LauncherPanel panel;
			if (panelsIndex == Panels.Count) {
				Panels.Add(panel = new LauncherPanel());
			} else {
				panel = Panels[panelsIndex];
			}

			var remainingCount = remainingItems.Count;
			remainingItems = panel.ArrangeItems(remainingItems, finalSize);
			if (remainingItems.Count == remainingCount || remainingCount == 0) {
				for (var i = Panels.Count - 1; i > panelsIndex; i--) {
					Panels.RemoveAt(i);
				}

				break;
			}

			panelsIndex++;
		}
	}
}