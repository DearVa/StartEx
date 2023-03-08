using Avalonia;
using Avalonia.ReactiveUI;
using Splat;
using StartEx.Core.Interfaces;
using StartEx.Core.ViewModels;

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

			locator.BindToSelf(new LauncherViewModel(locator.GetRequiredService<IAppLibraryLoader>()));
		}));
}