using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Splat;

namespace StartEx.Core;

public static class Bootstrapper {
	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.LogToTrace()
			.UseReactiveUI()
			.RegisterServices();

	public static TAppBuilder RegisterServices<TAppBuilder>(this TAppBuilder builder)
		where TAppBuilder : AppBuilderBase<TAppBuilder>, new() =>
			builder.AfterPlatformServicesSetup(_ => Locator.RegisterResolverCallbackChanged(() => {
				if (Locator.CurrentMutable is null) {
					return;
				}


			}));
}