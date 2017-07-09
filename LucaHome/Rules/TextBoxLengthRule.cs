using System.Globalization;
using System.Windows.Controls;

namespace LucaHome.Rules
{
    public class TextBoxLengthRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var enteredString = value as string;

            if (enteredString == null)
            {
                return new ValidationResult(false, "Please enter some text");
            }

            if (enteredString == string.Empty)
            {
                return new ValidationResult(false, "Please enter some text");
            }

            if (enteredString.Length < 2)
            {
                return new ValidationResult(false, "Please enter some text");
            }

            return new ValidationResult(true, null);
        }
    }
}
