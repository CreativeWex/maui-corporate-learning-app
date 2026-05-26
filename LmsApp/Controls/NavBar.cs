using System.Windows.Input;
using LmsApp.Helpers;

namespace LmsApp.Controls;

/// <summary>
/// Единая премиум-шапка с кнопкой «Назад» (стиль Apple).
///
/// Кладётся первым элементом на любой вложенный экран; системная панель Shell при
/// этом скрывается через <c>Shell.NavBarIsVisible="False"</c>. Одна шапка работает
/// одинаково для ВСЕХ ролей и пользователей, потому что кнопка вызывает
/// <see cref="Nav.GoBackAsync"/> напрямую — минуя URI-маршруты Shell, которые
/// ломаются на скрытых TabBar'ах Manager / HR / Admin.
///
/// Стрелка «Назад» автоматически скрывается там, где возвращаться некуда (на
/// корневых вкладках), поэтому компонент безопасно ставить на любую страницу.
///
/// Для экранов с подтверждением выхода (например, квиз) передайте свой
/// <see cref="Command"/> — он выполнится вместо обычного возврата.
/// </summary>
public class NavBar : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(NavBar), string.Empty,
            propertyChanged: (b, _, v) => ((NavBar)b)._title.Text = v as string ?? string.Empty);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(NavBar), null);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Необязательное действие вместо стандартного возврата (напр., подтверждение выхода).</summary>
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    readonly Label _title;
    readonly View _back;

    public NavBar()
    {
        // OpenSans не содержит U+2190 (←); "‹" (U+2039) — его back-шеврон.
        var chevron = new Label
        {
            Text = "‹",
            FontFamily = "OpenSansSemibold",
            FontSize = 30,
            VerticalOptions = LayoutOptions.Center
        };
        chevron.SetDynamicResource(Label.TextColorProperty, "Primary");

        var caption = new Label
        {
            Text = "Назад",
            FontFamily = "OpenSansSemibold",
            FontSize = 17,
            VerticalOptions = LayoutOptions.Center
        };
        caption.SetDynamicResource(Label.TextColorProperty, "Primary");

        var backStack = new HorizontalStackLayout
        {
            Spacing = 2,
            VerticalOptions = LayoutOptions.Center,
            Children = { chevron, caption }
        };
        backStack.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(OnBack) });
        _back = backStack;

        _title = new Label
        {
            FontFamily = "OpenSansSemibold",
            FontSize = 17,
            MaxLines = 1,
            LineBreakMode = LineBreakMode.TailTruncation,
            HorizontalOptions = LayoutOptions.End,
            HorizontalTextAlignment = TextAlignment.End,
            VerticalOptions = LayoutOptions.Center
        };
        _title.SetAppThemeColor(Label.TextColorProperty, Color.FromArgb("#111827"), Colors.White);

        var header = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 8,
            // Сверху — небольшой запас под системный статус-бар (MAUI сам
            // отступает от него, это лишь «воздух» для премиального вида).
            Padding = new Thickness(12, 12, 16, 10)
        };
        header.Add(backStack, 0, 0);
        header.Add(_title, 1, 0);

        var hairline = new BoxView { HeightRequest = 1 };
        hairline.SetAppThemeColor(BoxView.ColorProperty, Color.FromArgb("#E5E7EB"), Color.FromArgb("#374151"));

        var root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            }
        };
        root.Add(header, 0, 0);
        root.Add(hairline, 0, 1);
        Content = root;

        // Видимость стрелки зависит от того, есть ли куда возвращаться.
        // К моменту Loaded страница уже находится в стеке навигации.
        Loaded += (_, _) => _back.IsVisible = CanGoBack();
    }

    static bool CanGoBack()
    {
        var nav = Shell.Current?.Navigation;
        if (nav == null) return false;
        return nav.NavigationStack.Count > 1 || nav.ModalStack.Count > 0;
    }

    async void OnBack()
    {
        if (Command is { } command)
        {
            if (command.CanExecute(null))
                command.Execute(null);
            return;
        }
        await Nav.GoBackAsync();
    }
}
