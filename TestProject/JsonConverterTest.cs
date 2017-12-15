using Common.Converter;
using Common.Dto;
using Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TestProject
{
    [TestClass]
    public class JsonConverterTest
    {
        private const string TAG = "JsonConverterTest";

        [TestMethod]
        public void JsonDataToBirthdayConverterTest()
        {
            string response = "{\"Data\":[{\"Birthday\":{\"ID\":0,\"Name\":\"Jonas Schubert\",\"RemindMe\":0,\"SendMail\":0,\"Date\":{\"Day\":2,\"Month\":1,\"Year\":1990}}},{\"Birthday\":{\"ID\":1,\"Name\":\"Ruediger Schubert\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":10,\"Month\":1,\"Year\":1960}}},{\"Birthday\":{\"ID\":2,\"Name\":\"Bernhard Huber\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":20,\"Month\":1,\"Year\":1953}}},{\"Birthday\":{\"ID\":3,\"Name\":\"Janos Dehler\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":10,\"Month\":2,\"Year\":1990}}},{\"Birthday\":{\"ID\":4,\"Name\":\"Sandra Huber\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":12,\"Month\":2,\"Year\":1988}}},{\"Birthday\":{\"ID\":5,\"Name\":\"Karl-Heinz Nusser\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":19,\"Month\":2,\"Year\":1964}}},{\"Birthday\":{\"ID\":6,\"Name\":\"Udo Jahreiss\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":8,\"Month\":3,\"Year\":1966}}},{\"Birthday\":{\"ID\":7,\"Name\":\"Eva Schubert\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":14,\"Month\":3,\"Year\":1962}}},{\"Birthday\":{\"ID\":8,\"Name\":\"Artur Rychter\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":21,\"Month\":3,\"Year\":1990}}},{\"Birthday\":{\"ID\":9,\"Name\":\"Martin Huber\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":31,\"Month\":3,\"Year\":1986}}},{\"Birthday\":{\"ID\":10,\"Name\":\"Nina Weinhardt\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":4,\"Month\":4,\"Year\":1988}}},{\"Birthday\":{\"ID\":11,\"Name\":\"Sarah Schubert\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":22,\"Month\":4,\"Year\":1992}}},{\"Birthday\":{\"ID\":12,\"Name\":\"Susanne Stephan\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":21,\"Month\":5,\"Year\":1990}}},{\"Birthday\":{\"ID\":13,\"Name\":\"Kevin Poehland\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":25,\"Month\":5,\"Year\":1990}}},{\"Birthday\":{\"ID\":14,\"Name\":\"Dan (Sarah)\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":4,\"Month\":6,\"Year\":1993}}},{\"Birthday\":{\"ID\":15,\"Name\":\"Roman Otto\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":22,\"Month\":6,\"Year\":1986}}},{\"Birthday\":{\"ID\":16,\"Name\":\"Georg Wildholz\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":23,\"Month\":6,\"Year\":1988}}},{\"Birthday\":{\"ID\":17,\"Name\":\"Katerina Papanuskas\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":9,\"Month\":7,\"Year\":1992}}},{\"Birthday\":{\"ID\":18,\"Name\":\"Ronja Haag\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":13,\"Month\":7,\"Year\":1988}}},{\"Birthday\":{\"ID\":19,\"Name\":\"Maximilian Freiberg\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":25,\"Month\":7,\"Year\":1992}}},{\"Birthday\":{\"ID\":20,\"Name\":\"Chrissy Fausser\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":14,\"Month\":8,\"Year\":1988}}},{\"Birthday\":{\"ID\":21,\"Name\":\"Leila Rahal\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":4,\"Month\":11,\"Year\":1992}}},{\"Birthday\":{\"ID\":22,\"Name\":\"Jan-Olaf Becker\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":10,\"Month\":11,\"Year\":1987}}},{\"Birthday\":{\"ID\":23,\"Name\":\"Rebecca Schubert\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":10,\"Month\":12,\"Year\":1994}}},{\"Birthday\":{\"ID\":24,\"Name\":\"Marina Heinel\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":11,\"Month\":12,\"Year\":1993}}},{\"Birthday\":{\"ID\":25,\"Name\":\"Waltraud Huber\",\"RemindMe\":1,\"SendMail\":0,\"Date\":{\"Day\":11,\"Month\":12,\"Year\":1956}}}]} ";

            JsonDataToBirthdayConverter jsonDataToBirthdayConverter = new JsonDataToBirthdayConverter();
            IList<BirthdayDto> birthdayList = jsonDataToBirthdayConverter.GetList(response);

            Assert.IsNotNull(birthdayList);
            Assert.AreEqual(birthdayList.Count, 26);
            Assert.AreEqual(birthdayList[0].Name, "Jonas Schubert");
        }

        [TestMethod]
        public void JsonDataToChangeConverterTest()
        {
            string response = "{\"Data\":[{\"Change\":{\"Type\":\"Birthdays\",\"UserName\":\"Jonas\",\"Date\":{\"Day\":13,\"Month\":12,\"Year\":2017},\"Time\":{\"Hour\":20,\"Minute\":5}}},{\"Change\":{\"Type\":\"Coins\",\"UserName\":\"Jonas\",\"Date\":{\"Day\":13,\"Month\":12,\"Year\":2017},\"Time\":{\"Hour\":20,\"Minute\":5}}},{\"Change\":{\"Type\":\"MapContent\",\"UserName\":\"Jonas\",\"Date\":{\"Day\":13,\"Month\":12,\"Year\":2017},\"Time\":{\"Hour\":20,\"Minute\":5}}},{\"Change\":{\"Type\":\"Menu\",\"UserName\":\"Jonas\",\"Date\":{\"Day\":13,\"Month\":12,\"Year\":2017},\"Time\":{\"Hour\":20,\"Minute\":5}}},{\"Change\":{\"Type\":\"Settings\",\"UserName\":\"Jonas\",\"Date\":{\"Day\":13,\"Month\":12,\"Year\":2017},\"Time\":{\"Hour\":20,\"Minute\":5}}},{\"Change\":{\"Type\":\"ShoppingList\",\"UserName\":\"Jonas\",\"Date\":{\"Day\":13,\"Month\":12,\"Year\":2017},\"Time\":{\"Hour\":20,\"Minute\":5}}}]} ";

            JsonDataToChangeConverter jsonDataToChangeConverter = new JsonDataToChangeConverter();
            IList<ChangeDto> changeList = jsonDataToChangeConverter.GetList(response);

            Assert.IsNotNull(changeList);
            Assert.AreEqual(changeList.Count, 6);
            Assert.AreEqual(changeList[0].User, "Jonas");
        }

        [TestMethod]
        public void JsonDataToCoinConversionConverterTest()
        {
            string response = "{\"BTC\":{\"EUR\":1989.62},\"DASH\":{\"EUR\":144.12},\"ETC\":{\"EUR\":13.12},\"ETH\":{\"EUR\":185.94},\"LTC\":{\"EUR\":35.93},\"XMR\":{\"EUR\":31.17},\"ZEC\":{\"EUR\":174}}";

            JsonDataToCoinConversionConverter jsonDataToCoinConversionConverter = new JsonDataToCoinConversionConverter();
            IList<KeyValuePair<string, double>> conversionList = jsonDataToCoinConversionConverter.GetList(response);

            Assert.AreEqual(conversionList.Count, 7);
        }

        [TestMethod]
        public void JsonDataToCoinConverterTest()
        {
            string response = "{\"Data\":[{\"Coin\":{\"ID\":1,\"User\":\"Jonas\",\"Type\":\"ETC\",\"Amount\":4}},{\"Coin\":{\"ID\":2,\"User\":\"Jonas\",\"Type\":\"ETH\",\"Amount\":4}},{\"Coin\":{\"ID\":3,\"User\":\"Jonas\",\"Type\":\"LTC\",\"Amount\":4}},{\"Coin\":{\"ID\":4,\"User\":\"Jonas\",\"Type\":\"IOTA\",\"Amount\":264}}]} ";

            JsonDataToCoinConverter jsonDataToCoinConverter = new JsonDataToCoinConverter();
            IList<CoinDto> coinList = jsonDataToCoinConverter.GetList(response, new List<KeyValuePair<string, double>>());

            Assert.IsNotNull(coinList);
            Assert.AreEqual(coinList.Count, 4);
            Assert.AreEqual(coinList[0].Type, "ETC");
        }

        [TestMethod]
        public void JsonDataToCoinTrendConverterTest()
        {
            CoinDto coin = new CoinDto(1, "Jonas", "BTC", 0.25623, 5717.12, CoinDto.Trend.NULL);
            string response = "{\"Response\":\"Success\",\"Type\":100,\"Aggregated\":true,\"Data\":[{\"time\":1509526800,\"close\":5701.01,\"high\":5731.75,\"low\":5567.39,\"open\":5568.18,\"volumefrom\":5069.370000000001,\"volumeto\":28905115.34},{\"time\":1509537600,\"close\":5683.31,\"high\":5706.18,\"low\":5631.76,\"open\":5701.01,\"volumefrom\":3043.34,\"volumeto\":17265365.919999998},{\"time\":1509548400,\"close\":5737.8,\"high\":5759.6,\"low\":5671.74,\"open\":5683.31,\"volumefrom\":2867.68,\"volumeto\":16420272.52},{\"time\":1509559200,\"close\":5727.92,\"high\":5743.72,\"low\":5690.27,\"open\":5743.43,\"volumefrom\":1774.25,\"volumeto\":10127145.39},{\"time\":1509570000,\"close\":5870.89,\"high\":5871.48,\"low\":5717.72,\"open\":5727.23,\"volumefrom\":1840.0499999999997,\"volumeto\":10577935.32},{\"time\":1509580800,\"close\":5893.69,\"high\":5967.44,\"low\":5816.51,\"open\":5870.89,\"volumefrom\":1933.44,\"volumeto\":11355695.91},{\"time\":1509591600,\"close\":5931.23,\"high\":5947.51,\"low\":5885.83,\"open\":5897.27,\"volumefrom\":1056.1100000000001,\"volumeto\":6221662.460000001},{\"time\":1509602400,\"close\":6096.4,\"high\":6098.09,\"low\":5926.82,\"open\":5931.23,\"volumefrom\":3175.85,\"volumeto\":19039197.2},{\"time\":1509613200,\"close\":6039.48,\"high\":6426.61,\"low\":5779.65,\"open\":6093.49,\"volumefrom\":7850.2,\"volumeto\":47803459.3},{\"time\":1509624000,\"close\":6087.96,\"high\":6242.89,\"low\":5831.48,\"open\":6039.14,\"volumefrom\":4806.23,\"volumeto\":29064349.4},{\"time\":1509634800,\"close\":6080.03,\"high\":6165.98,\"low\":5945.54,\"open\":6090.11,\"volumefrom\":3146.4,\"volumeto\":19045486.759999998},{\"time\":1509645600,\"close\":6084.4,\"high\":6123.23,\"low\":5939.34,\"open\":6079.93,\"volumefrom\":2860.61,\"volumeto\":17153807.4},{\"time\":1509656400,\"close\":6077.35,\"high\":6124.34,\"low\":6062.22,\"open\":6084.4,\"volumefrom\":1538.7400000000002,\"volumeto\":9335896.53},{\"time\":1509667200,\"close\":6074.23,\"high\":6101.29,\"low\":6014.6,\"open\":6077.35,\"volumefrom\":1054.1399999999999,\"volumeto\":6332995.869999999},{\"time\":1509678000,\"close\":6227.98,\"high\":6229.75,\"low\":6073.99,\"open\":6074.3,\"volumefrom\":1258.25,\"volumeto\":7727202.23},{\"time\":1509688800,\"close\":6418.95,\"high\":6420.19,\"low\":6218.89,\"open\":6227.98,\"volumefrom\":2811.1400000000003,\"volumeto\":17611338.57},{\"time\":1509699600,\"close\":6335.81,\"high\":6447.05,\"low\":6287.77,\"open\":6418.95,\"volumefrom\":4276.21,\"volumeto\":27126568.7},{\"time\":1509710400,\"close\":6332.87,\"high\":6362.85,\"low\":6246.37,\"open\":6332.68,\"volumefrom\":3022.9300000000003,\"volumeto\":18933861.06},{\"time\":1509721200,\"close\":6360.81,\"high\":6392.17,\"low\":6322.82,\"open\":6332.87,\"volumefrom\":2151.13,\"volumeto\":13640346.049999999},{\"time\":1509732000,\"close\":6302.36,\"high\":6399.26,\"low\":6301.99,\"open\":6360.82,\"volumefrom\":1560.4499999999998,\"volumeto\":9881534.65},{\"time\":1509742800,\"close\":6248.98,\"high\":6311.22,\"low\":6187.69,\"open\":6303.14,\"volumefrom\":2538.9,\"volumeto\":15771730.52},{\"time\":1509753600,\"close\":6219.44,\"high\":6250.01,\"low\":6079.32,\"open\":6248.98,\"volumefrom\":1933.16,\"volumeto\":11848070.45},{\"time\":1509764400,\"close\":6217.52,\"high\":6283.66,\"low\":6202.47,\"open\":6219.44,\"volumefrom\":657.5999999999999,\"volumeto\":4081269.96},{\"time\":1509775200,\"close\":6218.61,\"high\":6275.6,\"low\":6176.74,\"open\":6218.12,\"volumefrom\":834.12,\"volumeto\":5177045.94},{\"time\":1509786000,\"close\":6205.33,\"high\":6243.29,\"low\":6171.01,\"open\":6218.61,\"volumefrom\":875.45,\"volumeto\":5411794.54}],\"TimeTo\":1509793200,\"TimeFrom\":1509526800,\"FirstValueInArray\":true,\"ConversionType\":{\"type\":\"direct\",\"conversionSymbol\":\"\"}}";

            JsonDataToCoinTrendConverter jsonDataToCoinTrendConverter = new JsonDataToCoinTrendConverter();
            coin = jsonDataToCoinTrendConverter.UpdateTrend(coin, response, "EUR");

            Assert.AreNotEqual(coin.CurrentTrend, CoinDto.Trend.NULL);
        }

        [TestMethod]
        public void JsonDataToListedMenuConverterTest()
        {
            string response = "{\"Data\":[{\"ListedMenu\":{\"ID\":1,\"Title\":\"Bratlinge mit Reis\",\"Description\":\"\",\"UseCounter\":4,\"Rating\":0}},{\"ListedMenu\":{\"ID\":2,\"Title\":\"Pizza\",\"Description\":\"\",\"UseCounter\":5,\"Rating\":0}},{\"ListedMenu\":{\"ID\":3,\"Title\":\"Sojagyros\",\"Description\":\"\",\"UseCounter\":5,\"Rating\":0}},{\"ListedMenu\":{\"ID\":4,\"Title\":\"Sojalasagne\",\"Description\":\"\",\"UseCounter\":4,\"Rating\":0}},{\"ListedMenu\":{\"ID\":5,\"Title\":\"Nudeln mit Walnuss-Oliven/getrocknete Tomaten Sauce\",\"Description\":\"\",\"UseCounter\":4,\"Rating\":0}},{\"ListedMenu\":{\"ID\":6,\"Title\":\"Quiche\",\"Description\":\"\",\"UseCounter\":5,\"Rating\":0}},{\"ListedMenu\":{\"ID\":7,\"Title\":\"Flammkuchen\",\"Description\":\"\",\"UseCounter\":5,\"Rating\":0}},{\"ListedMenu\":{\"ID\":8,\"Title\":\"Zwiebelkuchen\",\"Description\":\"\",\"UseCounter\":4,\"Rating\":0}},{\"ListedMenu\":{\"ID\":9,\"Title\":\"Indischer Eintopf\",\"Description\":\"\",\"UseCounter\":3,\"Rating\":0}},{\"ListedMenu\":{\"ID\":10,\"Title\":\"Tortellini\",\"Description\":\"\",\"UseCounter\":3,\"Rating\":0}},{\"ListedMenu\":{\"ID\":11,\"Title\":\"Ueberbackene Maultaschen\",\"Description\":\"\",\"UseCounter\":4,\"Rating\":0}},{\"ListedMenu\":{\"ID\":12,\"Title\":\"Kartoffelauflauf\",\"Description\":\"\",\"UseCounter\":3,\"Rating\":0}},{\"ListedMenu\":{\"ID\":13,\"Title\":\"Kartoffel-Spinat-Feta Auflauf\",\"Description\":\"\",\"UseCounter\":3,\"Rating\":0}},{\"ListedMenu\":{\"ID\":14,\"Title\":\"Auberginen-Tomaten-Auflauf\",\"Description\":\"\",\"UseCounter\":3,\"Rating\":0}},{\"ListedMenu\":{\"ID\":15,\"Title\":\"Sheperds Pie\",\"Description\":\"\",\"UseCounter\":4,\"Rating\":0}},{\"ListedMenu\":{\"ID\":16,\"Title\":\"Kichererbsenbraten/Nussbraten\",\"Description\":\"\",\"UseCounter\":5,\"Rating\":0}},{\"ListedMenu\":{\"ID\":17,\"Title\":\"Couscous-Gemuesepfanne\",\"Description\":\"\",\"UseCounter\":4,\"Rating\":0}},{\"ListedMenu\":{\"ID\":18,\"Title\":\"Brokkoli-Mix-Salat\",\"Description\":\"\",\"UseCounter\":5,\"Rating\":0}}]} ";

            JsonDataToListedMenuConverter jsonDataToListedMenuConverter = new JsonDataToListedMenuConverter();
            IList<ListedMenuDto> listedMenuList = jsonDataToListedMenuConverter.GetList(response);

            Assert.IsNotNull(listedMenuList);
            Assert.AreEqual(listedMenuList.Count, 18);
            Assert.AreEqual(listedMenuList[0].Title, "Bratlinge mit Reis");
        }

        [TestMethod]
        public void JsonDataToMapContentConverterTest()
        {
            string response = "{\"Data\":[{\"MapContent\":{\"ID\":1,\"Type\":\"WirelessSocket\",\"TypeId\":1,\"Name\":\"Light_Sleeping\",\"ShortName\":\"LiSlSo\",\"Area\":\"Sleeping_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":4,\"Y\":3}}}},{\"MapContent\":{\"ID\":2,\"Type\":\"WirelessSocket\",\"TypeId\":2,\"Name\":\"MediaServer_Sleeping\",\"ShortName\":\"MsSlSo\",\"Area\":\"Sleeping_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":30,\"Y\":3}}}},{\"MapContent\":{\"ID\":3,\"Type\":\"WirelessSocket\",\"TypeId\":3,\"Name\":\"MediaServer_Living\",\"ShortName\":\"MsLiSo\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":76,\"Y\":48}}}},{\"MapContent\":{\"ID\":4,\"Type\":\"WirelessSocket\",\"TypeId\":4,\"Name\":\"MediaServer_Kitchen\",\"ShortName\":\"MsKiSo\",\"Area\":\"Kitchen\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":89,\"Y\":54}}}},{\"MapContent\":{\"ID\":5,\"Type\":\"WirelessSocket\",\"TypeId\":5,\"Name\":\"WorkStation_Jonas\",\"ShortName\":\"WsJoSo\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":60,\"Y\":2}}}},{\"MapContent\":{\"ID\":6,\"Type\":\"WirelessSocket\",\"TypeId\":6,\"Name\":\"TV\",\"ShortName\":\"TvLiSo\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":99,\"Y\":3}}}},{\"MapContent\":{\"ID\":7,\"Type\":\"WirelessSocket\",\"TypeId\":7,\"Name\":\"Light_Living\",\"ShortName\":\"LiLiSo\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":65,\"Y\":2}}}},{\"MapContent\":{\"ID\":8,\"Type\":\"WirelessSocket\",\"TypeId\":8,\"Name\":\"RaspberryPi_Media\",\"ShortName\":\"RPiMeSo\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":99,\"Y\":18}}}},{\"MapContent\":{\"ID\":9,\"Type\":\"LAN\",\"TypeId\":1,\"Name\":\"LAN1\",\"ShortName\":\"LAN1\",\"Area\":\"Hall\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":43,\"Y\":27}}}},{\"MapContent\":{\"ID\":10,\"Type\":\"LAN\",\"TypeId\":2,\"Name\":\"LAN2\",\"ShortName\":\"LAN2\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":65,\"Y\":10}}}},{\"MapContent\":{\"ID\":11,\"Type\":\"LAN\",\"TypeId\":3,\"Name\":\"LAN3\",\"ShortName\":\"LAN3\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":99,\"Y\":10}}}},{\"MapContent\":{\"ID\":12,\"Type\":\"MediaServer\",\"TypeId\":1,\"Name\":\"MediaServer_Sleeping\",\"ShortName\":\"MsSl\",\"Area\":\"Sleeping_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":21,\"Y\":3}}}},{\"MapContent\":{\"ID\":13,\"Type\":\"MediaServer\",\"TypeId\":2,\"Name\":\"MediaServer_Living\",\"ShortName\":\"MsLi\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":71,\"Y\":48}}}},{\"MapContent\":{\"ID\":14,\"Type\":\"MediaServer\",\"TypeId\":3,\"Name\":\"MediaServer_Kitchen\",\"ShortName\":\"MsKi\",\"Area\":\"Kitchen\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":93,\"Y\":54}}}},{\"MapContent\":{\"ID\":15,\"Type\":\"RaspberryPi\",\"TypeId\":1,\"Name\":\"RaspberryPi_Server\",\"ShortName\":\"RPiS\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":60,\"Y\":10}}}},{\"MapContent\":{\"ID\":16,\"Type\":\"RaspberryPi\",\"TypeId\":2,\"Name\":\"RaspberryPi_Media\",\"ShortName\":\"RPiM\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":94,\"Y\":18}}}},{\"MapContent\":{\"ID\":17,\"Type\":\"NAS\",\"TypeId\":1,\"Name\":\"NAS1\",\"ShortName\":\"NAS1\",\"Area\":\"Hall\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":43,\"Y\":30}}}},{\"MapContent\":{\"ID\":18,\"Type\":\"NAS\",\"TypeId\":2,\"Name\":\"NAS2\",\"ShortName\":\"NAS2\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":94,\"Y\":10}}}},{\"MapContent\":{\"ID\":19,\"Type\":\"LightSwitch\",\"TypeId\":1,\"Name\":\"LightSwitch_Hall\",\"ShortName\":\"LSHa\",\"Area\":\"Hall\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":43,\"Y\":43}}}},{\"MapContent\":{\"ID\":20,\"Type\":\"Temperature\",\"TypeId\":1,\"Name\":\"Temperature_LivingRoom1\",\"ShortName\":\"TLi1\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":65,\"Y\":18}}}},{\"MapContent\":{\"ID\":21,\"Type\":\"Temperature\",\"TypeId\":2,\"Name\":\"Temperature_SleepingRoom\",\"ShortName\":\"TSl\",\"Area\":\"Sleeping_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":10,\"Y\":40}}}},{\"MapContent\":{\"ID\":22,\"Type\":\"Temperature\",\"TypeId\":3,\"Name\":\"Temperature_Kitchen\",\"ShortName\":\"TKi\",\"Area\":\"Kitchen\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":99,\"Y\":62}}}},{\"MapContent\":{\"ID\":23,\"Type\":\"Temperature\",\"TypeId\":4,\"Name\":\"Temperature_LivingRoom2\",\"ShortName\":\"TLi2\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":85,\"Y\":2}}}},{\"MapContent\":{\"ID\":24,\"Type\":\"Temperature\",\"TypeId\":5,\"Name\":\"Temperature_Bath\",\"ShortName\":\"TBa\",\"Area\":\"Bath\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":57,\"Y\":74}}}},{\"MapContent\":{\"ID\":25,\"Type\":\"Temperature\",\"TypeId\":6,\"Name\":\"Temperature_Hall\",\"ShortName\":\"THa\",\"Area\":\"Hall\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":48,\"Y\":30}}}},{\"MapContent\":{\"ID\":26,\"Type\":\"Temperature\",\"TypeId\":7,\"Name\":\"Temperature_Outdoor\",\"ShortName\":\"TO\",\"Area\":\"Outdoor\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":100,\"Y\":36}}}},{\"MapContent\":{\"ID\":27,\"Type\":\"PuckJS\",\"TypeId\":1,\"Name\":\"PuckJS_SleepingRoom\",\"ShortName\":\"PSl\",\"Area\":\"Sleeping_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":5,\"Y\":40}}}},{\"MapContent\":{\"ID\":28,\"Type\":\"PuckJS\",\"TypeId\":2,\"Name\":\"PuckJS_Kitchen\",\"ShortName\":\"PKi\",\"Area\":\"Kitchen\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":99,\"Y\":58}}}},{\"MapContent\":{\"ID\":29,\"Type\":\"PuckJS\",\"TypeId\":3,\"Name\":\"PuckJS_LivingRoom\",\"ShortName\":\"PSl\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":80,\"Y\":2}}}},{\"MapContent\":{\"ID\":30,\"Type\":\"PuckJS\",\"TypeId\":4,\"Name\":\"PuckJS_Bath\",\"ShortName\":\"PBa\",\"Area\":\"Bath\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":62,\"Y\":74}}}},{\"MapContent\":{\"ID\":31,\"Type\":\"PuckJS\",\"TypeId\":5,\"Name\":\"PuckJS_Hall\",\"ShortName\":\"PHa\",\"Area\":\"Hall\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":48,\"Y\":30}}}},{\"MapContent\":{\"ID\":32,\"Type\":\"Menu\",\"TypeId\":1,\"Name\":\"ListedMenu\",\"ShortName\":\"L\",\"Area\":\"Kitchen\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":83,\"Y\":66}}}},{\"MapContent\":{\"ID\":33,\"Type\":\"Menu\",\"TypeId\":2,\"Name\":\"Menu\",\"ShortName\":\"M\",\"Area\":\"Kitchen\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":83,\"Y\":74}}}},{\"MapContent\":{\"ID\":34,\"Type\":\"ShoppingList\",\"TypeId\":1,\"Name\":\"ShoppingList\",\"ShortName\":\"S\",\"Area\":\"Hall\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":48,\"Y\":62}}}},{\"MapContent\":{\"ID\":35,\"Type\":\"Camera\",\"TypeId\":1,\"Name\":\"Camera_Living\",\"ShortName\":\"C\",\"Area\":\"Living_Room\",\"Visibility\":1,\"Position\":{\"Point\":{\"X\":60,\"Y\":18}}}}]} ";

            JsonDataToMapContentConverter jsonDataToMapContentConverter = new JsonDataToMapContentConverter();
            IList<MapContentDto> mapContentList = jsonDataToMapContentConverter.GetList(response, new List<ListedMenuDto>(), new List<MenuDto>(), new List<ShoppingEntryDto>(), new List<MediaServerDto>(), new List<SecurityDto>(), new List<TemperatureDto>(), new List<WirelessSocketDto>(), new List<WirelessSwitchDto>());

            Assert.IsNotNull(mapContentList);
            Assert.AreEqual(mapContentList.Count, 35);
            Assert.AreEqual(mapContentList[0].Name, "Light_Sleeping");
        }

        [TestMethod]
        public void JsonDataToMenuConverterTest()
        {
            string response = "{\"Data\":[{\"Menu\":{\"Title\":\"-\",\"Description\":\"-\",\"Weekday\":\"monday\",\"Date\":{\"Day\":11,\"Month\":12,\"Year\":2017}}},{\"Menu\":{\"Title\":\"-\",\"Description\":\"-\",\"Weekday\":\"tuesday\",\"Date\":{\"Day\":12,\"Month\":12,\"Year\":2017}}},{\"Menu\":{\"Title\":\"-\",\"Description\":\"-\",\"Weekday\":\"wednesday\",\"Date\":{\"Day\":13,\"Month\":12,\"Year\":2017}}},{\"Menu\":{\"Title\":\"-\",\"Description\":\"-\",\"Weekday\":\"thursday\",\"Date\":{\"Day\":14,\"Month\":12,\"Year\":2017}}},{\"Menu\":{\"Title\":\"-\",\"Description\":\"-\",\"Weekday\":\"friday\",\"Date\":{\"Day\":15,\"Month\":12,\"Year\":2017}}},{\"Menu\":{\"Title\":\"-\",\"Description\":\"-\",\"Weekday\":\"saturday\",\"Date\":{\"Day\":16,\"Month\":12,\"Year\":2017}}},{\"Menu\":{\"Title\":\"-\",\"Description\":\"-\",\"Weekday\":\"sunday\",\"Date\":{\"Day\":17,\"Month\":12,\"Year\":2017}}}]} ";

            JsonDataToMenuConverter jsonDataToMenuConverter = new JsonDataToMenuConverter();
            IList<MenuDto> menuList = jsonDataToMenuConverter.GetList(response);

            Assert.IsNotNull(menuList);
            Assert.AreEqual(menuList.Count, 7);
            Assert.AreEqual(menuList[0].Date.DayOfWeek, System.DayOfWeek.Monday);
        }

        [TestMethod]
        public void JsonDataToMovieConverterTest()
        {
            string response = "{\"Data\":[{\"Movie\":{\"Title\":\"Fluch der Karibik III - Am Ende der Welt\",\"Genre\":\"- \",\"Description\":\"- \",\"Rating\":0,\"Watched\":0}},{\"Movie\":{\"Title\":\"Fluch der Karibik IV - Fremde Gezeiten\",\"Genre\":\"- \",\"Description\":\"- \",\"Rating\":0,\"Watched\":0}},{\"Movie\":{\"Title\":\"Forrest Gump\",\"Genre\":\"- \",\"Description\":\"- \",\"Rating\":0,\"Watched\":0}}]} ";

            JsonDataToMovieConverter jsonDataToMovieConverter = new JsonDataToMovieConverter();
            IList<MovieDto> movieList = jsonDataToMovieConverter.GetList(response);

            Assert.IsNotNull(movieList);
            Assert.AreEqual(movieList.Count, 3);
            Assert.AreEqual(movieList[0].Title, "Fluch der Karibik III - Am Ende der Welt");
        }

        [TestMethod]
        public void JsonDataToScheduleConverterTest()
        {
            string response = "{\"Data\":[{\"Schedule\":{\"Name\":\"Enable_Sleep_Light\",\"Socket\":\"Light_Sleeping\",\"Gpio\":\"\",\"Switch\":\"\",\"Weekday\":0,\"Hour\":20,\"Minute\":33,\"OnOff\":1,\"IsTimer\":0,\"State\":1}},{\"Schedule\":{\"Name\":\"Disable_Living\",\"Socket\":\"Light_Living\",\"Gpio\":\"\",\"Switch\":\"\",\"Weekday\":2,\"Hour\":11,\"Minute\":15,\"OnOff\":0,\"IsTimer\":1,\"State\":1}}]} ";

            JsonDataToScheduleConverter jsonDataToScheduleConverter = new JsonDataToScheduleConverter();
            IList<ScheduleDto> scheduleList = jsonDataToScheduleConverter.GetList(response, new List<WirelessSocketDto>(), new List<WirelessSwitchDto>());

            Assert.IsNotNull(scheduleList);
            Assert.AreEqual(scheduleList.Count, 1);
            Assert.AreEqual(scheduleList[0].Name, "Enable_Sleep_Light");
        }

        [TestMethod]
        public void JsonDataToSecurityConverterTest()
        {
            string response = "{\"MotionData\":{\"State\":\"OFF\",\"Control\":\"OFF\",\"URL\":\"192.168.178.25:8081\",\"Events\":[]}}";

            JsonDataToSecurityConverter jsonDataToSecurityConverter = new JsonDataToSecurityConverter();
            IList<SecurityDto> securityList = jsonDataToSecurityConverter.GetList(response);

            Assert.IsNotNull(securityList);
            Assert.AreEqual(securityList.Count, 1);
            Assert.IsFalse(securityList[0].IsCameraActive);
        }

        [TestMethod]
        public void JsonDataToShoppingListConverterTest()
        {
            string response = "{\"Data\":[{\"ShoppingEntry\":{\"ID\":1,\"Name\":\"Mehl\",\"Group\":\"Baking\",\"Quantity\":2}},{\"ShoppingEntry\":{\"ID\":1,\"Name\":\"Apfel\",\"Group\":\"Fruit\",\"Quantity\":4}}]} ";

            JsonDataToShoppingConverter jsonDataToShoppingConverter = new JsonDataToShoppingConverter();
            IList<ShoppingEntryDto> shoppingList = jsonDataToShoppingConverter.GetList(response);

            Assert.IsNotNull(shoppingList);
            Assert.AreEqual(shoppingList.Count, 2);
            Assert.AreEqual(shoppingList[0].Group, ShoppingEntryGroup.BAKING);
        }

        [TestMethod]
        public void JsonDataToTemperatureConverterTest()
        {
            string response = "{\"Temperature\":{\"Value\":17.562,\"Area\":\"Workspace_Jonas\",\"SensorPath\":\"/sys/bus/w1/devices/28-000006f437d1/w1_slave\",\"GraphPath\":\"192.168.178.25/cgi-bin/webgui.py\"}}";

            JsonDataToTemperatureConverter jsonDataToTemperatureConverter = new JsonDataToTemperatureConverter();
            IList<TemperatureDto> temperatureList = jsonDataToTemperatureConverter.GetList(response);

            Assert.IsNotNull(temperatureList);
            Assert.AreEqual(temperatureList.Count, 1);
            Assert.AreEqual(temperatureList[0].Area, "Workspace_Jonas");
        }

        [TestMethod]
        public void JsonDataToTimerConverterTest()
        {
            string response = "{\"Data\":[{\"Schedule\":{\"Name\":\"Enable_Sleep_Light\",\"Socket\":\"Light_Sleeping\",\"Gpio\":\"\",\"Switch\":\"\",\"Weekday\":0,\"Hour\":20,\"Minute\":33,\"OnOff\":1,\"IsTimer\":0,\"State\":1}},{\"Schedule\":{\"Name\":\"Disable_Living\",\"Socket\":\"Light_Living\",\"Gpio\":\"\",\"Switch\":\"\",\"Weekday\":2,\"Hour\":11,\"Minute\":15,\"OnOff\":0,\"IsTimer\":1,\"State\":1}}]} ";

            JsonDataToTimerConverter jsonDataToTimerConverter = new JsonDataToTimerConverter();
            IList<TimerDto> timerList = jsonDataToTimerConverter.GetList(response, new List<WirelessSocketDto>(), new List<WirelessSwitchDto>());

            Assert.IsNotNull(timerList);
            Assert.AreEqual(timerList.Count, 1);
            Assert.AreEqual(timerList[0].Name, "Disable_Living");
        }

        [TestMethod]
        public void JsonDataToWirelessSocketConverterTest()
        {
            string response = "{\"Data\":[{\"WirelessSocket\":{\"Name\":\"Light_Sleeping\",\"Area\":\"Sleeping_Room\",\"Code\":\"10101A\",\"State\":-1,\"LastTrigger\":{\"Hour\":-1,\"Minute\":-1,\"Day\":-1,\"Month\":-1,\"Year\":-1,\"UserName\":\"N.A.\"}}},{\"WirelessSocket\":{\"Name\":\"MediaServer_Sleeping\",\"Area\":\"Sleeping_Room\",\"Code\":\"10101B\",\"State\":-1,\"LastTrigger\":{\"Hour\":-1,\"Minute\":-1,\"Day\":-1,\"Month\":-1,\"Year\":-1,\"UserName\":\"N.A.\"}}},{\"WirelessSocket\":{\"Name\":\"MediaServer_Living\",\"Area\":\"Living_Room\",\"Code\":\"10101C\",\"State\":-1,\"LastTrigger\":{\"Hour\":-1,\"Minute\":-1,\"Day\":-1,\"Month\":-1,\"Year\":-1,\"UserName\":\"N.A.\"}}},{\"WirelessSocket\":{\"Name\":\"MediaServer_Kitchen\",\"Area\":\"Kitchen\",\"Code\":\"10101D\",\"State\":-1,\"LastTrigger\":{\"Hour\":-1,\"Minute\":-1,\"Day\":-1,\"Month\":-1,\"Year\":-1,\"UserName\":\"N.A.\"}}},{\"WirelessSocket\":{\"Name\":\"WorkStation_Jonas\",\"Area\":\"Living_Room\",\"Code\":\"11110A\",\"State\":-1,\"LastTrigger\":{\"Hour\":-1,\"Minute\":-1,\"Day\":-1,\"Month\":-1,\"Year\":-1,\"UserName\":\"N.A.\"}}},{\"WirelessSocket\":{\"Name\":\"TV\",\"Area\":\"Living_Room\",\"Code\":\"11110B\",\"State\":-1,\"LastTrigger\":{\"Hour\":-1,\"Minute\":-1,\"Day\":-1,\"Month\":-1,\"Year\":-1,\"UserName\":\"N.A.\"}}},{\"WirelessSocket\":{\"Name\":\"Light_Living\",\"Area\":\"Living_Room\",\"Code\":\"11110C\",\"State\":-1,\"LastTrigger\":{\"Hour\":-1,\"Minute\":-1,\"Day\":-1,\"Month\":-1,\"Year\":-1,\"UserName\":\"N.A.\"}}},{\"WirelessSocket\":{\"Name\":\"RaspberryPi_Living\",\"Area\":\"Living_Room\",\"Code\":\"11110D\",\"State\":-1,\"LastTrigger\":{\"Hour\":-1,\"Minute\":-1,\"Day\":-1,\"Month\":-1,\"Year\":-1,\"UserName\":\"N.A.\"}}}]} ";

            JsonDataToWirelessSocketConverter jsonDataToWirelessSocketConverter = new JsonDataToWirelessSocketConverter();
            IList<WirelessSocketDto> wirelessSocketList = jsonDataToWirelessSocketConverter.GetList(response);

            Assert.IsNotNull(wirelessSocketList);
            Assert.AreEqual(wirelessSocketList.Count, 8);
            Assert.AreEqual(wirelessSocketList[0].Name, "Light_Sleeping");
        }

        [TestMethod]
        public void JsonDataToWirelessSwitchConverterTest()
        {
            string response = "{\"Data\":[{\"WirelessSwitch\":{\"Name\":\"Light_Hall\",\"Area\":\"Hall\",\"RemoteId\":-1,\"KeyCode\":1,\"Action\":0,\"LastTrigger\":{\"Hour\":0,\"Minute\":0,\"Day\":1,\"Month\":1,\"Year\":1970,\"UserName\":\"NULL\"}}}]} ";

            JsonDataToWirelessSwitchConverter jsonDataToWirelessSwitchConverter = new JsonDataToWirelessSwitchConverter();
            IList<WirelessSwitchDto> wirelessSwitchList = jsonDataToWirelessSwitchConverter.GetList(response);

            Assert.IsNotNull(wirelessSwitchList);
            Assert.AreEqual(wirelessSwitchList.Count, 1);
            Assert.AreEqual(wirelessSwitchList[0].Name, "Light_Hall");
        }
    }
}
