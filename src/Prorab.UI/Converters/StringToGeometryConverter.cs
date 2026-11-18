using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Prorab.UI.Converters;

/// <summary>Превращает SVG-путь (строку) в Geometry для PathIcon.</summary>
public class StringToGeometryConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string path && !string.IsNullOrWhiteSpace(path) ? Geometry.Parse(path) : null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
