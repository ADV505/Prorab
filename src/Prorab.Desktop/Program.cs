using AtomUI;
using AtomUI.Desktop.Controls;
using AtomUI.Localization;
using AtomUI.Theme;
using Avalonia;
using Prorab.UI;

internal static class Program
{
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseAtomUIPlatformDetect()
            .WithAtomUIDefaultOptions()
            .UseAtomUI(builder =>
            {
                builder.WithInitialTheme(IThemeManager.DEFAULT_THEME_ID);
                builder.UseDesktopControls();
                builder.UseLanguages(LanguageTags.EnUS, [LanguageTags.EnUS]);
            });

    [STAThread]
    public static void Main(string[] args)
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
}
