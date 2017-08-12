using Common.Interfaces;
using Common.Tools;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Common.Converter
{
    public class JsonDataToCoinConversionConverter : IJsonDataConverter<KeyValuePair<string, double>>
    {
        private const string TAG = "JsonDataToCoinConversionConverter";
        private readonly Logger _logger;

        public JsonDataToCoinConversionConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<KeyValuePair<string, double>> GetList(string[] stringArray)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return parseStringToList(stringArray[0]);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, "");
                return parseStringToList(usedEntry);
            }
        }

        public IList<KeyValuePair<string, double>> GetList(string responseString)
        {
            return parseStringToList(responseString);
        }

        private IList<KeyValuePair<string, double>> parseStringToList(string jsonValue)
        {
            IList<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>();

            string[] entries = Regex.Split(jsonValue, "\\},");
            for (int index = 0; index < entries.Length; index++)
            {
                string entry = entries[index];
                string replacedEntry = entry.Replace("},", "").Replace("{", "").Replace("}}", "");
                string[] data = Regex.Split(replacedEntry, "\\:");

                string key = data[0].Replace("\"", "");
                string valueString = data[2].Replace(".", ",");
                double value = 0;
                bool parseValueSuccess = double.TryParse(valueString, out value);

                KeyValuePair<string, double> newValue = new KeyValuePair<string, double>(key, value);
                list.Add(newValue);
            }

            return list;
        }
    }
}
