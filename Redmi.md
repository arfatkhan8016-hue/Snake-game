# 🐍 Snake Game for Android (C# / .NET MAUI)

A touch-controlled Snake game for Android built with **C# and .NET MAUI**.
Swipe to move, tap to restart. Auto-built into an **APK** via GitHub Actions.

## 🎮 Controls

| Action | Gesture |
|--------|---------|
| Move   | Swipe on the game board OR tap D-pad buttons |
| Pause  | ⏸ button |
| Restart| "Restart" button (top-right) or tap when game over |

## 📥 Download the APK

1. Go to **Actions** tab on GitHub
2. Click the latest successful **Build Android APK** run
3. Download **`snake-game-android-apk`** artifact
4. Enable *"Install unknown apps"* on your Android device
5. Install and play!

## 🛠️ Local Development

**Prerequisites:** .NET 8 SDK + MAUI workload

```bash
dotnet workload install maui-android
cd SnakeGameAndroid
dotnet build -f net8.0-android
