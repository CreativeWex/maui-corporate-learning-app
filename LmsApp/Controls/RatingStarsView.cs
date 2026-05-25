namespace LmsApp.Controls;

public class RatingStarsView : GraphicsView
{
    public static readonly BindableProperty RatingProperty =
        BindableProperty.Create(nameof(Rating), typeof(double), typeof(RatingStarsView), 0.0,
            propertyChanged: (b, _, __) => ((RatingStarsView)b).Invalidate());

    public static readonly BindableProperty StarCountProperty =
        BindableProperty.Create(nameof(StarCount), typeof(int), typeof(RatingStarsView), 5,
            propertyChanged: (b, _, __) => ((RatingStarsView)b).Invalidate());

    public double Rating
    {
        get => (double)GetValue(RatingProperty);
        set => SetValue(RatingProperty, value);
    }

    public int StarCount
    {
        get => (int)GetValue(StarCountProperty);
        set => SetValue(StarCountProperty, value);
    }

    public RatingStarsView()
    {
        Drawable = new StarsDrawable(this);
        HeightRequest = 24;
        WidthRequest = 120;
    }

    class StarsDrawable : IDrawable
    {
        private readonly RatingStarsView _view;
        public StarsDrawable(RatingStarsView view) => _view = view;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float starSize = dirtyRect.Height * 0.8f;
            float gap = 2;
            var filledColor = Color.FromArgb("#F59E0B");
            var emptyColor = Color.FromArgb("#D1D5DB");

            for (int i = 0; i < _view.StarCount; i++)
            {
                float x = i * (starSize + gap);
                float filled = (float)Math.Clamp(_view.Rating - i, 0, 1);

                canvas.FontSize = starSize;
                canvas.FontColor = filled > 0.5 ? filledColor : emptyColor;
                canvas.DrawString("★", x, 0, starSize, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Top);
            }
        }
    }
}
