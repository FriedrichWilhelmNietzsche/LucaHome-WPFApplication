using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace LucaHome.Rules
{
    public class WirelessSocketCodeFormatRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var enteredString = value as string;

            var regex = new Regex(@"[0-1]{5}[A-E]{1}");

            if (!regex.IsMatch(enteredString))
            {
                return new ValidationResult(false, "Please enter valid a code");
            }

            return new ValidationResult(true, null);
        }
    }
}
