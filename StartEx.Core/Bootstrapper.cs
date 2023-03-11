using Avalonia;
using Avalonia.ReactiveUI;
using Splat;
using StartEx.Core.Interfaces;
using StartEx.Core.ViewModels;
using StartEx.Core.Views.Windows;
using StartEx.PhysicsEngine;
using StartEx.PhysicsEngine.Interfaces;

namespace StartEx.Core;

public static class Bootstrapper {
	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.LogToTrace()
			.UseReactiveUI()
			.RegisterPlatformImplements();

	private static AppBuilder RegisterPlatformImplements(this AppBuilder builder) =>
		builder.AfterSetup(_ => Locator.RegisterResolverCallbackChanged(() => {
			var locator = AvaloniaLocator.CurrentMutable;
			
			locator.Bind<IPhysicsScene>().ToConstant(new PhysicsScene());

			locator.BindToSelf(new MainWindowViewModel(locator.GetRequiredService<IAppLibraryLoader>()));
			locator.BindToSelf(new MainWindow(locator.GetRequiredService<MainWindowViewModel>()));
		}));
}