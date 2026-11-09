namespace SnakeGameAndroid.Game;

public enum Direction { Up, Down, Left, Right }

public class GameEngine
{
    public int Width { get; }
    public int Height { get; }
    public int Score { get; private set; }
    public bool GameOver { get; private set; }

    private readonly LinkedList<Position> _snake = new();
    private readonly Random _random = new();
    private Direction _direction = Direction.Right;
    private Position _food;

    public IReadOnlyCollection<Position> Snake => _snake;
    public Position Food => _food;
    public Direction CurrentDirection => _direction;

    public GameEngine(int width = 20, int height = 25)
    {
        Width = width;
        Height = height;
        Reset();
    }

    public void Reset()
    {
        _snake.Clear();
        int sx = Width / 2, sy = Height / 2;
        _snake.AddLast(new Position(sx, sy));
        _snake.AddLast(new Position(sx - 1, sy));
        _snake.AddLast(new Position(sx - 2, sy));
        _direction = Direction.Right;
        Score = 0;
        GameOver = false;
        PlaceFood();
    }

    private void PlaceFood()
    {
        do
        {
            _food = new Position(_random.Next(Width), _random.Next(Height));
        } while (_snake.Contains(_food));
    }

    public void ChangeDirection(Direction dir)
    {
        if ((_direction == Direction.Up && dir == Direction.Down) ||
            (_direction == Direction.Down && dir == Direction.Up) ||
            (_direction == Direction.Left && dir == Direction.Right) ||
            (_direction == Direction.Right && dir == Direction.Left))
            return;
        _direction = dir;
    }

    public void Tick()
    {
        if (GameOver) return;

        var head = _snake.First!.Value;
        var next = _direction switch
        {
            Direction.Up    => new Position(head.X, head.Y - 1),
            Direction.Down  => new Position(head.X, head.Y + 1),
            Direction.Left  => new Position(head.X - 1, head.Y),
            Direction.Right => new Position(head.X + 1, head.Y),
            _ => head
        };

        // Wall collision
        if (next.X < 0 || next.X >= Width || next.Y < 0 || next.Y >= Height)
        {
            GameOver = true;
            return;
        }

        // Self collision (exclude tail — it will move)
        var tail = _snake.Last!.Value;
        foreach (var part in _snake)
        {
            if (part.Equals(next) && !part.Equals(tail))
            {
                GameOver = true;
                return;
            }
        }

        _snake.AddFirst(next);

        if (next.Equals(_food))
        {
            Score += 10;
            PlaceFood();
        }
        else
        {
            _snake.RemoveLast();
        }
    }
}
