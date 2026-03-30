using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Torsion.Apps.IconConverter.ValueConverters;

#nullable enable
public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter where T : class, new()
{
    private static readonly Lazy<T> _converter = new Lazy<T>(() => new T(), true);
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _converter.Value;
    }
    public abstract object Convert(object? value, Type targetType, object? parameter, CultureInfo culture);
    public abstract object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture);
}