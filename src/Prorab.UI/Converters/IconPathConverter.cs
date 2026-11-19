using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Prorab.UI.Converters;

public class IconPathConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string path ? Geometry.Parse(path) : null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
