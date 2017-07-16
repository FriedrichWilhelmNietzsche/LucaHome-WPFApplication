using Common.Dto;
using Common.Tools;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Common.Converter
{
    public class JsonDataToCoinConverter
    {
        private const string TAG = "JsonDataToCoinConverter";
        private static string _searchParameter = "{coin:";

        private static Logger _logger;

        public JsonDataToCoinConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<CoinDto> GetList(string[] stringArray)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return ParseStringToList(stringArray[0]);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, _searchParameter);
                return ParseStringToList(usedEntry);
            }
        }

        public IList<CoinDto> GetList(string responseString)
        {
            return ParseStringToList(responseString);
        }

        private IList<CoinDto> ParseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 1)
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
                            CoinDto newValue = ParseStringToValue(data);
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

        private CoinDto ParseStringToValue(string[] data)
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

                    string amountString = data[3].Replace("{Amount:", "").Replace("};", "");
                    double amount = -1;
                    bool parseSuccessAmount = double.TryParse(amountString, out amount);
                    if (!parseSuccessAmount)
                    {
                        _logger.Error("Failed to parse amount from data!");
                        return null;
                    }

                    return new CoinDto(id, user, type, amount);
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
