using Avalonia.ReactiveUI;
using StartEx.Core.ViewModels;

namespace StartEx.Core.Views.Windows;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel> {
	public MainWindow(MainWindowViewModel viewModel) {
		InitializeComponent();
		ViewModel = viewModel;
	}
}