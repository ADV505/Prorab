using AtomUI.Controls;
using AtomUI.Icons;
using AtomUI.Icons.AntDesign;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Prorab.UI.Converters;

/// <summary>Превращает AntDesignIconKind в объект Icon для Avatar.</summary>
public class AntDesignIconKindToIconConverter : IValueConverter
{
    private static readonly Dictionary<AntDesignIconKind, Icon> Cache = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not AntDesignIconKind kind) return null;
        if (Cache.TryGetValue(kind, out var icon)) return icon;

        // Имя enum-значения совпадает с именем класса иконки: TeamOutlined и т.д.
        var type = typeof(TeamOutlined).Assembly
            .GetType($"AtomUI.Icons.AntDesign.{kind}");
        if (type is null) return null;

        icon = (Icon)Activator.CreateInstance(type)!;
        Cache[kind] = icon;
        return icon;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
