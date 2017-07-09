using LucaHome.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;

namespace TestProject
{
    [TestClass]
    public class RuleUnitTest
    {
        [TestMethod]
        public void TextBoxNotEmptyRuleStringEmptyInvalid()
        {
            string testString = string.Empty;

            ValidationResult ruleResult = new TextBoxNotEmptyRule().Validate(testString, null);
            ValidationResult expectedResult = new ValidationResult(false, "Please enter some text");

            Assert.AreEqual(ruleResult, expectedResult);
        }

        [TestMethod]
        public void TextBoxNotEmptyRuleStringNullInvalid()
        {
            string testString = null;

            ValidationResult ruleResult = new TextBoxNotEmptyRule().Validate(testString, null);
            ValidationResult expectedResult = new ValidationResult(false, "Please enter some text");

            Assert.AreEqual(ruleResult, expectedResult);
        }

        [TestMethod]
        public void TextBoxNotEmptyRuleStringValid()
        {
            string testString = "Hello world";

            ValidationResult ruleResult = new TextBoxNotEmptyRule().Validate(testString, null);
            ValidationResult expectedResult = new ValidationResult(true, null);

            Assert.AreEqual(ruleResult, expectedResult);
        }

        [TestMethod]
        public void TextBoxLengthRuleStringInvalid()
        {
            string testString = "A";

            ValidationResult ruleResult = new TextBoxLengthRule().Validate(testString, null);
            ValidationResult expectedResult = new ValidationResult(false, "Please enter some text");

            Assert.AreEqual(ruleResult, expectedResult);
        }

        [TestMethod]
        public void TextBoxLengthRuleStringValid()
        {
            string testString = "Hello world";

            ValidationResult ruleResult = new TextBoxLengthRule().Validate(testString, null);
            ValidationResult expectedResult = new ValidationResult(true, null);

            Assert.AreEqual(ruleResult, expectedResult);
        }

        [TestMethod]
        public void WirelessSocketCodeRuleValid()
        {
            string testString1 = "00100E";

            ValidationResult ruleResult1 = new WirelessSocketCodeFormatRule().Validate(testString1, null);
            ValidationResult expectedResult1 = new ValidationResult(true, null);

            Assert.AreEqual(ruleResult1, expectedResult1);

            string testString2 = "11111C";

            ValidationResult ruleResult2 = new WirelessSocketCodeFormatRule().Validate(testString2, null);
            ValidationResult expectedResult2 = new ValidationResult(true, null);

            Assert.AreEqual(ruleResult2, expectedResult2);
        }

        [TestMethod]
        public void WirelessSocketCodeRuleInvalid()
        {
            string testString1 = "02100R";

            ValidationResult ruleResult1 = new WirelessSocketCodeFormatRule().Validate(testString1, null);
            ValidationResult expectedResult1 = new ValidationResult(false, "Please enter valid a code");

            Assert.AreEqual(ruleResult1, expectedResult1);

            string testString2 = "3 C11T";

            ValidationResult ruleResult2 = new WirelessSocketCodeFormatRule().Validate(testString2, null);
            ValidationResult expectedResult2 = new ValidationResult(false, "Please enter valid a code");

            Assert.AreEqual(ruleResult2, expectedResult2);

            string testString3 = "0011A";

            ValidationResult ruleResult3 = new WirelessSocketCodeFormatRule().Validate(testString3, null);
            ValidationResult expectedResult3 = new ValidationResult(false, "Please enter valid a code");

            Assert.AreEqual(ruleResult3, expectedResult3);
        }
    }
}
