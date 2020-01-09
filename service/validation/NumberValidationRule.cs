using System.Globalization;
using System.Windows.Controls;

namespace GpsMapRoutes
{
    public class NumberValidationRule : ValidationRule
    {
        public double result;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo = null)
        {
            if (cultureInfo is null)
                cultureInfo = CultureInfo.CurrentCulture;

            result = 0.0;
            bool canConvert = double.TryParse(value as string, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            return new ValidationResult(canConvert, "Значение не является координатой (например: 51.6776254)");
        }
    }
}
