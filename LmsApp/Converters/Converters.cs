using System.Globalization;
using LmsApp.Models.Enums;

namespace LmsApp.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && b;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && b;
}

public class InvertedBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
}

public class InvertedBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not bool b || !b;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
}

public class PercentToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int i ? $"{i}%" : value is double d ? $"{d:F0}%" : "0%";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => 0;
}

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ModuleStatus status)
        {
            return status switch
            {
                ModuleStatus.Completed  => Color.FromArgb("#10B981"),
                ModuleStatus.InProgress => Color.FromArgb("#F97316"),
                ModuleStatus.Locked     => Color.FromArgb("#9CA3AF"),
                _                       => Color.FromArgb("#6B7280")
            };
        }
        return Color.FromArgb("#9CA3AF");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => ModuleStatus.NotStarted;
}

public class DeadlineToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime deadline)
        {
            var days = (deadline - DateTime.Today).TotalDays;
            if (days < 0)   return Color.FromArgb("#EF4444");
            if (days <= 3)  return Color.FromArgb("#F59E0B");
            return Color.FromArgb("#10B981");
        }
        if (value is not null)
        {
            DateTime? nd = value as DateTime?;
            if (nd.HasValue)
            {
                var days2 = (nd.Value - DateTime.Today).TotalDays;
                if (days2 < 0)  return Color.FromArgb("#EF4444");
                if (days2 <= 3) return Color.FromArgb("#F59E0B");
                return Color.FromArgb("#10B981");
            }
        }
        return Color.FromArgb("#6B7280");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => DateTime.Now;
}

public class ModuleTypeToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ModuleType type)
        {
            return type switch
            {
                ModuleType.Article     => "📄",
                ModuleType.Flashcards  => "🃏",
                ModuleType.Quiz        => "❓",
                ModuleType.Infographic => "📊",
                ModuleType.MindMap     => "🗺️",
                ModuleType.Image       => "🖼️",
                _                      => "📄"
            };
        }
        return "📄";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => ModuleType.Article;
}

public class RoleToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is UserRole role && parameter is string roleStr)
        {
            var allowedRoles = roleStr.Split(',').Select(r => Enum.Parse<UserRole>(r.Trim())).ToList();
            return allowedRoles.Contains(role);
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => UserRole.Employee;
}

public class ProgressToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int progress)
        {
            return progress switch
            {
                >= 80 => Color.FromArgb("#10B981"),
                >= 50 => Color.FromArgb("#F97316"),
                >= 30 => Color.FromArgb("#F59E0B"),
                _     => Color.FromArgb("#EF4444")
            };
        }
        return Color.FromArgb("#9CA3AF");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => 0;
}

public class StringNotEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => !string.IsNullOrWhiteSpace(value?.ToString());

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => string.Empty;
}

public class PercentToProgressConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int i ? i / 100.0 : value is double d ? d / 100.0 : 0.0;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is double d ? (int)(d * 100) : 0;
}

public class IntToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int i && i > 0;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => 0;
}
