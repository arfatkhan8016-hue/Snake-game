using SnakeGameAndroid.Game;

namespace SnakeGameAndroid.Pages;

public partial class GamePage : ContentPage
{
    private readonly GameEngine _engine;
    private readonly SnakeDrawable _drawable;
    private IDispatcherTimer? _timer;
    private bool _paused;

    public GamePage()
    {
        InitializeComponent();

        _engine = new GameEngine(width: 20, height: 25);
        _drawable = new SnakeDrawable { Engine = _engine };
        GameCanvas.Drawable = _drawable;

        // Tap on canvas → restart when game over
        var tap = new TapGestureRecognizer();
        tap.Tapped += OnCanvasTapped;
        GameCanvas.GestureRecognizers.Add(tap);

        // Swipe gestures on canvas
        var swipe = new SwipeGestureRecognizer { Direction = SwipeDirection.Up };
        swipe.Swiped += (_, _) => _engine.ChangeDirection(Direction.Up);
        GameCanvas.GestureRecognizers.Add(swipe);

        var swipeDown = new SwipeGestureRecognizer { Direction = SwipeDirection.Down };
        swipeDown.Swiped += (_, _) => _engine.ChangeDirection(Direction.Down);
        GameCanvas.GestureRecognizers.Add(swipeDown);

        var swipeLeft = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };
        swipeLeft.Swiped += (_, _) => _engine.ChangeDirection(Direction.Left);
        GameCanvas.GestureRecognizers.Add(swipeLeft);

        var swipeRight = new SwipeGestureRecognizer { Direction = SwipeDirection.Right };
        swipeRight.Swiped += (_, _) => _engine.ChangeDirection(Direction.Right);
        GameCanvas.GestureRecognizers.Add(swipeRight);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartTimer();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timer?.Stop();
    }

    private void StartTimer()
    {
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(150);
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (_paused) return;

        _engine.Tick();
        ScoreLabel.Text = $"Score: {_engine.Score}";
        GameCanvas.Invalidate();
    }

    private void OnUpClicked(object sender, EventArgs e)    => _engine.ChangeDirection(Direction.Up);
    private void OnDownClicked(object sender, EventArgs e)  => _engine.ChangeDirection(Direction.Down);
    private void OnLeftClicked(object sender, EventArgs e)  => _engine.ChangeDirection(Direction.Left);
    private void OnRightClicked(object sender, EventArgs e) => _engine.ChangeDirection(Direction.Right);

    private void OnPauseClicked(object sender, EventArgs e)
    {
        _paused = !_paused;
    }

    private void OnRestartClicked(object sender, EventArgs e) => Restart();

    private void OnCanvasTapped(object sender, TappedEventArgs e)
    {
        if (_engine.GameOver) Restart();
    }

    private void Restart()
    {
        _engine.Reset();
        _paused = false;
        ScoreLabel.Text = "Score: 0";
        GameCanvas.Invalidate();
    }
}
