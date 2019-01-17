using System.Globalization;
using System.Windows.Controls;

namespace IrixiStepperControllerHelper.ValidationRules
{
    public class RuleIsInteger: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            
            if(value.ToString().IndexOf('.') > -1)
            {
                return new ValidationResult(false, "The input value must be an integer.");
            }

            if(int.TryParse(value.ToString(), out int ret))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, "The input value must be an integer.");
            }
        }
    }
}
