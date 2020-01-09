using System.Globalization;
using System.Windows.Controls;

namespace GpsMapRoutes
{
    public class PipelineNameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo = null)
        {
            if (cultureInfo is null)
                cultureInfo = CultureInfo.CurrentCulture;

            string result = value as string;
            if (string.IsNullOrWhiteSpace(result))
                return new ValidationResult(false, "Наименование не может быть пустым");

            return new ValidationResult(true, "all right");
        }
    }
}
