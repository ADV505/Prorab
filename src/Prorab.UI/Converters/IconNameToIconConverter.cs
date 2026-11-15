using AtomUI.Controls;
using AtomUI.Icons;
using AtomUI.Icons.AntDesign;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Prorab.UI.Converters;

/// <summary>
/// Превращает строку вида "TeamOutlined" в иконку AtomUI.
/// Имя ищется по классам AtomUI.Icons.AntDesign — новые иконки добавлять в код не нужно.
/// </summary>
public class IconNameToIconConverter : IValueConverter
{
    // Кэш: чтобы иконка не пересоздавалась на каждый рендер
    private static readonly Dictionary<string, Icon> Cache = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string name || string.IsNullOrWhiteSpace(name))
            return null;

        if (Cache.TryGetValue(name, out var cached))
            return cached;

        var type = typeof(TeamOutlined).Assembly
            .GetType($"AtomUI.Icons.AntDesign.{name.Trim()}");

        if (type is null)
            return null;

        var icon = (Icon)Activator.CreateInstance(type)!;
        Cache[name] = icon;
        return icon;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
