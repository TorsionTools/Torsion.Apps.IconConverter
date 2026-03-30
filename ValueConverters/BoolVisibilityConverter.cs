using System.Globalization;
using System.Windows;

namespace Torsion.Apps.IconConverter.ValueConverters;

internal class BoolVisibilityConverter : BaseValueConverter<BoolVisibilityConverter>
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool)value ? Visibility.Visible : Visibility.Hidden;
    }

    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
