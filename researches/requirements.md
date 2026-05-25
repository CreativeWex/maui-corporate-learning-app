# Технический стек — CreditApp

## Платформа и язык

| Параметр | Значение |
|---|---|
| **Платформа** | .NET MAUI (Multi-platform App UI) |
| **Язык** | C# 13 |
| **Target Framework** | `net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst` |
| **SDK** | .NET 10.0.103 |
| **Проект** | Single Project (один .csproj для всех платформ) |
| **Nullable** | enabled |
| **Implicit Usings** | enabled |

---

## NuGet-пакеты (прямые зависимости)

| Пакет | Версия | Назначение |
|---|---|---|
| `Microsoft.Maui.Controls` | `$(MauiVersion)` = **10.0.20** | UI-фреймворк |
| `CommunityToolkit.Mvvm` | **8.4.2** | MVVM: `ObservableObject`, `[ObservableProperty]`, `[RelayCommand]` |
| `sqlite-net-pcl` | **1.9.172** | ORM для SQLite (`SQLiteAsyncConnection`, атрибуты `[Table]`, `[PrimaryKey]`) |
| `SQLitePCLRaw.bundle_green` | **2.1.11** | Нативные биндинги SQLite (нужен в паре с sqlite-net-pcl) |
| `Microsoft.Extensions.Logging.Debug` | **10.0.0** | Debug-логгер для отладки |

---

## Архитектура приложения

**Паттерн:** MVVM + Service Layer + Repository

```
Views (XAML + code-behind)
    └─► ViewModels (CommunityToolkit.Mvvm)
            └─► Services (интерфейсы + реализации)
                    └─► Infrastructure
                            ├─ IApiClient / MockApiClient  (имитация сервера)
                            └─ ILocalRepository / LocalRepository (SQLite)
```

**DI-контейнер:** встроенный `Microsoft.Extensions.DependencyInjection` (регистрация в `MauiProgram.cs`)
- Сервисы — `AddSingleton`
- ViewModels и Views — `AddTransient`

**Навигация:** Shell (`AppShell.xaml`), маршруты через `Shell.GoToAsync()`

**XAML Inflator:** `MauiXamlInflator=SourceGen` — генерация C# из XAML во время компиляции (быстрее Runtime-режима)

---

## Структура проекта

```
CreditApp/
├── Models/              — доменные модели (CreditContract, Borrower и т.д.)
├── ViewModels/          — 13 VM, наследуют BaseViewModel : ObservableObject
├── Views/               — 13 страниц (.xaml + .xaml.cs)
├── Services/            — 7 сервисов (каждый: интерфейс + реализация)
├── Infrastructure/      — MockApiClient, LocalRepository, ContractEntity
├── Converters/          — 6 IValueConverter
├── Resources/
│   ├── Fonts/           — OpenSans-Regular.ttf, OpenSans-Semibold.ttf
│   ├── Styles/          — Colors.xaml, Styles.xaml
│   └── Splash/AppIcon/  — SVG
└── Platforms/Android, iOS, MacCatalyst, Windows/
```

---

## IDE

| Параметр | Значение |
|---|---|
| **IDE** | JetBrains Rider **2025.3.2** |
| **OS разработки** | macOS (Darwin 24.2.0, Apple Silicon) |
| **Конфиг проекта** | `.idea/` (Rider-формат) |

---

## Эмулятор Android

| Параметр | Значение |
|---|---|
| **AVD Name** | Pixel 7 |
| **Android API** | **34** (Android 14) |
| **System Image** | `google_apis_playstore / arm64-v8a` |
| **ABI** | `arm64-v8a` |
| **CPU** | `arm64`, 4 ядра |
| **RAM** | 2048 MB |
| **Heap** | 228 MB |
| **Разрешение экрана** | 1080 × 2400 px |
| **DPI** | 420 (xxhdpi) |
| **Ориентация** | Portrait |
| **GPU** | Enabled, mode = auto |
| **Play Store** | Включён |
| **Данные** | 6 GB data partition, 512 MB SD Card |
| **Кожа** | `pixel_7` (с рамкой устройства) |
| **Сеть** | latency=none, speed=full |
| **Android SDK** | `~/Library/Android/sdk/` |
| **Установленные platform-levels** | android-36, android-36.1 |

---

## Минимальные поддерживаемые версии ОС

| Платформа | Min версия |
|---|---|
| Android | **21** (Android 5.0) |
| iOS | **15.0** |
| macCatalyst | **15.0** |
| Windows | **10.0.17763** (1809) |

---

## Конфигурация .csproj для воспроизведения

```xml
<TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst</TargetFrameworks>
<UseMaui>true</UseMaui>
<MauiXamlInflator>SourceGen</MauiXamlInflator>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>

<PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.2" />
<PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.0" />
<PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
<PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.1.11" />
```
