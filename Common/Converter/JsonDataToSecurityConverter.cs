using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Common.Dto.SecurityDto;

namespace Common.Converter
{
    public class JsonDataToSecurityConverter : IJsonDataConverter<SecurityDto>
    {
        private const string TAG = "JsonDataToSecurityConverter";
        private static string _searchParameter = "{MotionData:";

        private readonly Logger _logger;

        public JsonDataToSecurityConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<SecurityDto> GetList(string[] stringArray)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return parseStringToList(stringArray[0]);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, _searchParameter);
                return parseStringToList(usedEntry);
            }
        }

        public IList<SecurityDto> GetList(string responseString)
        {
            return parseStringToList(responseString);
        }

        private IList<SecurityDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 0)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<SecurityDto> list = new List<SecurityDto>();

                        string[] entries = Regex.Split(value, "\\" + _searchParameter);
                        for (int index = 1; index < entries.Length; index++)
                        {
                            string entry = entries[index];
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = Regex.Split(replacedEntry, "\\};");
                            SecurityDto newValue = parseStringToValue(data);
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

            return new List<SecurityDto>();
        }

        private SecurityDto parseStringToValue(string[] data)
        {
            if (data.Length == 4)
            {
                if (data[0].Contains("{State:")
                    && data[1].Contains("{URL:")
                    && data[2].Contains("{MotionEvents:")
                    && data[3].Contains("{Control:"))
                {

                    string stateString = data[0].Replace("{State:", "").Replace("};", "");
                    bool isCameraActive = stateString.Contains("1");

                    string controlStateString = data[3].Replace("{Control:", "").Replace("};", "");
                    bool isCameraControlActive = controlStateString.Contains("1");

                    string url = data[1].Replace("{URL:", "").Replace("};", "");

                    string motionEvents = data[2].Replace("{MotionEvents:", "").Replace("};", "");
                    string[] registeredMotionEventStringArray = Regex.Split(motionEvents, "\\};");

                    IList<RegisteredEventDto> registeredMotionEvents = new List<RegisteredEventDto>();
                    for (int index = 0; index < registeredMotionEventStringArray.Length; index++)
                    {
                        string registeredMotionEventString = registeredMotionEventStringArray[index];
                        string registeredMotionEvent = registeredMotionEventString.Replace("{Event:", "").Replace("},", "");
                        registeredMotionEvents.Add(new RegisteredEventDto(index, registeredMotionEvent));
                    }

                    return new SecurityDto(isCameraActive, isCameraControlActive, url, registeredMotionEvents);
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
