using Common.Dto;
using Common.Tools;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Common.Converter
{
    public class JsonDataToWirelessSocketConverter
    {
        private const string TAG = "JsonDataToWirelessSocketConverter";
        private static string _searchParameter = "{socket:";

        private static Logger _logger;

        public JsonDataToWirelessSocketConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<WirelessSocketDto> GetList(string[] jsonStringArray)
        {
            if (StringHelper.StringsAreEqual(jsonStringArray))
            {
                return ParseStringToList(jsonStringArray[0]);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(jsonStringArray, _searchParameter);
                return ParseStringToList(usedEntry);
            }
        }

        public IList<WirelessSocketDto> GetList(string jsonString)
        {
            _logger.Debug(string.Format("GetList with jsonString {0}", jsonString));

            return ParseStringToList(jsonString);
        }

        private IList<WirelessSocketDto> ParseStringToList(string value)
        {
            _logger.Debug(string.Format("ParseStringToList with value {0}", value));

            if (!value.Contains("Error"))
            {
                _logger.Debug("ParseStringToList value has no Error!");

                int stringCount = StringHelper.GetStringCount(value, _searchParameter);
                if (stringCount > 1)
                {
                    _logger.Debug("ParseStringToList stringCount is larger then 1!");

                    if (value.Contains(_searchParameter))
                    {
                        _logger.Debug(string.Format("ParseStringToList value Contains _searchParameter {0}", _searchParameter));

                        IList<WirelessSocketDto> list = new List<WirelessSocketDto>();

                        string[] entries = Regex.Split(value, "\\" + _searchParameter);
                        for (int index = 1; index < entries.Length; index++)
                        {
                            string entry = entries[index];
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = Regex.Split(replacedEntry, "\\};");
                            WirelessSocketDto newValue = ParseStringToValue(index, data);
                            if (newValue != null)
                            {
                                list.Add(newValue);
                            }
                        }

                        return list;
                    }
                    else
                    {
                        _logger.Error(string.Format("Value {0} doesnot contain searchparameter {1}", value, _searchParameter));
                    }
                }
                else
                {
                    _logger.Error(string.Format("Value {0} has invalid stringCount {1}", value, stringCount));
                }
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return new List<WirelessSocketDto>();
        }

        private WirelessSocketDto ParseStringToValue(int id, string[] data)
        {
            if (data.Length == 4)
            {
                if (data[0].Contains("{Name:")
                    && data[1].Contains("{Area:")
                    && data[2].Contains("{Code:")
                    && data[3].Contains("{State:"))
                {
                    string name = data[0].Replace("{Name:", "").Replace("};", "");
                    string area = data[1].Replace("{Area:", "").Replace("};", "");
                    string code = data[2].Replace("{Code:", "").Replace("};", "");

                    string isActivatedString = data[3].Replace("{State:", "").Replace("};", "");
                    bool isActivated = isActivatedString.Contains("1");

                    return new WirelessSocketDto(id, name, area, code, isActivated);
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
