using Common.Dto;
using Common.Tools;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Common.Converter
{
    public class JsonDataToCoinConverter
    {
        private const string TAG = "JsonDataToCoinConverter";
        private static string _searchParameter = "{coin:";

        private readonly Logger _logger;

        public JsonDataToCoinConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<CoinDto> GetList(string[] stringArray, IList<KeyValuePair<string, double>> conversionList)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return parseStringToList(stringArray[0], conversionList);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, _searchParameter);
                return parseStringToList(usedEntry, conversionList);
            }
        }

        public IList<CoinDto> GetList(string responseString, IList<KeyValuePair<string, double>> conversionList)
        {
            return parseStringToList(responseString, conversionList);
        }

        private IList<CoinDto> parseStringToList(string value, IList<KeyValuePair<string, double>> conversionList)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 0)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<CoinDto> list = new List<CoinDto>();

                        string[] entries = Regex.Split(value, "\\" + _searchParameter);
                        for (int index = 1; index < entries.Length; index++)
                        {
                            string entry = entries[index];
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = Regex.Split(replacedEntry, "\\};");
                            CoinDto newValue = parseStringToValue(data, conversionList);
                            if (newValue != null)
                            {
                                list.Add(newValue);
                            }
                        }

                        return list;
                    }
                }
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return new List<CoinDto>();
        }

        private CoinDto parseStringToValue(string[] data, IList<KeyValuePair<string, double>> conversionList)
        {
            if (data.Length == 4)
            {
                if (data[0].Contains("{Id:")
                    && data[1].Contains("{User:")
                    && data[2].Contains("{Type:")
                    && data[3].Contains("{Amount:"))
                {

                    string idString = data[0].Replace("{Id:", "").Replace("};", "");
                    int id = -1;
                    bool parseSuccessId = int.TryParse(idString, out id);
                    if (!parseSuccessId)
                    {
                        _logger.Warning("Failed to parse id from data!");
                    }

                    string user = data[1].Replace("{User:", "").Replace("};", "");

                    string type = data[2].Replace("{Type:", "").Replace("};", "");

                    string numberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    string amountString = data[3].Replace("{Amount:", "").Replace("};", "").Replace(".", numberDecimalSeparator);
                    double amount = -1;
                    bool parseSuccessAmount = double.TryParse(amountString, out amount);
                    if (!parseSuccessAmount)
                    {
                        _logger.Error("Failed to parse amount from data!");
                        return null;
                    }

                    double currentConversion = 0;
                    foreach (KeyValuePair<string, double> entry in conversionList)
                    {
                        if (entry.Key.Contains(type))
                        {
                            currentConversion = entry.Value;
                            break;
                        }
                    }

                    return new CoinDto(id, user, type, amount, currentConversion, CoinDto.Trend.NULL);
                }
                else
                {
                    _logger.Error("data contains invalid entries!");
                }
            }
            else
            {
                _logger.Error(string.Format("Data has invalid length {0}", data.Length));
            }

            _logger.Error(string.Format("{0} has an error!", data));

            return null;
        }
    }
}
