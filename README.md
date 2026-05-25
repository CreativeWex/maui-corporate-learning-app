# LmsApp

Краткая инструкция по запуску приложения.

## Шаг 1 — запустить эмулятор и дождаться полной загрузки Android

```bash
export PATH="$HOME/Library/Android/sdk/emulator:$HOME/Library/Android/sdk/platform-tools:$PATH"
emulator -avd Pixel_7 -no-snapshot-load & export PATH="$HOME/Library/Android/sdk/platform-tools:$PATH"
until [ "$(adb shell getprop sys.boot_completed 2>/dev/null | tr -d '\r')" = "1" ]; do
  echo "Ожидание загрузки эмулятора..."; sleep 3
done
echo "Эмулятор готов."
```

## Шаг 2 — собрать и установить приложение

```bash
export PATH="$HOME/.dotnet:$HOME/Library/Android/sdk/platform-tools:$PATH"
export JAVA_HOME="/Applications/Android Studio.app/Contents/jbr/Contents/Home"
export NUGET_HTTP_CACHE_PATH="$HOME/tmp/nuget-http-cache"
export NUGET_PACKAGES="$HOME/tmp/nuget-packages"

dotnet run -f net10.0-android \
  -p:AndroidSdkDirectory="$HOME/Library/Android/sdk"
```
