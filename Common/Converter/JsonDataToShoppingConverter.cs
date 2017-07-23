using Common.Dto;
using Common.Enums;
using Common.Tools;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Common.Converter
{
    public class JsonDataToShoppingConverter
    {
        private const string TAG = "JsonDataToShoppingConverter";
        private static string _searchParameter = "{shopping_entry:";

        private readonly Logger _logger;

        public JsonDataToShoppingConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<ShoppingEntryDto> GetList(string[] stringArray)
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

        public IList<ShoppingEntryDto> GetList(string jsonString)
        {
            return parseStringToList(jsonString);
        }

        private IList<ShoppingEntryDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 0)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<ShoppingEntryDto> list = new List<ShoppingEntryDto>();

                        string[] entries = Regex.Split(value, "\\" + _searchParameter);
                        foreach (string entry in entries)
                        {
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = Regex.Split(replacedEntry, "\\};");
                            ShoppingEntryDto newValue = parseStringToValue(data);
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

            return new List<ShoppingEntryDto>();
        }

        private ShoppingEntryDto parseStringToValue(string[] data)
        {
            if (data.Length == 4)
            {
                if (data[0].Contains("{id:") && data[1].Contains("{name:") && data[2].Contains("{group:")
                        && data[3].Contains("{quantity:"))
                {

                    string idString = data[0].Replace("{id:", "").Replace("};", "");
                    int id = -1;
                    bool parseSuccessId = int.TryParse(idString, out id);
                    if (!parseSuccessId)
                    {
                        _logger.Warning("Failed to parse id from data!");
                    }

                    string name = data[1].Replace("{name:", "").Replace("};", "");

                    string groupString = data[2].Replace("{group:", "").Replace("};", "");
                    ShoppingEntryGroup shoppingEntryGroup = ShoppingEntryGroup.OTHER;
                    foreach (ShoppingEntryGroup entry in ShoppingEntryGroup.Values)
                    {
                        if (entry.Description.Contains(groupString))
                        {
                            shoppingEntryGroup = entry;
                            break;
                        }
                    }

                    string quantityString = data[3].Replace("{quantity:", "").Replace("};", "");
                    int quantity = -1;
                    bool parseSuccessQuantity = int.TryParse(quantityString, out quantity);
                    if (!parseSuccessQuantity)
                    {
                        _logger.Warning("Failed to parse quantity from data!");
                    }

                    ShoppingEntryDto newValue = new ShoppingEntryDto(id, name, shoppingEntryGroup, quantity);
                    return newValue;
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
