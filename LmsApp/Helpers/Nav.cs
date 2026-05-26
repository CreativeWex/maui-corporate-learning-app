using System.Windows.Input;

namespace LmsApp.Helpers;

/// <summary>
/// Robust navigation helpers.
///
/// The Shell relative route navigation <c>GoToAsync("..")</c> fails on the
/// hidden Manager / HR / Admin TabBars with errors such as
/// "unable to figure out route for: //manager/DEFAULT_TabXX/team", because the
/// auto-generated tab route segment does not round-trip through URI resolution.
/// That single bug breaks both the top navigation-bar back arrow and the
/// "Назначить курс" submit flow. <see cref="GoBackAsync"/> pops the navigation
/// stack directly, bypassing URI resolution entirely, so it works for every role.
/// </summary>
public static class Nav
{
    /// <summary>
    /// Wires the top navigation-bar "Navigate up" arrow to a role-safe back action.
    ///
    /// Two reasons this must be done in code-behind with an explicit icon:
    /// (1) A <c>BackButtonBehavior</c> declared in XAML does NOT inherit the page's
    ///     BindingContext, so a <c>{Binding …Command}</c> there resolves to null.
    /// (2) On Android a <c>BackButtonBehavior</c> with only a <c>Command</c> (no
    ///     <c>IconOverride</c>) does not invoke the command — the default nav-icon tap
    ///     bypasses it (dotnet/maui#7045). Setting an <c>IconOverride</c> forces the
    ///     custom button whose tap runs the delegate. (.NET 10 also no longer routes
    ///     the toolbar back tap through <c>OnBackButtonPressed</c> — dotnet/maui#33523.)
    /// </summary>
    public static void AttachBackButton(Page page, ICommand? command = null)
    {
        // OpenSans has no left-arrow glyph (U+2190); "‹" (U+2039) is its back chevron.
        var icon = new FontImageSource
        {
            FontFamily = "OpenSansSemibold",
            Glyph = "‹",
            Size = 28
        };
        icon.SetAppThemeColor(FontImageSource.ColorProperty, Colors.Black, Colors.White);

        Shell.SetBackButtonBehavior(page, new BackButtonBehavior
        {
            IconOverride = icon,
            Command = command ?? new Command(async () => await GoBackAsync())
        });
    }

    public static async Task GoBackAsync()
    {
        var shell = Shell.Current;
        if (shell == null) return;

        var nav = shell.Navigation;
        try
        {
            if (nav.NavigationStack.Count > 1)
            {
                await nav.PopAsync();
                return;
            }
            if (nav.ModalStack.Count > 0)
            {
                await nav.PopModalAsync();
                return;
            }
            // Nothing pushed (we are at a tab root) — there is nowhere to go back to.
        }
        catch
        {
            // Last-resort fallback; never let back navigation surface an error.
            try { await shell.GoToAsync(".."); } catch { }
        }
    }
}
