using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToMeterDataConverter : IJsonDataConverter<MeterDataDto>
    {
        private const string TAG = "JsonDataToMeterDataConverter";
        private static string _searchParameter = "{\"Data\":";

        private static JsonDataToMeterDataConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToMeterDataConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToMeterDataConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToMeterDataConverter();
                    }

                    return _instance;
                }
            }
        }

        public IList<MeterDataDto> GetList(string[] stringArray)
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

        public IList<MeterDataDto> GetList(string responseString)
        {
            return parseStringToList(responseString);
        }

        private IList<MeterDataDto> parseStringToList(string jsonString)
        {
            if (!jsonString.Contains("Error"))
            {
                IList<MeterDataDto> meterDataList = new List<MeterDataDto>();

                JObject jsonObject = JObject.Parse(jsonString);
                JToken jsonObjectData = jsonObject.GetValue("Data");

                foreach (JToken child in jsonObjectData.Children())
                {
                    JToken meterDataJsonData = child["MeterData"];

                    int id = int.Parse(meterDataJsonData["ID"].ToString());

                    string type = meterDataJsonData["Type"].ToString();
                    int typeId = int.Parse(meterDataJsonData["TypeId"].ToString());

                    JToken meterDataJsonDate = meterDataJsonData["Date"];

                    int day = int.Parse(meterDataJsonDate["Day"].ToString());
                    int month = int.Parse(meterDataJsonDate["Month"].ToString());
                    int year = int.Parse(meterDataJsonDate["Year"].ToString());

                    JToken meterDataJsonTime = meterDataJsonData["Time"];

                    int hour = int.Parse(meterDataJsonTime["Hour"].ToString());
                    int minute = int.Parse(meterDataJsonTime["Minute"].ToString());

                    DateTime saveDate = new DateTime(year, month, day, hour, minute, 0);

                    string meterId = meterDataJsonData["MeterId"].ToString();
                    string area = meterDataJsonData["Area"].ToString();

                    double value = double.Parse(meterDataJsonDate["Value"].ToString());
                    string imageName = meterDataJsonData["ImageName"].ToString();

                    MeterDataDto newMeterData = new MeterDataDto(id, type, typeId, saveDate, meterId, area, value, imageName);
                    meterDataList.Add(newMeterData);
                }

                return meterDataList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", jsonString));

            return new List<MeterDataDto>();
        }
    }
}
