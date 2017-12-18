using Common.Interfaces;
using Common.Tools;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Common.Converter
{
    public class JsonDataToCoinConversionConverter : IJsonDataConverter<KeyValuePair<string, double>>
    {
        private const string TAG = "JsonDataToCoinConversionConverter";

        private static JsonDataToCoinConversionConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToCoinConversionConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToCoinConversionConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToCoinConversionConverter();
                    }

                    return _instance;
                }
            }
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

                if (data.Length == 3)
                {
                    string key = data[0].Replace("\"", "");

                    string numberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    string valueString = data[2].Replace(".", numberDecimalSeparator);
                    double value = 0;
                    bool parseValueSuccess = double.TryParse(valueString, out value);

                    KeyValuePair<string, double> newValue = new KeyValuePair<string, double>(key, value);
                    list.Add(newValue);
                }
                else
                {
                    Logger.Instance.Warning(TAG, string.Format("Data has invalid length {0}, entry is {1}", data.Length, entry));
                }
            }

            return list;
        }
    }
}
