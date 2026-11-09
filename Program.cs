using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeGame
{
    public enum Direction { Up, Down, Left, Right }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y) { X = x; Y = y; }

        public override bool Equals(object? obj)
            => obj is Position p && p.X == X && p.Y == Y;

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }

    public class Game
    {
        private readonly int _width;
        private readonly int _height;
        private readonly List<Position> _snake;
        private Position _food;
        private Direction _direction;
        private int _score;
        private readonly Random _random = new();
        private bool _gameOver;

        public int Score => _score;
        public bool GameOver => _gameOver;

        public Game(int width = 30, int height = 20)
        {
            _width = width;
            _height = height;
            _snake = new List<Position>();
            Reset();
        }

        public void Reset()
        {
            _snake.Clear();
            int startX = _width / 2;
            int startY = _height / 2;
            _snake.Add(new Position(startX, startY));
            _snake.Add(new Position(startX - 1, startY));
            _snake.Add(new Position(startX - 2, startY));
            _direction = Direction.Right;
            _score = 0;
            _gameOver = false;
            PlaceFood();
        }

        private void PlaceFood()
        {
            do
            {
                _food = new Position(_random.Next(0, _width), _random.Next(0, _height));
            } while (_snake.Contains(_food));
        }

        public void ChangeDirection(Direction newDir)
        {
            // Prevent reversing into itself
            if ((_direction == Direction.Up && newDir == Direction.Down) ||
                (_direction == Direction.Down && newDir == Direction.Up) ||
                (_direction == Direction.Left && newDir == Direction.Right) ||
                (_direction == Direction.Right && newDir == Direction.Left))
                return;

            _direction = newDir;
        }

        public void Tick()
        {
            if (_gameOver) return;

            var head = _snake[0];
            var newHead = new Position(head.X, head.Y);

            switch (_direction)
            {
                case Direction.Up: newHead.Y--; break;
                case Direction.Down: newHead.Y++; break;
                case Direction.Left: newHead.X--; break;
                case Direction.Right: newHead.X++; break;
            }

            // Wall collision
            if (newHead.X < 0 || newHead.X >= _width ||
                newHead.Y < 0 || newHead.Y >= _height)
            {
                _gameOver = true;
                return;
            }

            // Self collision (ignore tail since it will move)
            for (int i = 0; i < _snake.Count - 1; i++)
            {
                if (_snake[i].Equals(newHead))
                {
                    _gameOver = true;
                    return;
                }
            }

            _snake.Insert(0, newHead);

            if (newHead.Equals(_food))
            {
                _score += 10;
                PlaceFood();
            }
            else
            {
                _snake.RemoveAt(_snake.Count - 1);
            }
        }

        public void Render()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($" Score: {_score}   ");
            Console.WriteLine("+" + new string('-', _width) + "+");

            for (int y = 0; y < _height; y++)
            {
                Console.Write("|");
                for (int x = 0; x < _width; x++)
                {
                    var pos = new Position(x, y);
                    if (_snake.Count > 0 && _snake[0].Equals(pos))
                        Console.Write("O");
                    else if (_snake.Contains(pos))
                        Console.Write("o");
                    else if (_food.Equals(pos))
                        Console.Write("*");
                    else
                        Console.Write(" ");
                }
                Console.WriteLine("|");
            }

            Console.WriteLine("+" + new string('-', _width) + "+");
        }

        public List<Position> GetSnake() => _snake;
        public Position GetFood() => _food;
        public Direction GetDirection() => _direction;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // Non-interactive mode for CI/testing
            if (args.Length > 0 && args[0] == "--test")
            {
                RunTests();
                return;
            }

            RunGame();
        }

        private static void RunGame()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;
            Console.Clear();

            var game = new Game();
            int delay = 120;

            while (!game.GameOver)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow: game.ChangeDirection(Direction.Up); break;
                        case ConsoleKey.DownArrow: game.ChangeDirection(Direction.Down); break;
                        case ConsoleKey.LeftArrow: game.ChangeDirection(Direction.Left); break;
                        case ConsoleKey.RightArrow: game.ChangeDirection(Direction.Right); break;
                        case ConsoleKey.Escape: return;
                    }
                }

                game.Tick();
                game.Render();
                Thread.Sleep(delay);
            }

            Console.SetCursorPosition(0, 25);
            Console.WriteLine($"\nGAME OVER! Final Score: {game.Score}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void RunTests()
        {
            Console.WriteLine("Running Snake Game Tests...");
            int passed = 0, failed = 0;

            void Assert(bool condition, string name)
            {
                if (condition) { Console.WriteLine($"  PASS: {name}"); passed++; }
                else { Console.WriteLine($"  FAIL: {name}"); failed++; }
            }

            // Test 1: Initial snake length
            var g1 = new Game(10, 10);
            Assert(g1.GetSnake().Count == 3, "Initial snake length is 3");

            // Test 2: Moving right
            var g2 = new Game(10, 10);
            var headBefore = g2.GetSnake()[0];
            g2.Tick();
            var headAfter = g2.GetSnake()[0];
            Assert(headAfter.X == headBefore.X + 1 && headAfter.Y == headBefore.Y,
                   "Snake moves right on tick");

            // Test 3: Direction change
            var g3 = new Game(10, 10);
            g3.ChangeDirection(Direction.Up);
            g3.Tick();
            Assert(g3.GetSnake()[0].Y < 5, "Snake moves up after direction change");

            // Test 4: Cannot reverse direction
            var g4 = new Game(10, 10);
            g4.ChangeDirection(Direction.Left);
            g4.Tick();
            Assert(g4.GetSnake()[0].X == 6, "Cannot reverse direction into itself");

            // Test 5: Wall collision
            var g5 = new Game(5, 5);
            for (int i = 0; i < 5; i++) g5.Tick();
            Assert(g5.GameOver, "Game over on wall collision");

            Console.WriteLine($"\nResults: {passed} passed, {failed} failed");
            Environment.Exit(failed == 0 ? 0 : 1);
        }
    }
}
