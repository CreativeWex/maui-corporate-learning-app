namespace LmsApp.Controls;

public class ConfettiView : GraphicsView
{
    private List<ConfettiParticle> _particles = new();
    private IDispatcherTimer? _timer;
    private bool _isRunning;

    public void Start()
    {
        if (_isRunning) return;
        _isRunning = true;
        _particles = Enumerable.Range(0, 60).Select(_ => new ConfettiParticle()).ToList();
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(16);
        _timer.Tick += (_, _) => { Update(); Invalidate(); };
        _timer.Start();
        Drawable = new ConfettiDrawable(_particles);
    }

    public void Stop()
    {
        _timer?.Stop();
        _isRunning = false;
        _particles.Clear();
        Invalidate();
    }

    void Update()
    {
        foreach (var p in _particles)
            p.Update();
        if (_particles.All(p => p.Y > 800))
            Stop();
    }

    class ConfettiDrawable : IDrawable
    {
        private readonly List<ConfettiParticle> _particles;
        public ConfettiDrawable(List<ConfettiParticle> particles) => _particles = particles;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            foreach (var p in _particles)
            {
                canvas.FillColor = p.Color;
                canvas.FillRectangle(p.X, p.Y, p.Size, p.Size);
            }
        }
    }

    class ConfettiParticle
    {
        private static readonly Random Rng = new();
        private static readonly Color[] Colors =
        [
            Color.FromArgb("#4F46E5"), Color.FromArgb("#10B981"),
            Color.FromArgb("#F59E0B"), Color.FromArgb("#EF4444"),
            Color.FromArgb("#7C3AED"), Color.FromArgb("#3B82F6")
        ];

        public float X = Rng.NextSingle() * 400;
        public float Y = -Rng.NextSingle() * 200;
        public float Size = Rng.Next(4, 10);
        public Color Color = Colors[Rng.Next(Colors.Length)];
        private float _vx = (Rng.NextSingle() - 0.5f) * 3;
        private float _vy = Rng.NextSingle() * 4 + 2;
        private float _rot;
        private float _rotSpeed = (Rng.NextSingle() - 0.5f) * 10;

        public void Update()
        {
            X += _vx;
            Y += _vy;
            _rot += _rotSpeed;
            _vy += 0.05f;
        }
    }
}
