////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Globalization;
using System.Windows.Controls;

namespace GpsMapRoutes
{
    public class LngValidationRule : NumberValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo = null)
        {
            ValidationResult result_validation = base.Validate(value, cultureInfo);
            if (!result_validation.IsValid)
            {
                return result_validation;
            }

            if (result < -180 || result > 180)
            {
                return new ValidationResult(false, "Долгота измеряется от -180 до +180; ноль - Основной меридиан, проходящий через Гринвич, минусовые значения - западное полушарие, плюсовые - восточное.");
            }

            return result_validation;
        }
    }
}