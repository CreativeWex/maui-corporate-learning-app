namespace LmsApp.Controls;

public class StreakCalendarView : GraphicsView
{
    public static readonly BindableProperty ActivityDataProperty =
        BindableProperty.Create(nameof(ActivityData), typeof(Dictionary<DateTime, int>), typeof(StreakCalendarView), null,
            propertyChanged: (b, _, __) => ((StreakCalendarView)b).Invalidate());

    public static readonly BindableProperty WeeksProperty =
        BindableProperty.Create(nameof(Weeks), typeof(int), typeof(StreakCalendarView), 17,
            propertyChanged: (b, _, __) => ((StreakCalendarView)b).Invalidate());

    public Dictionary<DateTime, int>? ActivityData
    {
        get => (Dictionary<DateTime, int>?)GetValue(ActivityDataProperty);
        set => SetValue(ActivityDataProperty, value);
    }

    public int Weeks
    {
        get => (int)GetValue(WeeksProperty);
        set => SetValue(WeeksProperty, value);
    }

    public StreakCalendarView()
    {
        Drawable = new CalendarDrawable(this);
        HeightRequest = 90;
    }

    class CalendarDrawable : IDrawable
    {
        private readonly StreakCalendarView _view;
        public CalendarDrawable(StreakCalendarView view) => _view = view;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var data = _view.ActivityData;
            int weeks = _view.Weeks;
            float cellSize = (dirtyRect.Width - 4) / weeks;
            float gap = 2;
            float cell = cellSize - gap;

            var today = DateTime.Today;
            var startDate = today.AddDays(-(weeks * 7 - 1));

            for (int w = 0; w < weeks; w++)
            {
                for (int d = 0; d < 7; d++)
                {
                    var date = startDate.AddDays(w * 7 + d);
                    if (date > today) continue;

                    float x = w * cellSize;
                    float y = d * (cell + gap);

                    int minutes = data != null && data.TryGetValue(date, out var m) ? m : 0;
                    var color = GetColor(minutes);

                    canvas.FillColor = color;
                    canvas.FillRoundedRectangle(x, y, cell, cell, 2);
                }
            }
        }

        static Color GetColor(int minutes) => minutes switch
        {
            0      => Color.FromArgb("#FFF7ED"),
            <= 15  => Color.FromArgb("#FED7AA"),
            <= 30  => Color.FromArgb("#FB923C"),
            <= 60  => Color.FromArgb("#F97316"),
            _      => Color.FromArgb("#EA580C")
        };
    }
}
