# Базовый план реализации: Корпоративная LMS на .NET MAUI

> Документ — детальная инженерная спецификация для реализации приложения из `researches/business_plan.md` на стеке `requirements.md`. Код в этом документе приведён только на уровне сигнатур/полей как спецификация; полная имплементация — на этапе разработки.

---

## 0. Контекст и зафиксированные решения

`researches/business_plan.md` — функциональная спецификация: корпоративная LMS, 15 экранов, 4 роли (Сотрудник / Менеджер / HR-L&D / Администратор), полный цикл «назначение → обучение → тест → сертификат» + геймификация.

`requirements.md` (озаглавлен «CreditApp») — **референс стека и архитектуры**, а не функциональности. Стек берём оттуда, функциональность — из бизнес-плана.

**Текущее состояние репозитория:** пустой проект (только `requirements.md` и `researches/business_plan.md`), не git-репозиторий. Реализация — полностью с нуля.

**Согласованные решения:**
1. **Объём:** все 15 экранов, 4 роли.
2. **Зависимости:** разрешено добавлять проверенные NuGet-пакеты сверх 5 базовых.
3. **Локализация:** только русский; переключатель языка в профиле декоративный (без RESX).
4. **Бэкенд отсутствует:** сервер имитируется `MockApiClient`, данные хранятся локально в SQLite (`LocalRepository`). Все данные — сид + локальная персистентность.
5. **Основная цель проверки:** Android-эмулятор Pixel 7, API 34 (из requirements.md).

---

## 1. Технический стек

### 1.1 Платформа
- `.NET 10` (SDK 10.0.103), C# 13
- TFM: `net10.0-android;net10.0-ios;net10.0-maccatalyst`
- Single Project, `Nullable=enable`, `ImplicitUsings=enable`, `MauiXamlInflator=SourceGen`
- Min OS: Android 21, iOS 15, macCatalyst 15

### 1.2 NuGet-пакеты

**Базовые (requirements.md):**
| Пакет | Версия |
|---|---|
| `Microsoft.Maui.Controls` | `$(MauiVersion)` = 10.0.20 |
| `CommunityToolkit.Mvvm` | 8.4.2 |
| `sqlite-net-pcl` | 1.9.172 |
| `SQLitePCLRaw.bundle_green` | 2.1.11 |
| `Microsoft.Extensions.Logging.Debug` | 10.0.0 |

**Добавляемые (под heavy-фичи):**
| Пакет | Назначение | Где используется |
|---|---|---|
| `CommunityToolkit.Maui` | Popup/BottomSheet, поведения, анимации, доп. конвертеры | Achievements, Leaderboard, TeamDashboard |
| `Markdig` | Markdown → HTML | LessonPage (статьи) |
| `Microcharts.Maui` | графики Bar/Line | MyProgress, TeamDashboard |
| `QuestPDF` | генерация PDF (Community-лицензия, на SkiaSharp) | CertificatesPage |
| `QRCoder` | генерация QR-кода (PNG-байты) для PDF | CertificatesPage |

> **Fallback-политика:** если любой добавляемый пакет несовместим с MAUI 10 при сборке — заменяем на ручную реализацию (`GraphicsView` для графиков/QR, `WebView` для Markdown, упрощённый PDF-шаблон). Heavy-фичи не блокируют остальные экраны.

### 1.3 Эталонный фрагмент .csproj
```xml
<TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst</TargetFrameworks>
<UseMaui>true</UseMaui>
<MauiXamlInflator>SourceGen</MauiXamlInflator>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
<ApplicationTitle>LmsApp</ApplicationTitle>
<!-- PackageReference: 5 базовых + 5 добавляемых -->
```

---

## 2. Архитектура

```
Views (XAML + code-behind, [QueryProperty] для параметров)
  └─► ViewModels (CommunityToolkit.Mvvm, : BaseViewModel)
        └─► Services (интерфейс + Mock-реализация, бизнес-логика)
              └─► Infrastructure
                    ├─ IApiClient / MockApiClient   (имитация сервера + SeedData)
                    └─ ILocalRepository / LocalRepository (SQLite, async)
```

- **MVVM:** `ObservableObject`, `[ObservableProperty]`, `[RelayCommand]`. Никакой бизнес-логики в code-behind (только UI-specifics: анимации, lifecycle).
- **DI:** `Microsoft.Extensions.DependencyInjection` в `MauiProgram.cs`. Сервисы — `AddSingleton`, ViewModels и Views — `AddTransient`.
- **Навигация:** Shell + `Shell.GoToAsync("route?id=X")`, параметры через `[QueryProperty]`.
- **Сессия:** `ISessionService` (Singleton) хранит текущего `User`/роль; токен — `SecureStorage`; настройки приложения — `Preferences`.
- **Ошибки:** единый `try/catch` в `BaseViewModel.RunSafeAsync`, показ через `IDialogService`/Snackbar.

### 2.1 Полная структура проекта
```
LmsApp/
├── App.xaml / App.xaml.cs
├── AppShell.xaml / AppShell.xaml.cs        — динамические табы по роли
├── MauiProgram.cs                          — регистрация DI
├── Models/
│   ├── Domain/    (20 моделей — §4)
│   ├── Dto/       (LoginRequest, AuthResponse, AssignmentRequest, …)
│   └── Enums/     (UserRole, ModuleType, ModuleStatus, QuestionType, ContentType)
├── ViewModels/
│   ├── BaseViewModel.cs
│   └── (15 VM — §7)
├── Views/         (15 .xaml + .xaml.cs — §7)
├── Services/
│   ├── Interfaces/      (11 сервисов §6 БП + ISessionService + IDialogService)
│   └── Implementations/ (Mock*-реализации)
├── Infrastructure/
│   ├── IApiClient.cs / MockApiClient.cs
│   ├── SeedData.cs
│   ├── ILocalRepository.cs / LocalRepository.cs
│   └── Entities/        (*Entity для SQLite)
├── Controls/      (CircularProgressView, StreakCalendarView, ConfettiView, RatingStarsView)
├── Converters/    (§9)
├── Helpers/       (EmailValidator, ColorThemeManager, …)
└── Resources/
    ├── Styles/    (Colors.xaml, Styles.xaml)
    ├── Fonts/     (OpenSans-Regular.ttf, OpenSans-Semibold.ttf)
    ├── Images/    (иконки табов, обложки-заглушки, бейджи)
    └── Splash/AppIcon (SVG)
```

---

## 3. Перечисления (Enums)

| Enum | Значения |
|---|---|
| `UserRole` | `Employee`, `Manager`, `Hr`, `Admin` |
| `ModuleType` / `ContentType` | `Article`, `Flashcards`, `Quiz`, `Infographic`, `MindMap`, `Image` |
| `ModuleStatus` | `NotStarted`, `InProgress`, `Completed`, `Locked` |
| `QuestionType` | `SingleChoice`, `MultipleChoice`, `TrueFalse` |
| `LeaderboardScope` | `Team`, `Company` |
| `StatPeriod` | `Week`, `Month`, `Quarter`, `AllTime` |

---

## 4. Доменные модели (§5 БП) — поля

| Модель | Ключевые поля |
|---|---|
| `User` | Id, Name, Email, Role, Department, Position, AvatarUrl |
| `UserProfile` | Id, Name, Position, Department, Email, AvatarUrl, Level, TotalXp |
| `AuthToken` | Token, ExpiresAt, UserId |
| `Course` | Id, Title, Category, CoverUrl, AuthorName, DurationMinutes, Rating, IsAssigned, DeadlineDate?, ProgressPercent |
| `CourseDetail` | : Course + Description, Modules[] |
| `Module` | Id, Title, Type(ModuleType), DurationMin, Status(ModuleStatus), OrderIndex |
| `Lesson` | Id, Title, Type, Content(string/blob), DurationMin |
| `Quiz` | Id, Title, TimeLimit?, IsFinal, MaxAttempts, Questions[] |
| `Question` | Id, Text, ImageUrl?, Type(QuestionType), Options[], Explanation |
| `Answer` | QuestionId, SelectedOptionIds[], IsCorrect |
| `QuizResult` | QuizId, Score, PassedPercent, Passed, AttemptNumber, AnswerDetails[] |
| `Assignment` | Id, CourseId, UserId, AssignedById, DeadlineDate, AssignedAt, IsMandatory |
| `Certificate` | Id, CourseId, CourseName, IssuedAt, PdfPath, QrCode |
| `Achievement` | Id, Name, Description, IconUrl, XpReward, UnlockedAt?, Condition |
| `UserLevel` | Level, LevelName, CurrentXp, XpToNext |
| `LeaderboardEntry` | Rank, UserId, DisplayName, AvatarUrl, Department, TotalXp, BadgeCount, IsCurrentUser |
| `LearningSession` | LessonId, StartedAt, CompletedAt?, TimeSpentSec |
| `ProgressStats` | Period, CompletedCourses, TotalHours, CurrentStreak, MaxStreak, Rank |
| `DailyActivity` | Date, MinutesSpent |
| `TeamMember` | UserId, Name, AvatarUrl, AssignedCourses, CompletedCourses, OverallProgress, IsAtRisk |
| `TeamStats` | TotalMembers, AvgProgress, AssignedCount, OverdueCount |
| `UserStats` | CompletedCourses, TotalHours, Badges, LeaderboardRank |
| `AppSettings` | Language, Theme, NotificationPreferences |

**DTO:** `LoginRequest{Email,Password}`, `AuthResponse{Token,ExpiresAt,User}`, `AssignmentRequest{CourseId,RecipientIds[],DeadlineDate,IsMandatory,ReminderDays[],Message}`.

---

## 5. Инфраструктура (данные)

### 5.1 Entities (SQLite, sqlite-net-pcl)
`Entities/*Entity` с `[Table]`, `[PrimaryKey]`, `[Indexed]`. Минимум: `CourseEntity`, `ModuleEntity`, `LessonEntity` (Content как blob), `QuizEntity`, `QuestionEntity`, `QuizResultEntity`, `AssignmentEntity`, `CertificateEntity` (Pdf как blob), `AchievementEntity`, `LearningSessionEntity`, `UserEntity`, `LeaderboardEntryEntity`. Маппинг Entity↔Domain — в `LocalRepository` (или статические Mapper-методы).

### 5.2 LocalRepository
- `SQLiteAsyncConnection` (путь `FileSystem.AppDataDirectory/lms.db3`).
- `InitAsync()` — `CreateTablesAsync` для всех Entity, проверка флага «засеяно» в `Preferences`, при первом запуске — загрузка `SeedData`.
- CRUD-методы по каждой сущности + специализированные запросы (курсы в процессе, назначения за 7 дней, сессии за период).

### 5.3 MockApiClient (IApiClient)
- Имитирует HTTP: методы возвращают `Task<T>` с `await Task.Delay(150..400)` для реалистичной латентности; может симулировать ошибки (флаг для тестов).
- На старте отдаёт `SeedData`; далее источник истины — `LocalRepository`.

### 5.4 SeedData
- **Пользователи (4 роли):** employee@corp / manager@corp / hr@corp / admin@corp (пароль `123456`).
- **Курсы:** 6–8 шт. разных категорий (IT, Soft Skills, Compliance, Менеджмент), у каждого 3–5 модулей (статья, карточки, инфографика, финальный тест), обложки-заглушки.
- **Назначения:** 2–3 курса назначены employee с разными дедлайнами (включая «горящий» <3 дней).
- **Уроки:** Markdown-статьи, наборы карточек, инфографика.
- **Тесты:** все три типа вопросов, с объяснениями.
- **Достижения:** 6+ бейджей (часть получена, часть заблокирована) с XP.
- **Рейтинг:** 10–15 записей с XP/бейджами для команды и компании.
- **Команда менеджера:** 5–7 `TeamMember`, у части — риск просрочки.
- **LearningSession:** история активности за ~30 дней для streak-календаря и графиков.

---

## 6. Сервисный слой (§6 БП) — интерфейсы и логика

Каждый: интерфейс в `Services/Interfaces/`, Mock-реализация в `Services/Implementations/`, работает через `IApiClient` + `ILocalRepository`.

| Сервис | Ключевые методы | Бизнес-логика |
|---|---|---|
| `IAuthService` | `LoginAsync(LoginRequest)`, `LogoutAsync()`, `RefreshTokenAsync()`, `GetSavedTokenAsync()` | проверка сид-учёток, токен в SecureStorage, блокировка после 5 попыток (15 мин) |
| `ISessionService` | `CurrentUser`, `Role`, `SetSession()`, `Clear()` | Singleton-состояние сессии |
| `ICourseService` | `GetCatalogAsync(filter,search)`, `GetCourseDetailAsync(id)`, `GetContinueLearningAsync()`, `GetNewAssignmentsAsync()` | сортировка назначенные→новые→популярные; debounce — на стороне VM |
| `ILessonService` | `GetLessonAsync(id)`, `MarkCompletedAsync(id)`, `SaveSessionAsync(LearningSession)` | фиксация прочтения, кэш контента в SQLite |
| `IQuizService` | `GetQuizAsync(id)`, `SubmitAnswerAsync()`, `CalculateResultAsync()` | рандомизация вариантов, проверка ответов, лимит попыток |
| `IProgressService` | `GetStatsAsync(period)`, `GetDailyActivityAsync(period)`, `GetStreakAsync()` | расчёт streak (дни подряд с ≥1 уроком), агрегация из LearningSession |
| `ICertificateService` | `GetCertificatesAsync()`, `GenerateAsync(courseId)`, `GetPdfAsync(id)` | QuestPDF + QRCoder, сохранение blob, генерация при ≥80% финала |
| `IGamificationService` | `GetAchievementsAsync()`, `GetUserLevelAsync()`, `GetLeaderboardAsync(scope,period)`, `AwardXpAsync()` | начисление XP (урок +10, тест +20..100, бейджи, streak), уровни |
| `IAssignmentService` | `AssignAsync(AssignmentRequest)`, `GetAssignmentsAsync(userId)` | валидация дедлайна (≥завтра), запрет дубля, журнал с инициатором |
| `ITeamService` | `GetTeamStatsAsync()`, `GetTeamMembersAsync()`, `GetAtRiskAsync()` | зона риска: прогресс <30% и дедлайн ≤3 дн |
| `INotificationService` | `SendReminderAsync(userId)`, `NotifyAssignmentAsync()` | мок-пуши, троттлинг «Напомнить» 1/24ч |
| `ISettingsService` | `GetSettings()`, `SetTheme()`, `SetNotificationPref()`, `SetAnonymous()` | Preferences, немедленное применение |
| `IDialogService` | `ShowAlert()`, `ShowConfirm()`, `ShowToast()` | обёртка над DisplayAlert/Snackbar |

---

## 7. Экраны — детальная спецификация

> Формат: **VM (свойства / команды) → ключевые UI-компоненты → бизнес-правила → навигация → файлы**.

### Экран 1. SplashPage — `SplashViewModel`
- **Свойства:** `Version`. **Команды:** `InitializeCommand` (OnAppearing).
- **UI:** логотип, название (H1), `ActivityIndicator`, версия снизу.
- **Логика:** `LocalRepository.InitAsync()` (+сид) → проверка токена в SecureStorage → мин. показ 1.5 c → `home`/`login`.
- **Навигация:** старт → `HomePage`/`LoginPage`.
- **Файлы:** `Views/SplashPage.xaml(.cs)`, `ViewModels/SplashViewModel.cs`.

### Экран 2. LoginPage — `LoginViewModel`
- **Свойства:** `Email`, `Password`, `IsBusy`, `ErrorMessage`, `AttemptsLeft`. **Команды:** `LoginCommand`, `SsoCommand`(заглушка), `ForgotPasswordCommand`(заглушка).
- **UI:** логотип, Entry Email (keyboard=Email), Entry Password (IsPassword), кнопка «Войти», ссылки SSO/«Забыли пароль?», `ActivityIndicator`.
- **Правила:** email по regex, пароль ≥6; 5 неудач → блок 15 мин; токен в SecureStorage; стартовый экран по роли.
- **Навигация:** → `HomePage` (Сотрудник) / `team-dashboard` (Менеджер) и т.п.
- **Файлы:** `Views/LoginPage.xaml(.cs)`, `ViewModels/LoginViewModel.cs`, `Helpers/EmailValidator.cs`.

### Экран 3. HomePage — `HomeViewModel`
- **Свойства:** `Greeting`, `StreakDays`, `ContinueLearning`(ObservableCollection<CourseProgress>), `NewAssignments`, `Stats`(UserStats). **Команды:** `LoadCommand`, `OpenCourseCommand(id)`, `RefreshCommand`.
- **UI:** приветствие, streak-плашка, горизонтальный `CollectionView` «Продолжить обучение» (карточка: обложка/название/ProgressBar/%), «Новые назначения» (2 карточки), статистика Grid 2×2.
- **Правила:** «Продолжить» — прогресс 0<p<100, сорт. по LastAccessedAt; «Новые» — AssignedAt ≤7 дн.
- **Навигация:** карточка → `course-detail?id=X`; табы.
- **Файлы:** `Views/HomePage.xaml(.cs)`, `ViewModels/HomeViewModel.cs`.

### Экран 4. CatalogPage — `CatalogViewModel`
- **Свойства:** `SearchText`, `SelectedFilter`, `Courses`, `IsGridView`, `IsEmpty`. **Команды:** `SearchCommand`(debounce 300мс), `SelectFilterCommand`, `ToggleViewCommand`, `OpenCourseCommand`.
- **UI:** `SearchBar`, чипы-фильтры (Все/Назначенные/IT/Soft Skills/Compliance/Менеджмент), переключатель плитка/список, `CollectionView` карточек (обложка, чип-категория, название, автор, длительность, рейтинг ⭐, прогресс-бар), пустое состояние.
- **Правила:** поиск по названию+описанию; бейдж «Назначен»+дедлайн; просрочка — красным; сорт. назначенные→новые→популярные.
- **Файлы:** `Views/CatalogPage.xaml(.cs)`, `ViewModels/CatalogViewModel.cs`.

### Экран 5. CourseDetailPage — `CourseDetailViewModel` (`[QueryProperty("CourseId","id")]`)
- **Свойства:** `Course`(CourseDetail), `Modules`, `OverallProgress`, `CtaText`, `Deadline`. **Команды:** `LoadCommand`, `StartContinueCommand`, `OpenModuleCommand(module)`.
- **UI:** hero-изображение (35%), кнопка «Назад», название H1, автор+аватар, метаданные Grid, описание (expandable), ProgressBar «X/Y модулей», `CollectionView` модулей (иконка типа/название/длительность/статус), CTA «Начать/Продолжить/Повторить» (фикс. снизу).
- **Правила:** последовательная разблокировка модулей; предупреждение при дедлайне <3 дн; CTA ведёт к первому незавершённому; после всех модулей — финальный тест.
- **Навигация:** → `lesson?id=X` / `quiz?id=X`; финал → `quiz-result`.
- **Файлы:** `Views/CourseDetailPage.xaml(.cs)`, `ViewModels/CourseDetailViewModel.cs`.

### Экран 6. LessonPage — `LessonViewModel` (`[QueryProperty]`)
- **Свойства:** `Lesson`, `Progress`(«Урок 3/8»), `CanGoNext`, `IsCompleted`, `ScrollPercent`. **Команды:** `LoadCommand`, `NextCommand`, `PreviousCommand`, `MarkCompletedCommand`.
- **UI по типу:** статья — `WebView` (Markdig→HTML); карточки — `CarouselView` (flip-анимация); инфографика — pinch-zoom `Image`; ментальная карта — `WebView`/`GraphicsView`. Линейный прогресс-бар сверху, кнопки навигации, чекбокс «Завершить».
- **Правила:** прочтение при скролле 80% или «Завершить»; карточки — перелистать все; запись `LearningSession` (время); оффлайн из SQLite-кэша; начисление XP +10.
- **Навигация:** «Следующий» → следующий `lesson`/`quiz`; «Назад» → `course-detail`.
- **Файлы:** `Views/LessonPage.xaml(.cs)`, `ViewModels/LessonViewModel.cs`.

### Экран 7. QuizPage — `QuizViewModel` (`[QueryProperty]`)
- **Свойства:** `Quiz`, `CurrentQuestion`, `QuestionIndex`, `SelectedOptions`, `TimeLeft`, `IsAnswered`, `CanSubmit`. **Команды:** `LoadCommand`, `SubmitAnswerCommand`, `NextCommand`, `SkipCommand`.
- **UI:** заголовок+курс, «Вопрос 4/10»+ProgressBar, опц. таймер, текст вопроса H3, опц. изображение; ответы: RadioButton (single) / CheckBox (multiple) / 2 кнопки (true/false); кнопка «Ответить/Далее», «Пропустить».
- **Правила:** по одному вопросу; «Ответить» активна после выбора; подсветка зелёный/красный + объяснение; рандомизация вариантов; лимит попыток финала (3).
- **Навигация:** после последнего → `quiz-result`; «Назад» → `course-detail` (предупреждение).
- **Файлы:** `Views/QuizPage.xaml(.cs)`, `ViewModels/QuizViewModel.cs`.

### Экран 8. QuizResultPage — `QuizResultViewModel` (`[QueryProperty]`)
- **Свойства:** `Result`(QuizResult), `Passed`, `ScorePercent`, `Correct/Wrong/Skipped/Time`, `AnswerDetails`, `CanRetry`, `CertificateIssued`. **Команды:** `ContinueCourseCommand`, `RetryCommand`, `GoToCertificatesCommand`.
- **UI:** анимированная иконка ✅/⚠️, `CircularProgressView` с баллами, текст итога H1, статистика Grid 2×2, разбор вопросов (collapsible CollectionView), кнопки «Продолжить курс»/«Пройти ещё раз», конфетти (`ConfettiView`).
- **Правила:** проходной 80% (настройка HR); провал+нет попыток → блок курса + уведомление HR; успех финала → генерация сертификата; результат → история/статистика.
- **Файлы:** `Views/QuizResultPage.xaml(.cs)`, `ViewModels/QuizResultViewModel.cs`, `Controls/CircularProgressView.cs`, `Controls/ConfettiView.cs`.

### Экран 9. MyProgressPage — `MyProgressViewModel`
- **Свойства:** `SelectedPeriod`(StatPeriod), `Stats`, `DailyActivity`, `CurrentStreak`, `MaxStreak`, `CompletedCourses`, `AssignedCourses`. **Команды:** `LoadCommand`, `ChangePeriodCommand`, `OpenCourseCommand`.
- **UI:** заголовок H1, SegmentedControl периода, сводные карточки (HorizontalScrollView), `StreakCalendarView` (Grid 7×N, GitHub-style), график активности (Microcharts Bar/Line), списки завершённых и назначенных курсов (с прогрессом/дедлайнами).
- **Правила:** streak — дни подряд ≥1 урок; агрегация из LearningSession; просрочка — красным.
- **Файлы:** `Views/MyProgressPage.xaml(.cs)`, `ViewModels/MyProgressViewModel.cs`, `Controls/StreakCalendarView.cs`.

### Экран 10. CertificatesPage — `CertificatesViewModel`
- **Свойства:** `Certificates`, `Count`, `IsEmpty`. **Команды:** `LoadCommand`, `DownloadPdfCommand(cert)`, `ShareCommand(cert)`.
- **UI:** заголовок, счётчик, `CollectionView` карточек (превью, название курса, дата, эмитент, кнопки «Скачать PDF»/«Поделиться»), пустое состояние.
- **Правила:** генерация при ≥80% финала; PDF (QuestPDF): имя, курс, дата, подпись HR-директора, QR (QRCoder); blob в SQLite; «Поделиться» — `Share.RequestAsync`.
- **Файлы:** `Views/CertificatesPage.xaml(.cs)`, `ViewModels/CertificatesViewModel.cs`, `Services/Implementations/CertificateService.cs` (QuestPDF+QRCoder).

### Экран 11. AchievementsPage — `AchievementsViewModel`
- **Свойства:** `Level`(UserLevel), `Achievements`, `SelectedTab`(Все/Получено/Заблокировано). **Команды:** `LoadCommand`, `SelectTabCommand`, `ShowDetailCommand(badge)`, `OpenLeaderboardCommand`.
- **UI:** заголовок, уровень+XP ProgressBar, TabBar (3 вкладки), Grid 3-кол. бейджей (цветной/серый), BottomSheet (CommunityToolkit Popup) с деталями и прогрессом.
- **Правила:** примеры бейджей (Первый шаг/Отличник/Марафонец/Полиглот/Быстрый старт); XP 50–500.
- **Файлы:** `Views/AchievementsPage.xaml(.cs)`, `ViewModels/AchievementsViewModel.cs`.

### Экран 12. LeaderboardPage — `LeaderboardViewModel`
- **Свойства:** `Scope`(Team/Company), `Period`, `TopThree`, `CurrentUserEntry`, `Entries`, `IsRefreshing`. **Команды:** `LoadCommand`, `ChangeScopeCommand`, `ChangePeriodCommand`, `RefreshCommand`, `ShowProfileCommand(entry)`.
- **UI:** заголовок, SegmentedControl Команда/Компания и период, топ-3 подиум, выделенная карточка пользователя, `ListView`/`CollectionView` позиций 4–N, pull-to-refresh, BottomSheet публичного профиля.
- **Правила:** XP-источники (уроки/тесты/бейджи/streak); пересчёт раз в час (мок); анонимизация из настроек.
- **Файлы:** `Views/LeaderboardPage.xaml(.cs)`, `ViewModels/LeaderboardViewModel.cs`.

### Экран 13. TeamDashboardPage — `TeamDashboardViewModel` (роль Manager/Admin)
- **Свойства:** `TeamStats`, `AtRiskMembers`, `AllMembers`, `DepartmentName`. **Команды:** `LoadCommand`, `RemindCommand(member)`, `OpenMemberCommand(member)`, `AssignCourseCommand`.
- **UI:** заголовок+отдел, сводка Grid 2×2 (просрочки красным), «Требуют внимания» (CollectionView, прогресс-бар красный при <30%, кнопка «Напомнить»), список сотрудников, FAB «Назначить курс».
- **Правила:** зона риска — прогресс <30% и дедлайн ≤3 дн; только прямые подчинённые; «Напомнить» 1/24ч.
- **Навигация:** FAB → `assign-course`; тап → BottomSheet статистики.
- **Файлы:** `Views/TeamDashboardPage.xaml(.cs)`, `ViewModels/TeamDashboardViewModel.cs`.

### Экран 14. AssignCoursePage — `AssignCourseViewModel` (роль HR/Manager)
- **Свойства:** `Step`(1-3), `SelectedCourse`, `Recipients`, `RecipientMode`(Сотрудник/Группа/Отдел), `Deadline`, `IsMandatory`, `ReminderDays`, `Message`, `Summary`. **Команды:** `NextStepCommand`, `PrevStepCommand`, `SelectCourseCommand`, `ToggleRecipientCommand`, `AssignCommand`.
- **UI:** мастер 3 шага — (1) выбор курса (SearchBar+фильтры+RadioButton), (2) получатели (SegmentedControl+SearchBar+CheckBox), (3) настройки (DatePicker дедлайн, Switch обязательность/напоминания, TextEditor сообщение); карточка-сводка; кнопка «Назначить».
- **Правила:** дедлайн ≥ завтра; запрет дубля (предупреждение); пуши получателям; журнал с инициатором+меткой.
- **Навигация:** успех → `team-dashboard` + toast.
- **Файлы:** `Views/AssignCoursePage.xaml(.cs)`, `ViewModels/AssignCourseViewModel.cs`.

### Экран 15. ProfilePage — `ProfileViewModel`
- **Свойства:** `Profile`, `Level`, `Settings`(тогглы), `Theme`, `Language`, `IsAnonymous`. **Команды:** `LoadCommand`, `ChangeAvatarCommand`, `ToggleNotificationCommand`, `ToggleThemeCommand`, `OpenAchievementsCommand`, `OpenLeaderboardCommand`, `LogoutCommand`.
- **UI:** аватар+смена фото, имя/должность, email/отдел, секция «Обучение» (уровень+XP, ссылки), тогглы уведомлений (4 шт.), секция «Приложение» (язык Picker — декоративный, тема Toggle, версия), кнопка «Выйти» (красная).
- **Правила:** аватар через `MediaPicker`; настройки → `Preferences` немедленно; «Выйти» — очистка SecureStorage+кэша → `login`; анонимизация в рейтинге.
- **Файлы:** `Views/ProfilePage.xaml(.cs)`, `ViewModels/ProfileViewModel.cs`.

---

## 8. Shell-навигация и ролевая логика

### 8.1 AppShell.xaml
- Auth-`ShellContent` без TabBar: `splash`, `login`.
- TabBar Сотрудника: `home` / `catalog` / `progress` / `profile` (иконки из Resources/Images).

### 8.2 Маршруты (`Routing.RegisterRoute` в `AppShell.xaml.cs`)
`course-detail`, `lesson`, `quiz`, `quiz-result`, `certificates`, `achievements`, `leaderboard`, `team-dashboard`, `assign-course`.

### 8.3 Ролевые табы
В `AppShell.xaml.cs` после логина читаем `ISessionService.Role` и формируем набор табов:
- **Employee:** Главная/Каталог/Прогресс/Профиль.
- **Manager/Admin:** + таб «Команда» (`team-dashboard`).
- **HR:** + раздел управления (`assign-course` / каталог управления).
Реализация: построение `Tab`/`ShellContent` в коде или переключение `CurrentItem`/видимости. Admin (без отдельного экрана в БП) переиспользует управленческие экраны.

---

## 9. Конвертеры и контролы

**Converters/:** `StatusToColorConverter`, `ProgressToWidthConverter`, `BoolToVisibilityConverter`, `InvertedBoolConverter`, `DeadlineToColorConverter` (просрочка→красный), `RoleToVisibilityConverter`, `ModuleTypeToIconConverter`, `PercentToStringConverter`.

**Controls/ (GraphicsView):** `CircularProgressView` (QuizResult), `StreakCalendarView` (GitHub-style), `ConfettiView` (анимация), `RatingStarsView` (⭐). Графики активности — Microcharts.

**Resources/Styles/:** `Colors.xaml` (палитра, акценты, статусы), `Styles.xaml` (типографика H1/H2/H3, кнопки, Entry, карточки, чипы), светлая/тёмная темы (`AppThemeBinding`).

---

## 10. Поэтапный план исполнения (чек-лист)

- [ ] **Этап 0 — Скелет:** `dotnet new maui` → `LmsApp`, TFM/флаги, все NuGet, папки, `Colors.xaml`/`Styles.xaml`, `BaseViewModel`, `MauiProgram` (DI), пустой `AppShell` + заглушки → сборка и запуск на эмуляторе.
- [ ] **Этап 1 — Модели:** 20 доменных моделей (§4), DTO, Enums (§3).
- [ ] **Этап 2 — Инфраструктура:** Entities, `LocalRepository` (Init+CRUD), `MockApiClient`, `SeedData`.
- [ ] **Этап 3 — Сервисы:** 11 сервисов + `ISessionService` + `IDialogService` (интерфейс+Mock), бизнес-логика (streak, XP, проходной балл, генерация сертификата).
- [ ] **Этап 4 — Конвертеры/контролы:** §9.
- [ ] **Этап 5 — Auth:** Splash, Login (экраны 1–2).
- [ ] **Этап 6 — Ядро обучения:** Home, Catalog, CourseDetail, Lesson, Quiz, QuizResult (3–8).
- [ ] **Этап 7 — Личный кабинет:** MyProgress, Certificates (9–10, QuestPDF+QRCoder).
- [ ] **Этап 8 — Геймификация:** Achievements, Leaderboard (11–12).
- [ ] **Этап 9 — Менеджер/HR:** TeamDashboard, AssignCourse (13–14).
- [ ] **Этап 10 — Профиль:** ProfilePage (15).
- [ ] **Этап 11 — Ролевая навигация и полировка:** динамические табы, анимации, состояния загрузки/пустые, единый стиль ошибок, оффлайн-кэш.

**Зависимости этапов:** 0→1→2→3 строго последовательно (фундамент). Этапы 5–10 зависят от 0–4, но между собой относительно независимы (можно вести по флоу). Этап 11 — финальный.

---

## 11. Верификация

Основная цель проверки — Android-эмулятор Pixel 7, API 34.

1. **Сборка:** `dotnet build -f net10.0-android` без ошибок и nullable-варнингов.
2. **Запуск:** деплой на эмулятор; Splash → (нет токена) → Login.
3. **Сквозной сценарий Сотрудника:** логин (employee@corp/123456) → Home → Catalog (поиск+фильтр) → CourseDetail → Lesson (статья/карточки/инфографика, фиксация прочтения) → Quiz (single/multiple/true-false) → QuizResult (≥80% → конфетти + сертификат) → Certificates (PDF с QR, «Поделиться») → MyProgress (streak-календарь + график) → Achievements/Leaderboard.
4. **Роли:** перелогин Менеджером (TeamDashboard, зона риска, «Напомнить») и HR (AssignCourse — мастер 3 шага, валидация дедлайна).
5. **Персистентность:** перезапуск — токен ведёт сразу на Home; прогресс/сертификаты в SQLite; тема/уведомления сохранены.
6. **Оффлайн:** отключить сеть эмулятора — уроки открываются из локального кэша.
7. **Граничные случаи:** 5 неудачных логинов → блокировка; провал теста без попыток → блок курса; дедлайн раньше завтра → запрет; повторное назначение → предупреждение.

> iOS/macCatalyst не проверяем на этом этапе (разработка под macOS, таргет — Android); код платформонезависим.

---

*Документ подготовлен в рамках курсовой работы по дисциплине «Разработка мобильных приложений».*
