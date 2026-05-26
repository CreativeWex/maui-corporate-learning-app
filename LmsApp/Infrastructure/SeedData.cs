using System.Text.Json;
using LmsApp.Infrastructure.Entities;
using LmsApp.Models.Enums;

namespace LmsApp.Infrastructure;

public static class SeedData
{
    public static List<UserEntity> Users() =>
    [
        new() { Id = 1, Email = "employee@corp", PasswordHash = "123456", Name = "Алексей Иванов", Role = (int)UserRole.Employee, Department = "IT", Position = "Разработчик", ManagerId = 2, TotalXp = 340, Level = 3 },
        new() { Id = 2, Email = "manager@corp",  PasswordHash = "123456", Name = "Мария Петрова",  Role = (int)UserRole.Manager,  Department = "IT", Position = "Тимлид",     ManagerId = null, TotalXp = 870, Level = 6 },
        new() { Id = 3, Email = "hr@corp",        PasswordHash = "123456", Name = "Ольга Смирнова", Role = (int)UserRole.Hr,       Department = "HR", Position = "HR-менеджер", ManagerId = null, TotalXp = 620, Level = 5 },
        new() { Id = 4, Email = "admin@corp",     PasswordHash = "123456", Name = "Дмитрий Козлов", Role = (int)UserRole.Admin,    Department = "IT", Position = "Администратор", ManagerId = null, TotalXp = 1200, Level = 8 },
        new() { Id = 5, Email = "emp2@corp",      PasswordHash = "123456", Name = "Игорь Сидоров",  Role = (int)UserRole.Employee, Department = "IT", Position = "Тестировщик", ManagerId = 2, TotalXp = 210, Level = 2 },
        new() { Id = 6, Email = "emp3@corp",      PasswordHash = "123456", Name = "Елена Новикова", Role = (int)UserRole.Employee, Department = "IT", Position = "Аналитик",    ManagerId = 2, TotalXp = 480, Level = 4 },
        new() { Id = 7, Email = "emp4@corp",      PasswordHash = "123456", Name = "Сергей Морозов", Role = (int)UserRole.Employee, Department = "IT", Position = "Разработчик", ManagerId = 2, TotalXp = 90,  Level = 1 },
        new() { Id = 8, Email = "emp5@corp",      PasswordHash = "123456", Name = "Анна Волкова",   Role = (int)UserRole.Employee, Department = "IT", Position = "Дизайнер",    ManagerId = 2, TotalXp = 560, Level = 4 },
    ];

    public static List<CourseEntity> Courses() =>
    [
        new() { Id = 1, Title = "Python для начинающих",          Category = "IT",          AuthorName = "Михаил Скворцов",  DurationMinutes = 240, Rating = 4.8, Description = "Базовый курс по программированию на Python. Изучите основы языка, структуры данных и объектно-ориентированное программирование.", IsNew = false },
        new() { Id = 2, Title = "Эффективные коммуникации",       Category = "Soft Skills", AuthorName = "Анна Крылова",     DurationMinutes = 120, Rating = 4.6, Description = "Курс по развитию навыков делового общения, публичных выступлений и работы в команде.", IsNew = false },
        new() { Id = 3, Title = "Основы информационной безопасности", Category = "Compliance", AuthorName = "Сергей Белов",  DurationMinutes = 90,  Rating = 4.5, Description = "Обязательный курс по защите корпоративных данных, парольной политике и безопасной работе в сети.", IsNew = false },
        new() { Id = 4, Title = "Управление проектами: Agile",    Category = "Менеджмент", AuthorName = "Наталья Фёдорова", DurationMinutes = 180, Rating = 4.7, Description = "Практический курс по Agile-методологиям: Scrum, Kanban, планирование спринтов и управление бэклогом.", IsNew = true },
        new() { Id = 5, Title = "Git и версионный контроль",      Category = "IT",          AuthorName = "Михаил Скворцов",  DurationMinutes = 150, Rating = 4.9, Description = "Полный курс по работе с Git: ветки, слияния, pull request'ы и лучшие практики командной разработки.", IsNew = true },
        new() { Id = 6, Title = "Деловой этикет",                 Category = "Soft Skills", AuthorName = "Екатерина Лебедева", DurationMinutes = 60, Rating = 4.4, Description = "Нормы и правила профессионального поведения в офисе и на деловых мероприятиях.", IsNew = false },
        new() { Id = 7, Title = "GDPR и защита персональных данных", Category = "Compliance", AuthorName = "Сергей Белов",   DurationMinutes = 75,  Rating = 4.3, Description = "Курс о требованиях GDPR, правах субъектов данных и обязанностях компании при обработке персональных данных.", IsNew = false },
        new() { Id = 8, Title = "Docker и контейнеризация",       Category = "IT",          AuthorName = "Алексей Дроздов",  DurationMinutes = 200, Rating = 4.7, Description = "Практический курс по Docker: образы, контейнеры, Docker Compose и развёртывание микросервисов.", IsNew = true },
    ];

    public static List<ModuleEntity> Modules() =>
    [
        // Course 1: Python
        new() { Id = 1,  CourseId = 1, Title = "Введение в Python",        Type = (int)ModuleType.Article,     DurationMin = 20, Status = (int)ModuleStatus.Completed, OrderIndex = 0, LessonId = 1 },
        new() { Id = 2,  CourseId = 1, Title = "Типы данных",               Type = (int)ModuleType.Flashcards,  DurationMin = 15, Status = (int)ModuleStatus.Completed, OrderIndex = 1, LessonId = 2 },
        new() { Id = 3,  CourseId = 1, Title = "Функции и классы",          Type = (int)ModuleType.Article,     DurationMin = 25, Status = (int)ModuleStatus.InProgress, OrderIndex = 2, LessonId = 3 },
        new() { Id = 4,  CourseId = 1, Title = "Финальный тест",            Type = (int)ModuleType.Quiz,        DurationMin = 30, Status = (int)ModuleStatus.Locked,    OrderIndex = 3, QuizId = 1 },
        // Course 2: Коммуникации
        new() { Id = 5,  CourseId = 2, Title = "Активное слушание",         Type = (int)ModuleType.Article,     DurationMin = 20, Status = (int)ModuleStatus.NotStarted, OrderIndex = 0, LessonId = 4 },
        new() { Id = 6,  CourseId = 2, Title = "Типы коммуникаций",         Type = (int)ModuleType.Infographic, DurationMin = 10, Status = (int)ModuleStatus.Locked,    OrderIndex = 1, LessonId = 5 },
        new() { Id = 7,  CourseId = 2, Title = "Тест по коммуникациям",     Type = (int)ModuleType.Quiz,        DurationMin = 20, Status = (int)ModuleStatus.Locked,    OrderIndex = 2, QuizId = 2 },
        // Course 3: ИБ
        new() { Id = 8,  CourseId = 3, Title = "Угрозы информационной безопасности", Type = (int)ModuleType.Article, DurationMin = 25, Status = (int)ModuleStatus.Completed, OrderIndex = 0, LessonId = 6 },
        new() { Id = 9,  CourseId = 3, Title = "Карточки: термины ИБ",      Type = (int)ModuleType.Flashcards,  DurationMin = 10, Status = (int)ModuleStatus.Completed, OrderIndex = 1, LessonId = 7 },
        new() { Id = 10, CourseId = 3, Title = "Финальный тест ИБ",         Type = (int)ModuleType.Quiz,        DurationMin = 20, Status = (int)ModuleStatus.Completed, OrderIndex = 2, QuizId = 3 },
        // Course 4: Agile
        new() { Id = 11, CourseId = 4, Title = "Что такое Agile",           Type = (int)ModuleType.Article,     DurationMin = 20, Status = (int)ModuleStatus.NotStarted, OrderIndex = 0, LessonId = 8 },
        new() { Id = 12, CourseId = 4, Title = "Scrum: роли и артефакты",   Type = (int)ModuleType.Infographic, DurationMin = 15, Status = (int)ModuleStatus.Locked,    OrderIndex = 1, LessonId = 9 },
        new() { Id = 13, CourseId = 4, Title = "Тест Agile",                Type = (int)ModuleType.Quiz,        DurationMin = 25, Status = (int)ModuleStatus.Locked,    OrderIndex = 2, QuizId = 4 },
        // Course 5: Git
        new() { Id = 14, CourseId = 5, Title = "Основы Git",                Type = (int)ModuleType.Article,     DurationMin = 30, Status = (int)ModuleStatus.NotStarted, OrderIndex = 0, LessonId = 10 },
        new() { Id = 15, CourseId = 5, Title = "Ветки и слияния",           Type = (int)ModuleType.Flashcards,  DurationMin = 20, Status = (int)ModuleStatus.Locked,    OrderIndex = 1, LessonId = 11 },
        new() { Id = 16, CourseId = 5, Title = "Тест Git",                  Type = (int)ModuleType.Quiz,        DurationMin = 20, Status = (int)ModuleStatus.Locked,    OrderIndex = 2, QuizId = 5 },
    ];

    public static List<LessonEntity> Lessons() =>
    [
        // Course 1: Python
        new() { Id = 1,  CourseId = 1, ModuleId = 1,  Title = "Введение в Python",        Type = (int)ModuleType.Article,    DurationMin = 20, IsCompleted = true,
            Content = "## Что такое Python?\n\nPython — это высокоуровневый язык программирования с динамической типизацией.\n\n### Преимущества Python\n\n- Простой и читаемый синтаксис\n- Огромная стандартная библиотека\n- Кросс-платформенность\n- Популярность в ML/AI\n\n### Первая программа\n\n```python\nprint('Привет, мир!')\n```\n\nЗапустите эту команду — вы увидите приветствие в консоли.\n\n### Установка Python\n\n1. Скачайте Python 3.11+ с [python.org](https://python.org)\n2. Установите и добавьте в PATH\n3. Проверьте: `python --version`\n\nPython используется в веб-разработке (Django, Flask), анализе данных (Pandas, NumPy), машинном обучении (TensorFlow, PyTorch) и автоматизации." },
        new() { Id = 2,  CourseId = 1, ModuleId = 2,  Title = "Типы данных Python",       Type = (int)ModuleType.Flashcards, DurationMin = 15, IsCompleted = true,
            Content = JsonSerializer.Serialize(new[] {
                new { Front = "Что такое int?", Back = "Целое число. Пример: x = 42" },
                new { Front = "Что такое str?", Back = "Строка. Пример: name = 'Python'" },
                new { Front = "Что такое list?", Back = "Список. Пример: items = [1, 2, 3]" },
                new { Front = "Что такое dict?", Back = "Словарь. Пример: d = {'key': 'value'}" },
                new { Front = "Что такое bool?", Back = "Булевый тип: True или False" }
            }) },
        new() { Id = 3,  CourseId = 1, ModuleId = 3,  Title = "Функции и классы",         Type = (int)ModuleType.Article,    DurationMin = 25, IsCompleted = false,
            Content = "## Функции в Python\n\nФункции позволяют переиспользовать код:\n\n```python\ndef greet(name: str) -> str:\n    return f'Привет, {name}!'\n\nprint(greet('Алексей'))  # Привет, Алексей!\n```\n\n### Классы\n\nClassы реализуют объектно-ориентированное программирование:\n\n```python\nclass Animal:\n    def __init__(self, name: str):\n        self.name = name\n    \n    def speak(self) -> str:\n        return f'{self.name} говорит'\n\ncat = Animal('Кот')\nprint(cat.speak())  # Кот говорит\n```\n\n### Наследование\n\n```python\nclass Dog(Animal):\n    def speak(self) -> str:\n        return f'{self.name} гавкает'\n```" },
        // Course 2
        new() { Id = 4,  CourseId = 2, ModuleId = 5,  Title = "Активное слушание",        Type = (int)ModuleType.Article,    DurationMin = 20, IsCompleted = false,
            Content = "## Активное слушание\n\nАктивное слушание — это техника общения, при которой слушатель полностью концентрируется на говорящем.\n\n### Ключевые принципы\n\n1. **Поддерживайте зрительный контакт** — это показывает вовлечённость\n2. **Не перебивайте** — дайте человеку закончить мысль\n3. **Задавайте уточняющие вопросы** — «Правильно ли я понял, что...?»\n4. **Перефразируйте** — повторите своими словами услышанное\n\n### Практика\n\nВ следующем разговоре попробуйте не думать о своём ответе, пока собеседник говорит. Просто слушайте и впитывайте информацию." },
        new() { Id = 5,  CourseId = 2, ModuleId = 6,  Title = "Типы коммуникаций",        Type = (int)ModuleType.Infographic, DurationMin = 10, IsCompleted = false,
            Content = "## Типы коммуникаций\n\n**Вербальная** — слова и речь\n**Невербальная** — жесты, мимика, поза\n**Письменная** — email, документы\n**Визуальная** — графики, схемы" },
        // Course 3: ИБ
        new() { Id = 6,  CourseId = 3, ModuleId = 8,  Title = "Угрозы ИБ",               Type = (int)ModuleType.Article,    DurationMin = 25, IsCompleted = true,
            Content = "## Основные угрозы информационной безопасности\n\n### Фишинг\nФишинг — мошенничество с целью кражи данных через поддельные письма и сайты.\n\n**Признаки фишинга:**\n- Срочность и давление («Ваш аккаунт заблокирован!»)\n- Опечатки в домене (g00gle.com вместо google.com)\n- Просьба ввести пароль или данные карты\n\n### Вредоносное ПО\n- **Вирусы** — заражают файлы\n- **Трояны** — маскируются под легальный софт\n- **Шифровальщики** — блокируют файлы и требуют выкуп\n\n### Социальная инженерия\nМанипуляция людьми для получения конфиденциальной информации.\n\n### Правила безопасности в компании\n1. Не открывайте подозрительные письма\n2. Используйте сложные пароли (минимум 12 символов)\n3. Не подключайте личные USB-носители\n4. Сообщайте об инцидентах в ИБ-отдел" },
        new() { Id = 7,  CourseId = 3, ModuleId = 9,  Title = "Термины ИБ",              Type = (int)ModuleType.Flashcards, DurationMin = 10, IsCompleted = true,
            Content = JsonSerializer.Serialize(new[] {
                new { Front = "Что такое VPN?", Back = "Virtual Private Network — защищённый туннель для передачи данных через интернет" },
                new { Front = "Что такое двухфакторная аутентификация?", Back = "2FA — вход с использованием двух способов подтверждения: пароль + SMS/приложение" },
                new { Front = "Что такое брутфорс?", Back = "Атака перебором всех возможных паролей для получения доступа к системе" },
                new { Front = "Что такое инцидент ИБ?", Back = "Событие, которое может нанести вред информационным активам компании" }
            }) },
        // Course 4: Agile
        new() { Id = 8,  CourseId = 4, ModuleId = 11, Title = "Введение в Agile",        Type = (int)ModuleType.Article,    DurationMin = 20, IsCompleted = false,
            Content = "## Что такое Agile?\n\nAgile — это набор принципов гибкой разработки программного обеспечения, описанных в Манифесте Agile (2001).\n\n### 4 ценности Agile Манифеста\n\n1. **Люди и взаимодействие** важнее процессов и инструментов\n2. **Работающий продукт** важнее документации\n3. **Сотрудничество с заказчиком** важнее контракта\n4. **Готовность к изменениям** важнее следования плану\n\n### Scrum\n\nScrum — самый популярный Agile-фреймворк:\n- **Спринт** — итерация длиной 1-4 недели\n- **Product Owner** — владелец продукта, приоритизирует бэклог\n- **Scrum Master** — помогает команде следовать Scrum\n- **Daily Standup** — ежедневная встреча на 15 минут\n\n### Kanban\n\nKanban — визуализация рабочего потока через доску с колонками: To Do / In Progress / Done." },
        new() { Id = 9,  CourseId = 4, ModuleId = 12, Title = "Scrum: роли и артефакты", Type = (int)ModuleType.Infographic, DurationMin = 15, IsCompleted = false,
            Content = "## Scrum\n\n**Роли:** Product Owner, Scrum Master, Development Team\n**Артефакты:** Product Backlog, Sprint Backlog, Increment\n**События:** Sprint Planning, Daily Scrum, Sprint Review, Retrospective" },
        // Course 5: Git
        new() { Id = 10, CourseId = 5, ModuleId = 14, Title = "Основы Git",              Type = (int)ModuleType.Article,    DurationMin = 30, IsCompleted = false,
            Content = "## Что такое Git?\n\nGit — распределённая система контроля версий, созданная Линусом Торвальдсом в 2005 году.\n\n### Основные команды\n\n```bash\ngit init          # Инициализация репозитория\ngit add .         # Добавить все файлы\ngit commit -m 'msg' # Создать коммит\ngit push          # Отправить на сервер\ngit pull          # Получить изменения\ngit status        # Статус изменений\ngit log           # История коммитов\n```\n\n### Ветки\n\n```bash\ngit branch feature  # Создать ветку\ngit checkout feature # Переключиться\ngit merge feature    # Слить ветку\n```\n\n### GitHub Flow\n\n1. Создайте ветку от `main`\n2. Внесите изменения\n3. Создайте Pull Request\n4. Пройдите Code Review\n5. Слейте в `main`" },
        new() { Id = 11, CourseId = 5, ModuleId = 15, Title = "Ветки и слияния",         Type = (int)ModuleType.Flashcards, DurationMin = 20, IsCompleted = false,
            Content = JsonSerializer.Serialize(new[] {
                new { Front = "Что такое git merge?", Back = "Команда для объединения двух веток в одну" },
                new { Front = "Что такое git rebase?", Back = "Перенос коммитов одной ветки на вершину другой" },
                new { Front = "Что такое Pull Request?", Back = "Запрос на слияние ветки с code review перед мерджем" },
                new { Front = "Что такое git cherry-pick?", Back = "Применение конкретного коммита из другой ветки" }
            }) },
    ];

    public static List<QuizEntity> Quizzes() =>
    [
        new() { Id = 1, CourseId = 1, ModuleId = 4,  Title = "Финальный тест: Python",    IsFinal = true,  MaxAttempts = 3, PassPercent = 80, TimeLimitSeconds = 1200 },
        new() { Id = 2, CourseId = 2, ModuleId = 7,  Title = "Тест по коммуникациям",     IsFinal = true,  MaxAttempts = 3, PassPercent = 80 },
        new() { Id = 3, CourseId = 3, ModuleId = 10, Title = "Финальный тест ИБ",          IsFinal = true,  MaxAttempts = 3, PassPercent = 80, TimeLimitSeconds = 900 },
        new() { Id = 4, CourseId = 4, ModuleId = 13, Title = "Тест Agile",                 IsFinal = true,  MaxAttempts = 3, PassPercent = 80 },
        new() { Id = 5, CourseId = 5, ModuleId = 16, Title = "Тест Git",                   IsFinal = true,  MaxAttempts = 3, PassPercent = 80 },
    ];

    public static List<QuestionEntity> Questions() =>
    [
        // Quiz 1: Python (разные типы вопросов)
        new() { Id = 1,  QuizId = 1, Text = "Какой тип данных используется для хранения целых чисел в Python?",
            Type = (int)QuestionType.SingleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "int", "float", "str", "bool" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "int (integer) — тип для целых чисел: 1, -5, 42" },
        new() { Id = 2,  QuizId = 1, Text = "Python — интерпретируемый язык программирования?",
            Type = (int)QuestionType.TrueFalse,
            OptionsJson = JsonSerializer.Serialize(new[] { "Верно", "Неверно" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "Python интерпретируется построчно в момент выполнения" },
        new() { Id = 3,  QuizId = 1, Text = "Какие из перечисленных являются изменяемыми (mutable) типами в Python?",
            Type = (int)QuestionType.MultipleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "list", "tuple", "dict", "str" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0, 2 }),
            Explanation = "list и dict — изменяемые. tuple и str — неизменяемые (immutable)" },
        new() { Id = 4,  QuizId = 1, Text = "Как объявляется функция в Python?",
            Type = (int)QuestionType.SingleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "def func():", "function func()", "func() {}", "fun func():" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "Функции в Python объявляются с ключевым словом def" },
        new() { Id = 5,  QuizId = 1, Text = "Какой оператор используется для проверки принадлежности элемента к коллекции?",
            Type = (int)QuestionType.SingleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "in", "contains", "has", "include" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "Оператор 'in': if x in [1,2,3]:" },

        // Quiz 2: Коммуникации
        new() { Id = 13, QuizId = 2, Text = "Что является основой активного слушания?",
            Type = (int)QuestionType.SingleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "Полная концентрация на говорящем", "Быстрый ответ собеседнику", "Перебивание для уточнений", "Молчаливое присутствие" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "Активное слушание — это полная сосредоточенность на говорящем, понимание его слов и чувств" },
        new() { Id = 14, QuizId = 2, Text = "Невербальная коммуникация включает в себя жесты, мимику и позу?",
            Type = (int)QuestionType.TrueFalse,
            OptionsJson = JsonSerializer.Serialize(new[] { "Верно", "Неверно" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "Невербальная коммуникация — это передача информации без слов: через жесты, мимику, позу и взгляд" },
        new() { Id = 15, QuizId = 2, Text = "Какие техники относятся к активному слушанию?",
            Type = (int)QuestionType.MultipleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "Перефразирование услышанного", "Немедленное возражение", "Уточняющие вопросы", "Зрительный контакт" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0, 2, 3 }),
            Explanation = "Перефразирование, уточняющие вопросы и зрительный контакт — ключевые техники активного слушания" },
        new() { Id = 16, QuizId = 2, Text = "Какой тип коммуникации предполагает передачу информации через email и документы?",
            Type = (int)QuestionType.SingleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "Письменная", "Вербальная", "Невербальная", "Визуальная" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "Письменная коммуникация — email, документы, сообщения в мессенджерах" },

        // Quiz 3: ИБ
        new() { Id = 6,  QuizId = 3, Text = "Что такое фишинг?",
            Type = (int)QuestionType.SingleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "Мошенничество через поддельные письма/сайты", "Вид вируса, заражающего файлы", "Атака на сервер", "Тип шифрования данных" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "Фишинг — социальная инженерия для кражи учётных данных через поддельные ресурсы" },
        new() { Id = 7,  QuizId = 3, Text = "Минимальная длина надёжного пароля в корпоративной политике — 12 символов?",
            Type = (int)QuestionType.TrueFalse,
            OptionsJson = JsonSerializer.Serialize(new[] { "Верно", "Неверно" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "Корпоративная политика требует минимум 12 символов с использованием букв, цифр и спецсимволов" },
        new() { Id = 8,  QuizId = 3, Text = "Какие действия относятся к обеспечению информационной безопасности?",
            Type = (int)QuestionType.MultipleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "Использование VPN при работе из дома", "Передача пароля коллеге по просьбе", "Регулярное обновление ПО", "Сохранение паролей в браузере" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0, 2 }),
            Explanation = "VPN и обновление ПО — верные практики ИБ. Передача паролей и хранение в браузере — риски" },
        new() { Id = 9,  QuizId = 3, Text = "Что такое двухфакторная аутентификация (2FA)?",
            Type = (int)QuestionType.SingleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "Вход с двумя способами подтверждения", "Два разных пароля", "Вход с двух устройств одновременно", "Двойное шифрование" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "2FA = что-то, что вы знаете (пароль) + что-то, что у вас есть (SMS/приложение)" },

        // Quiz 4: Agile
        new() { Id = 10, QuizId = 4, Text = "Сколько ценностей содержит Манифест Agile?",
            Type = (int)QuestionType.SingleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "4", "12", "6", "8" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "Манифест Agile содержит 4 ценности и 12 принципов" },
        new() { Id = 11, QuizId = 4, Text = "Daily Standup длится не более 15 минут?",
            Type = (int)QuestionType.TrueFalse,
            OptionsJson = JsonSerializer.Serialize(new[] { "Верно", "Неверно" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "Daily Standup (Scrum) ограничен 15 минутами" },
        new() { Id = 12, QuizId = 4, Text = "Какие роли существуют в Scrum?",
            Type = (int)QuestionType.MultipleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "Product Owner", "Project Manager", "Scrum Master", "Development Team" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0, 2, 3 }),
            Explanation = "В Scrum три роли: Product Owner, Scrum Master и Development Team. Project Manager — не роль Scrum" },

        // Quiz 5: Git
        new() { Id = 17, QuizId = 5, Text = "Какая команда инициализирует новый Git-репозиторий?",
            Type = (int)QuestionType.SingleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "git init", "git start", "git create", "git new" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "git init создаёт новый репозиторий в текущей директории" },
        new() { Id = 18, QuizId = 5, Text = "git merge объединяет две ветки в одну?",
            Type = (int)QuestionType.TrueFalse,
            OptionsJson = JsonSerializer.Serialize(new[] { "Верно", "Неверно" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "git merge интегрирует изменения из одной ветки в другую" },
        new() { Id = 19, QuizId = 5, Text = "Какие команды Git используются для работы с ветками?",
            Type = (int)QuestionType.MultipleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "git branch", "git checkout", "git push", "git fetch" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0, 1 }),
            Explanation = "git branch создаёт ветки, git checkout переключается между ними" },
        new() { Id = 20, QuizId = 5, Text = "Что такое Pull Request?",
            Type = (int)QuestionType.SingleChoice,
            OptionsJson = JsonSerializer.Serialize(new[] { "Запрос на слияние ветки с code review", "Команда для получения изменений с сервера", "Создание новой ветки", "Откат последнего коммита" }),
            CorrectIndicesJson = JsonSerializer.Serialize(new[] { 0 }),
            Explanation = "Pull Request — механизм code review перед слиянием ветки в основную" },
    ];

    public static List<AssignmentEntity> Assignments(DateTime now) =>
    [
        new() { Id = 1, CourseId = 1, UserId = 1, AssignedById = 3, DeadlineDate = now.AddDays(14).ToString("O"), AssignedAt = now.AddDays(-7).ToString("O"), IsMandatory = false, Message = "Пожалуйста, пройдите курс до дедлайна", ReminderDaysJson = "[3,1]" },
        new() { Id = 2, CourseId = 3, UserId = 1, AssignedById = 3, DeadlineDate = now.AddDays(2).ToString("O"),  AssignedAt = now.AddDays(-14).ToString("O"), IsMandatory = true,  Message = "Обязательный курс по ИБ!", ReminderDaysJson = "[1]" },
        new() { Id = 3, CourseId = 2, UserId = 1, AssignedById = 2, DeadlineDate = now.AddDays(30).ToString("O"), AssignedAt = now.AddDays(-3).ToString("O"),  IsMandatory = false, Message = null, ReminderDaysJson = "[7,3]" },
        // Manager's team assignments
        new() { Id = 4, CourseId = 3, UserId = 5, AssignedById = 3, DeadlineDate = now.AddDays(2).ToString("O"),  AssignedAt = now.AddDays(-10).ToString("O"), IsMandatory = true,  ReminderDaysJson = "[1]" },
        new() { Id = 5, CourseId = 3, UserId = 6, AssignedById = 3, DeadlineDate = now.AddDays(10).ToString("O"), AssignedAt = now.AddDays(-5).ToString("O"),  IsMandatory = true,  ReminderDaysJson = "[3,1]" },
        new() { Id = 6, CourseId = 4, UserId = 7, AssignedById = 2, DeadlineDate = now.AddDays(1).ToString("O"),  AssignedAt = now.AddDays(-20).ToString("O"), IsMandatory = false, ReminderDaysJson = "[1]" },
        new() { Id = 7, CourseId = 4, UserId = 8, AssignedById = 2, DeadlineDate = now.AddDays(14).ToString("O"), AssignedAt = now.AddDays(-3).ToString("O"),  IsMandatory = false, ReminderDaysJson = "[7,3]" },
    ];

    public static List<AchievementEntity> Achievements() =>
    [
        new() { Id = 1, Name = "Первый шаг",     Description = "Завершите первый урок",             XpReward = 50,  Condition = "lessons_completed >= 1",   Progress = 1.0 },
        new() { Id = 2, Name = "Отличник",        Description = "Сдайте тест на 100%",               XpReward = 200, Condition = "quiz_score == 100",         Progress = 0.0 },
        new() { Id = 3, Name = "Марафонец",       Description = "Учитесь 7 дней подряд",             XpReward = 300, Condition = "streak >= 7",               Progress = 0.43 },
        new() { Id = 4, Name = "Полиглот",        Description = "Завершите 3 курса разных категорий", XpReward = 500, Condition = "courses_categories >= 3",   Progress = 0.33 },
        new() { Id = 5, Name = "Быстрый старт",   Description = "Пройдите урок в первый день",        XpReward = 100, Condition = "first_lesson_day1",         Progress = 1.0 },
        new() { Id = 6, Name = "Знаток ИБ",       Description = "Завершите курс по ИБ",              XpReward = 150, Condition = "course_3_completed",         Progress = 1.0 },
        new() { Id = 7, Name = "Командный игрок", Description = "Помогите 3 коллегам",               XpReward = 250, Condition = "help_colleagues >= 3",      Progress = 0.0 },
    ];

    public static List<UserAchievementEntity> UserAchievements(DateTime now) =>
    [
        new() { Id = 1, UserId = 1, AchievementId = 1, UnlockedAt = now.AddDays(-20).ToString("O") },
        new() { Id = 2, UserId = 1, AchievementId = 5, UnlockedAt = now.AddDays(-20).ToString("O") },
        new() { Id = 3, UserId = 1, AchievementId = 6, UnlockedAt = now.AddDays(-5).ToString("O")  },
    ];

    public static List<LeaderboardEntryEntity> Leaderboard() =>
    [
        new() { Id = 1,  UserId = 4, DisplayName = "Дмитрий Козлов",   Department = "IT",      TotalXp = 1200, BadgeCount = 7 },
        new() { Id = 2,  UserId = 2, DisplayName = "Мария Петрова",    Department = "IT",      TotalXp = 870,  BadgeCount = 5 },
        new() { Id = 3,  UserId = 3, DisplayName = "Ольга Смирнова",   Department = "HR",      TotalXp = 620,  BadgeCount = 4 },
        new() { Id = 4,  UserId = 8, DisplayName = "Анна Волкова",     Department = "IT",      TotalXp = 560,  BadgeCount = 3 },
        new() { Id = 5,  UserId = 6, DisplayName = "Елена Новикова",   Department = "IT",      TotalXp = 480,  BadgeCount = 3 },
        new() { Id = 6,  UserId = 1, DisplayName = "Алексей Иванов",   Department = "IT",      TotalXp = 340,  BadgeCount = 3 },
        new() { Id = 7,  UserId = 5, DisplayName = "Игорь Сидоров",    Department = "IT",      TotalXp = 210,  BadgeCount = 1 },
        new() { Id = 8,  UserId = 7, DisplayName = "Сергей Морозов",   Department = "IT",      TotalXp = 90,   BadgeCount = 0 },
        new() { Id = 9,  UserId = 10, DisplayName = "Пользователь #10", Department = "Sales",   TotalXp = 730,  BadgeCount = 4 },
        new() { Id = 10, UserId = 11, DisplayName = "Пользователь #11", Department = "Finance", TotalXp = 450,  BadgeCount = 2 },
        new() { Id = 11, UserId = 12, DisplayName = "Пользователь #12", Department = "Sales",   TotalXp = 380,  BadgeCount = 2 },
        new() { Id = 12, UserId = 13, DisplayName = "Пользователь #13", Department = "HR",      TotalXp = 150,  BadgeCount = 1 },
    ];

    public static List<LearningSessionEntity> LearningSessions(DateTime now)
    {
        var sessions = new List<LearningSessionEntity>();
        int id = 1;
        var rng = new Random(42);
        // ~30 дней истории активности, streak 3 дня
        var activeDays = new HashSet<int>();
        // Сделаем активность в большинстве из последних 30 дней, с небольшими пропусками
        for (int d = 30; d >= 0; d--)
        {
            if (d > 3 && rng.NextDouble() < 0.3) continue; // 30% пропуск для дней > 3 назад
            activeDays.Add(d);
        }
        // Последние 3 дня обязательно активны (для streak)
        activeDays.Add(0); activeDays.Add(1); activeDays.Add(2);

        foreach (var d in activeDays)
        {
            var date = now.Date.AddDays(-d);
            int sessionCount = rng.Next(1, 4);
            for (int s = 0; s < sessionCount; s++)
            {
                var startTime = date.AddHours(rng.Next(8, 20)).AddMinutes(rng.Next(0, 60));
                var duration = rng.Next(5, 45) * 60;
                sessions.Add(new LearningSessionEntity
                {
                    Id = id++,
                    UserId = 1,
                    LessonId = rng.Next(1, 8),
                    StartedAt = startTime.ToString("O"),
                    CompletedAt = startTime.AddSeconds(duration).ToString("O"),
                    TimeSpentSec = duration
                });
            }
        }
        return sessions;
    }

    public static List<UserModuleProgressEntity> UserModuleProgress() =>
    [
        // User 1 (employee@corp): Python course
        new() { UserId = 1, ModuleId = 1, Status = (int)ModuleStatus.Completed },
        new() { UserId = 1, ModuleId = 2, Status = (int)ModuleStatus.Completed },
        new() { UserId = 1, ModuleId = 3, Status = (int)ModuleStatus.InProgress },
        // User 1: ИБ course — all completed
        new() { UserId = 1, ModuleId = 8,  Status = (int)ModuleStatus.Completed },
        new() { UserId = 1, ModuleId = 9,  Status = (int)ModuleStatus.Completed },
        new() { UserId = 1, ModuleId = 10, Status = (int)ModuleStatus.Completed },
    ];

    public static QuizResultEntity CompletedIbQuizResult(DateTime now) => new()
    {
        Id = 1,
        QuizId = 3,
        UserId = 1,
        Score = 3,
        PassedPercent = 100,
        Passed = true,
        AttemptNumber = 1,
        TimeSpentSeconds = 420,
        AnswersJson = "[]",
        CompletedAt = now.AddDays(-5).ToString("O")
    };
}
