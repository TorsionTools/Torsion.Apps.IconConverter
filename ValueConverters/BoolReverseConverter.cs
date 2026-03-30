using System.Globalization;

namespace Torsion.Apps.IconConverter.ValueConverters;

internal class BoolReverseConverter : BaseValueConverter<BoolReverseConverter>
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !(bool)value;
    }

    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
