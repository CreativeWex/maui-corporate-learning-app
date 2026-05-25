using SQLite;
using LmsApp.Infrastructure.Entities;

namespace LmsApp.Infrastructure;

public class LocalRepository : ILocalRepository
{
    private SQLiteAsyncConnection? _db;
    private const string SeedFlag = "db_seeded_v1";

    async Task<SQLiteAsyncConnection> Db()
    {
        if (_db != null) return _db;
        var path = Path.Combine(FileSystem.AppDataDirectory, "lms.db3");
        _db = new SQLiteAsyncConnection(path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        return _db;
    }

    public async Task InitAsync()
    {
        var db = await Db();
        await db.CreateTablesAsync<UserEntity, CourseEntity, ModuleEntity, LessonEntity, QuizEntity>();
        await db.CreateTablesAsync<QuestionEntity, QuizResultEntity, AssignmentEntity, CertificateEntity, AchievementEntity>();
        await db.CreateTablesAsync<UserAchievementEntity, LearningSessionEntity, LeaderboardEntryEntity>();

        if (!Preferences.Get(SeedFlag, false))
            await SeedAsync(db);
    }

    async Task SeedAsync(SQLiteAsyncConnection db)
    {
        var now = DateTime.Now;
        await db.InsertAllAsync(SeedData.Users());
        await db.InsertAllAsync(SeedData.Courses());
        await db.InsertAllAsync(SeedData.Modules());
        await db.InsertAllAsync(SeedData.Lessons());
        await db.InsertAllAsync(SeedData.Quizzes());
        await db.InsertAllAsync(SeedData.Questions());
        await db.InsertAllAsync(SeedData.Assignments(now));
        await db.InsertAllAsync(SeedData.Achievements());
        await db.InsertAllAsync(SeedData.UserAchievements(now));
        await db.InsertAllAsync(SeedData.Leaderboard());
        await db.InsertAllAsync(SeedData.LearningSessions(now));
        await db.InsertAsync(SeedData.CompletedIbQuizResult(now));
        Preferences.Set(SeedFlag, true);
    }

    // --- Users ---
    public async Task<UserEntity?> GetUserByEmailAsync(string email)
    {
        var db = await Db();
        return await db.Table<UserEntity>().Where(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<UserEntity?> GetUserByIdAsync(int id)
    {
        var db = await Db();
        return await db.FindAsync<UserEntity>(id);
    }

    public async Task SaveUserAsync(UserEntity user)
    {
        var db = await Db();
        if (user.Id == 0) await db.InsertAsync(user);
        else await db.InsertOrReplaceAsync(user);
    }

    public async Task<List<UserEntity>> GetUsersByManagerIdAsync(int managerId)
    {
        var db = await Db();
        return await db.Table<UserEntity>().Where(u => u.ManagerId == managerId).ToListAsync();
    }

    // --- Courses ---
    public async Task<List<CourseEntity>> GetAllCoursesAsync()
    {
        var db = await Db();
        return await db.Table<CourseEntity>().ToListAsync();
    }

    public async Task<CourseEntity?> GetCourseByIdAsync(int id)
    {
        var db = await Db();
        return await db.FindAsync<CourseEntity>(id);
    }

    public async Task SaveCourseAsync(CourseEntity course)
    {
        var db = await Db();
        await db.InsertOrReplaceAsync(course);
    }

    // --- Modules ---
    public async Task<List<ModuleEntity>> GetModulesByCourseIdAsync(int courseId)
    {
        var db = await Db();
        return await db.Table<ModuleEntity>()
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.OrderIndex)
            .ToListAsync();
    }

    public async Task SaveModuleAsync(ModuleEntity module)
    {
        var db = await Db();
        if (module.Id == 0) await db.InsertAsync(module);
        else await db.InsertOrReplaceAsync(module);
    }

    public async Task UpdateModuleStatusAsync(int moduleId, int status)
    {
        var db = await Db();
        var m = await db.FindAsync<ModuleEntity>(moduleId);
        if (m != null) { m.Status = status; await db.UpdateAsync(m); }
    }

    // --- Lessons ---
    public async Task<LessonEntity?> GetLessonByIdAsync(int id)
    {
        var db = await Db();
        return await db.FindAsync<LessonEntity>(id);
    }

    public async Task<LessonEntity?> GetLessonByModuleIdAsync(int moduleId)
    {
        var db = await Db();
        return await db.Table<LessonEntity>().Where(l => l.ModuleId == moduleId).FirstOrDefaultAsync();
    }

    public async Task SaveLessonAsync(LessonEntity lesson)
    {
        var db = await Db();
        if (lesson.Id == 0) await db.InsertAsync(lesson);
        else await db.InsertOrReplaceAsync(lesson);
    }

    public async Task MarkLessonCompletedAsync(int lessonId)
    {
        var db = await Db();
        var l = await db.FindAsync<LessonEntity>(lessonId);
        if (l != null) { l.IsCompleted = true; await db.UpdateAsync(l); }
    }

    // --- Quizzes ---
    public async Task<QuizEntity?> GetQuizByIdAsync(int id)
    {
        var db = await Db();
        return await db.FindAsync<QuizEntity>(id);
    }

    public async Task<QuizEntity?> GetQuizByModuleIdAsync(int moduleId)
    {
        var db = await Db();
        return await db.Table<QuizEntity>().Where(q => q.ModuleId == moduleId).FirstOrDefaultAsync();
    }

    public async Task<List<QuestionEntity>> GetQuestionsByQuizIdAsync(int quizId)
    {
        var db = await Db();
        return await db.Table<QuestionEntity>().Where(q => q.QuizId == quizId).ToListAsync();
    }

    public async Task SaveQuizAsync(QuizEntity quiz)
    {
        var db = await Db();
        if (quiz.Id == 0) await db.InsertAsync(quiz);
        else await db.InsertOrReplaceAsync(quiz);
    }

    public async Task SaveQuestionAsync(QuestionEntity question)
    {
        var db = await Db();
        if (question.Id == 0) await db.InsertAsync(question);
        else await db.InsertOrReplaceAsync(question);
    }

    public async Task SaveQuizResultAsync(QuizResultEntity result)
    {
        var db = await Db();
        await db.InsertAsync(result);
    }

    public async Task<List<QuizResultEntity>> GetQuizResultsByUserAsync(int userId, int quizId)
    {
        var db = await Db();
        return await db.Table<QuizResultEntity>()
            .Where(r => r.UserId == userId && r.QuizId == quizId)
            .ToListAsync();
    }

    public async Task<List<QuizResultEntity>> GetAllQuizResultsByUserAsync(int userId)
    {
        var db = await Db();
        return await db.Table<QuizResultEntity>().Where(r => r.UserId == userId).ToListAsync();
    }

    public async Task<int> GetAttemptCountAsync(int userId, int quizId)
    {
        var db = await Db();
        return await db.Table<QuizResultEntity>()
            .CountAsync(r => r.UserId == userId && r.QuizId == quizId);
    }

    // --- Assignments ---
    public async Task<List<AssignmentEntity>> GetAssignmentsByUserIdAsync(int userId)
    {
        var db = await Db();
        return await db.Table<AssignmentEntity>().Where(a => a.UserId == userId).ToListAsync();
    }

    public async Task<AssignmentEntity?> GetAssignmentAsync(int userId, int courseId)
    {
        var db = await Db();
        return await db.Table<AssignmentEntity>()
            .Where(a => a.UserId == userId && a.CourseId == courseId)
            .FirstOrDefaultAsync();
    }

    public async Task SaveAssignmentAsync(AssignmentEntity assignment)
    {
        var db = await Db();
        if (assignment.Id == 0) await db.InsertAsync(assignment);
        else await db.InsertOrReplaceAsync(assignment);
    }

    // --- Certificates ---
    public async Task<List<CertificateEntity>> GetCertificatesByUserIdAsync(int userId)
    {
        var db = await Db();
        return await db.Table<CertificateEntity>().Where(c => c.UserId == userId).ToListAsync();
    }

    public async Task<CertificateEntity?> GetCertificateAsync(int userId, int courseId)
    {
        var db = await Db();
        return await db.Table<CertificateEntity>()
            .Where(c => c.UserId == userId && c.CourseId == courseId)
            .FirstOrDefaultAsync();
    }

    public async Task SaveCertificateAsync(CertificateEntity cert)
    {
        var db = await Db();
        if (cert.Id == 0) await db.InsertAsync(cert);
        else await db.InsertOrReplaceAsync(cert);
    }

    // --- Achievements ---
    public async Task<List<AchievementEntity>> GetAllAchievementsAsync()
    {
        var db = await Db();
        return await db.Table<AchievementEntity>().ToListAsync();
    }

    public async Task<List<UserAchievementEntity>> GetUserAchievementsAsync(int userId)
    {
        var db = await Db();
        return await db.Table<UserAchievementEntity>().Where(ua => ua.UserId == userId).ToListAsync();
    }

    public async Task SaveAchievementAsync(AchievementEntity achievement)
    {
        var db = await Db();
        await db.InsertOrReplaceAsync(achievement);
    }

    public async Task SaveUserAchievementAsync(UserAchievementEntity ua)
    {
        var db = await Db();
        await db.InsertAsync(ua);
    }

    // --- Learning Sessions ---
    public async Task SaveLearningSessionAsync(LearningSessionEntity session)
    {
        var db = await Db();
        await db.InsertAsync(session);
    }

    public async Task<List<LearningSessionEntity>> GetSessionsByUserAsync(int userId, DateTime from, DateTime to)
    {
        var fromStr = from.ToString("O");
        var toStr = to.ToString("O");
        var db = await Db();
        var all = await db.Table<LearningSessionEntity>()
            .Where(s => s.UserId == userId)
            .ToListAsync();
        return all.Where(s => DateTime.Parse(s.StartedAt) >= from && DateTime.Parse(s.StartedAt) <= to).ToList();
    }

    public async Task<List<LearningSessionEntity>> GetSessionsByUserAsync(int userId)
    {
        var db = await Db();
        return await db.Table<LearningSessionEntity>().Where(s => s.UserId == userId).ToListAsync();
    }

    // --- Leaderboard ---
    public async Task<List<LeaderboardEntryEntity>> GetLeaderboardAsync()
    {
        var db = await Db();
        return await db.Table<LeaderboardEntryEntity>().OrderByDescending(e => e.TotalXp).ToListAsync();
    }

    public async Task SaveLeaderboardEntryAsync(LeaderboardEntryEntity entry)
    {
        var db = await Db();
        if (entry.Id == 0) await db.InsertAsync(entry);
        else await db.InsertOrReplaceAsync(entry);
    }

    public async Task UpdateLeaderboardXpAsync(int userId, int xp)
    {
        var db = await Db();
        var entry = await db.Table<LeaderboardEntryEntity>().Where(e => e.UserId == userId).FirstOrDefaultAsync();
        if (entry != null) { entry.TotalXp = xp; await db.UpdateAsync(entry); }
    }
}
