using System.Globalization;
using System.Windows.Controls;

namespace GpsMapRoutes
{
    public class LatValidationRule : NumberValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo = null)
        {
            ValidationResult result_validation = base.Validate(value, cultureInfo);
            if (!result_validation.IsValid)
            {
                return result_validation;
            }

            if (result < -90 || result > 90)
            {
                return new ValidationResult(false, "Широта измеряется от -90 до +90. Ноль - экватор, минусовые значения - южное полушарие, плюсовые - северное.");
            }

            return new ValidationResult(true, "all right");
        }
    }
}