using Avalonia;
using System;
using StartEx.Core;
using Splat;
using StartEx.Core.Interfaces;
using StartEx.Windows.Implements;

namespace StartEx.Windows;

internal static class Program {
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args) => Bootstrapper
		.BuildAvaloniaApp()
		.RegisterPlatformImplements()
		.StartWithClassicDesktopLifetime(args);

	private static AppBuilder RegisterPlatformImplements(this AppBuilder builder) =>
		builder.AfterPlatformServicesSetup(_ => Locator.RegisterResolverCallbackChanged(() => {
			var locator = AvaloniaLocator.CurrentMutable;

			locator.Bind<IAppLibraryLoader>().ToSingleton<StartMenuAppLibraryLoader>();
		}));
}