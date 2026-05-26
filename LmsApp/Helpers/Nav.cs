namespace LmsApp.Helpers;

/// <summary>
/// Единая, не зависящая от роли навигация «назад».
///
/// Относительный маршрут Shell <c>GoToAsync("..")</c> падает на скрытых TabBar'ах
/// Manager / HR / Admin с ошибками вида
/// "unable to figure out route for: //manager/DEFAULT_TabXX/team": сегмент
/// авто-сгенерированного маршрута вкладки не разбирается обратно через URI.
/// <see cref="GoBackAsync"/> снимает страницу со стека навигации напрямую, минуя
/// разбор URI, поэтому работает для каждой роли. Это единственная точка возврата:
/// её зовут и общая шапка <c>Controls.NavBar</c>, и <c>BaseViewModel.GoBackCommand</c>.
/// </summary>
public static class Nav
{
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
