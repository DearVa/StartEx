using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using StartEx.Core.Views.Windows;

namespace StartEx.Core; 

public partial class App : Application {
	public override void Initialize() {
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted() {
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
			desktop.MainWindow = AvaloniaLocator.Current.GetRequiredService<MainWindow>();
		}

		base.OnFrameworkInitializationCompleted();
	}
}