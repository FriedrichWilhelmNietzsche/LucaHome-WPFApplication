using Common.Dto;
using Common.Enums;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToShoppingConverter : IJsonDataConverter<ShoppingEntryDto>
    {
        private const string TAG = "JsonDataToShoppingConverter";
        private static string _searchParameter = "{\"Data\":";

        private static JsonDataToShoppingConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToShoppingConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToShoppingConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToShoppingConverter();
                    }

                    return _instance;
                }
            }
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
                IList<ShoppingEntryDto> shoppingList = new List<ShoppingEntryDto>();

                try
                {
                    JObject jsonObject = JObject.Parse(value);
                    JToken jsonObjectData = jsonObject.GetValue("Data");

                    foreach (JToken child in jsonObjectData.Children())
                    {
                        JToken shoppingJsonData = child["ShoppingEntry"];

                        int id = int.Parse(shoppingJsonData["Id"].ToString());

                        string name = shoppingJsonData["Name"].ToString();
                        string group = shoppingJsonData["Group"].ToString();

                        int quantity = int.Parse(shoppingJsonData["Quantity"].ToString());
                        string unit = shoppingJsonData["Unit"].ToString();

                        ShoppingEntryDto newMenu = new ShoppingEntryDto(id, name, ShoppingEntryGroup.GetByDescription(group), quantity, unit);
                        shoppingList.Add(newMenu);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error(TAG, exception.Message);
                }

                return shoppingList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", value));

            return new List<ShoppingEntryDto>();
        }
    }
}
