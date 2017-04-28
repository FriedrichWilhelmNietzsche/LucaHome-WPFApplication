using Common.Dto;
using Common.Tools;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;

namespace Common.Converter
{
    public class JsonDataToShoppingConverter
    {
        private const string TAG = "JsonDataToShoppingConverter";
        private static string _searchParameter = "{shopping_entry:";

        private static Logger _logger;
        
        public JsonDataToShoppingConverter()
        {
            _logger = new Logger(TAG);
        }

        public static IList<ShoppingEntryDto> GetList(string[] stringArray)
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

        private static IList<ShoppingEntryDto> ParseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 1)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<ShoppingEntryDto> list = new List<ShoppingEntryDto>();
                        
                        string[] entries = value.Split(new string[] { "\\" + _searchParameter }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string entry in entries)
                        {
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = replacedEntry.Split(new string[] { "\\};" }, StringSplitOptions.RemoveEmptyEntries);
                            ShoppingEntryDto newValue = ParseStringToValue(data);
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

        private static ShoppingEntryDto ParseStringToValue(string[] data)
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
                    BitmapImage icon = parseIconFromString(groupString);

                    string quantityString = data[3].Replace("{quantity:", "").Replace("};", "");
                    int quantity = -1;
                    bool parseSuccessQuantity = int.TryParse(quantityString, out quantity);
                    if (!parseSuccessQuantity)
                    {
                        _logger.Warning("Failed to parse quantity from data!");
                    }

                    ShoppingEntryDto newValue = new ShoppingEntryDto(id, name, quantity, icon);
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

        private static BitmapImage parseIconFromString(string groupString)
        {
            switch (groupString)
            {
                case "Fruit":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/fruits.png"));
                case "Vegetable":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/vegetables.png"));
                case "Salat":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/salad.png"));
                case "Drinks":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/drinks.png"));
                case "Oil_and_vinegar":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/oil_vinegar.png"));
                case "Noodles":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/noodles.png"));
                case "Bread":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/bread.png"));
                case "Spread":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/spread.png"));
                case "Milk_product":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/milk.png"));
                case "Candy":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/candy.png"));
                case "Cleaning_material":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/cleaning.png"));
                case "Bath_utility":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/bath.png"));
                case "Leisure":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/leisure.png"));
                case "Baking":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/baking.png"));
                case "Cereals":
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/cereals.png"));
                case "Other":
                default:
                    return new BitmapImage(new Uri(new Uri("ms:appx"), "/Assets/Icons/shopping.png"));
            }
        }
    }
}
