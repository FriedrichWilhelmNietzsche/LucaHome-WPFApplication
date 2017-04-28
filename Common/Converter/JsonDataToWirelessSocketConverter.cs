using Common.Dto;
using Common.Tools;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;

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

        public static IList<WirelessSocketDto> GetList(string[] stringArray)
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

        private static IList<WirelessSocketDto> ParseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 1)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<WirelessSocketDto> list = new List<WirelessSocketDto>();
                        
                        string[] entries = value.Split(new string[] { "\\" + _searchParameter }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string entry in entries)
                        {
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = replacedEntry.Split(new string[] { "\\};" }, StringSplitOptions.RemoveEmptyEntries);
                            WirelessSocketDto newValue = ParseStringToValue(data);
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

            return null;
        }

        private static WirelessSocketDto ParseStringToValue(string[] data)
        {
            if (data.Length == 4)
            {
                if (data[0].Contains("{Name:") && data[1].Contains("{Area:") && data[2].Contains("{Code:")
                        && data[3].Contains("{State:"))
                {
                    string name = data[0].Replace("{Name:", "").Replace("};", "");
                    string area = data[1].Replace("{Area:", "").Replace("};", "");
                    string code = data[2].Replace("{Code:", "").Replace("};", "");

                    string isActivatedString = data[3].Replace("{State:", "").Replace("};", "");
                    bool isActivated = isActivatedString.Contains("1");

                    return new WirelessSocketDto(name, area, code, isActivated);
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
