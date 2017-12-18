using System;

namespace LucaHome.Rules
{
    public class TextBoxIntegerRule
    {
        public static bool Validate(string str)
        {
            foreach (char c in str)
            {
                if (!Char.IsNumber(c)) return false;
            }

            return true;
        }
    }
}
