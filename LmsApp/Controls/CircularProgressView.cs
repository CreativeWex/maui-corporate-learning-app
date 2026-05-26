namespace LmsApp.Controls;

public class CircularProgressView : GraphicsView
{
    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(nameof(Progress), typeof(double), typeof(CircularProgressView), 0.0,
            propertyChanged: (b, _, __) => ((CircularProgressView)b).Invalidate());

    public static readonly BindableProperty StrokeWidthProperty =
        BindableProperty.Create(nameof(StrokeWidth), typeof(float), typeof(CircularProgressView), 12f,
            propertyChanged: (b, _, __) => ((CircularProgressView)b).Invalidate());

    public static readonly BindableProperty TrackColorProperty =
        BindableProperty.Create(nameof(TrackColor), typeof(Color), typeof(CircularProgressView), Color.FromArgb("#E5E7EB"),
            propertyChanged: (b, _, __) => ((CircularProgressView)b).Invalidate());

    public static readonly BindableProperty ProgressColorProperty =
        BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(CircularProgressView), Color.FromArgb("#F97316"),
            propertyChanged: (b, _, __) => ((CircularProgressView)b).Invalidate());

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(CircularProgressView), string.Empty,
            propertyChanged: (b, _, __) => ((CircularProgressView)b).Invalidate());

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }
    public float StrokeWidth
    {
        get => (float)GetValue(StrokeWidthProperty);
        set => SetValue(StrokeWidthProperty, value);
    }
    public Color TrackColor
    {
        get => (Color)GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }
    public Color ProgressColor
    {
        get => (Color)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public CircularProgressView()
    {
        Drawable = new CircularDrawable(this);
        HeightRequest = 120;
        WidthRequest = 120;
    }

    class CircularDrawable : IDrawable
    {
        private readonly CircularProgressView _view;
        public CircularDrawable(CircularProgressView view) => _view = view;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float cx = dirtyRect.Width / 2;
            float cy = dirtyRect.Height / 2;
            float radius = Math.Min(cx, cy) - _view.StrokeWidth;

            // Track
            canvas.StrokeColor = _view.TrackColor;
            canvas.StrokeSize = _view.StrokeWidth;
            canvas.DrawEllipse(cx - radius, cy - radius, radius * 2, radius * 2);

            // Progress arc
            if (_view.Progress > 0)
            {
                canvas.StrokeColor = _view.ProgressColor;
                canvas.StrokeSize = _view.StrokeWidth;
                canvas.StrokeLineCap = LineCap.Round;
                float sweep = (float)(_view.Progress / 100.0 * 360.0);
                canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2, 90, 90 - sweep, false, false);
            }

            // Text
            if (!string.IsNullOrEmpty(_view.Text))
            {
                canvas.FontSize = 20;
                canvas.FontColor = _view.ProgressColor;
                canvas.DrawString(_view.Text, dirtyRect, HorizontalAlignment.Center, VerticalAlignment.Center);
            }
        }
    }
}
