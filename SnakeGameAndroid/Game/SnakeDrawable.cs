using Microsoft.Maui.Graphics;

namespace SnakeGameAndroid.Game;

public class SnakeDrawable : IDrawable
{
    public GameEngine? Engine { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Engine == null) return;

        float cell = Math.Min(dirtyRect.Width / Engine.Width, dirtyRect.Height / Engine.Height);
        float offsetX = (dirtyRect.Width - cell * Engine.Width) / 2;
        float offsetY = (dirtyRect.Height - cell * Engine.Height) / 2;

        // Background grid
        canvas.FillColor = Color.FromArgb("#0d1b2a");
        canvas.FillRectangle(dirtyRect);

        // Board background
        canvas.FillColor = Color.FromArgb("#1b263b");
        canvas.FillRectangle(offsetX, offsetY, cell * Engine.Width, cell * Engine.Height);

        // Grid lines
        canvas.StrokeColor = Color.FromArgb("#2a3d5c");
        canvas.StrokeSize = 0.5f;
        for (int i = 0; i <= Engine.Width; i++)
            canvas.DrawLine(offsetX + i * cell, offsetY,
                            offsetX + i * cell, offsetY + cell * Engine.Height);
        for (int j = 0; j <= Engine.Height; j++)
            canvas.DrawLine(offsetX, offsetY + j * cell,
                            offsetX + cell * Engine.Width, offsetY + j * cell);

        // Food
        canvas.FillColor = Color.FromArgb("#e63946");
        canvas.FillEllipse(offsetX + Engine.Food.X * cell + cell * 0.15f,
                           offsetY + Engine.Food.Y * cell + cell * 0.15f,
                           cell * 0.7f, cell * 0.7f);

        // Snake
        bool isHead = true;
        foreach (var seg in Engine.Snake)
        {
            canvas.FillColor = isHead ? Color.FromArgb("#ffd60a") : Color.FromArgb("#2dc653");
            float pad = isHead ? 0.5f : 1.5f;
            canvas.FillRoundedRectangle(
                offsetX + seg.X * cell + pad,
                offsetY + seg.Y * cell + pad,
                cell - pad * 2,
                cell - pad * 2,
                cell * 0.25f);
            isHead = false;
        }

        // Game over overlay
        if (Engine.GameOver)
        {
            canvas.FillColor = Color.FromRgba(0, 0, 0, 180);
            canvas.FillRectangle(dirtyRect);

            canvas.FontColor = Colors.White;
            canvas.FontSize = 36;
            canvas.DrawString("GAME OVER",
                dirtyRect.X, dirtyRect.Y + dirtyRect.Height / 2 - 60,
                dirtyRect.Width, 60,
                HorizontalAlignment.Center, VerticalAlignment.Center);

            canvas.FontSize = 22;
            canvas.DrawString($"Score: {Engine.Score}",
                dirtyRect.X, dirtyRect.Y + dirtyRect.Height / 2,
                dirtyRect.Width, 40,
                HorizontalAlignment.Center, VerticalAlignment.Center);

            canvas.FontSize = 16;
            canvas.DrawString("Tap to restart",
                dirtyRect.X, dirtyRect.Y + dirtyRect.Height / 2 + 60,
                dirtyRect.Width, 30,
                HorizontalAlignment.Center, VerticalAlignment.Center);
        }
    }
}
