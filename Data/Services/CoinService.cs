using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

namespace Data.Services
{
    public delegate void CoinConversionDownloadEventHandler(IList<KeyValuePair<string, double>> coinConversionList, bool success, string response);
    public delegate void CoinDownloadEventHandler(IList<CoinDto> coinList, bool success, string response);
    public delegate void CoinAddEventHandler(bool success, string response);
    public delegate void CoinUpdateEventHandler(bool success, string response);
    public delegate void CoinDeleteEventHandler(bool success, string response);

    public class CoinService
    {
        private const string TAG = "CoinService";
        private const int TIMEOUT = 30 * 60 * 1000;

        private readonly DownloadController _downloadController;

        private static CoinService _instance = null;
        private static readonly object _padlock = new object();

        private IList<CoinDto> _coinList = new List<CoinDto>();
        private IList<KeyValuePair<string, double>> _coinConversionList = new List<KeyValuePair<string, double>>();

        private Timer _downloadTimer;

        CoinService()
        {
            _downloadController = new DownloadController();
            _downloadController.OnDownloadFinished += _coinConversionDownloadFinished;
            _downloadController.OnDownloadFinished += _coinTrendDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event CoinConversionDownloadEventHandler OnCoinConversionDownloadFinished;
        private void publishOnCoinConversionDownloadFinished(IList<KeyValuePair<string, double>> coinConversionList, bool success, string response)
        {
            OnCoinConversionDownloadFinished?.Invoke(coinConversionList, success, response);
        }

        public event CoinDownloadEventHandler OnCoinDownloadFinished;
        private void publishOnCoinDownloadFinished(IList<CoinDto> coinList, bool success, string response)
        {
            OnCoinDownloadFinished?.Invoke(coinList, success, response);
        }

        public event CoinAddEventHandler OnCoinAddFinished;
        private void publishOnCoinAddFinished(bool success, string response)
        {
            OnCoinAddFinished?.Invoke(success, response);
        }

        public event CoinUpdateEventHandler OnCoinUpdateFinished;
        private void publishOnCoinUpdateFinished(bool success, string response)
        {
            OnCoinUpdateFinished?.Invoke(success, response);
        }

        public event CoinDeleteEventHandler OnCoinDeleteFinished;
        private void publishOnCoinDeleteFinished(bool success, string response)
        {
            OnCoinDeleteFinished?.Invoke(success, response);
        }

        public static CoinService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new CoinService();
                    }

                    return _instance;
                }
            }
        }

        public IList<CoinDto> CoinList
        {
            get
            {
                return _coinList;
            }
        }

        public CoinDto GetById(int id)
        {
            CoinDto foundCoin = _coinList
                        .Where(coin => coin.Id == id)
                        .Select(coin => coin)
                        .FirstOrDefault();

            return foundCoin;
        }

        public IList<CoinDto> FoundCoins(string searchKey)
        {
            if (searchKey == string.Empty)
            {
                return _coinList;
            }

            List<CoinDto> foundCoins = _coinList
                        .Where(coin =>
                            coin.Id.ToString().Contains(searchKey)
                            || coin.User.Contains(searchKey)
                            || coin.Type.Contains(searchKey)
                            || coin.Amount.ToString().Contains(searchKey))
                        .Select(coin => coin)
                        .ToList();

            return foundCoins;
        }

        public string AllCoinsValue
        {
            get
            {
                double value = 0;

                foreach (CoinDto coin in _coinList)
                {
                    value += coin.Value;
                }

                return string.Format("Sum: {0:0.00} €", value);
            }
        }
        public IList<string> TypeList
        {
            get
            {
                IList<string> typeList = new List<string>();

                typeList.Add("BTC");
                typeList.Add("DASH");
                typeList.Add("ETC");
                typeList.Add("ETH");
                typeList.Add("IOTA");
                typeList.Add("LTC");
                typeList.Add("XMR");
                typeList.Add("XRP");
                typeList.Add("ZEC");

                return typeList;
            }
        }

        public void LoadCoinConversionList()
        {
            loadCoinConversionAsync();
        }

        public void LoadCoinList()
        {
            loadCoinListAsync();
        }

        public void AddCoin(CoinDto newCoin)
        {
            Logger.Instance.Debug(TAG, string.Format("AddCoin: Adding new coin {0}", newCoin));
            addCoinAsync(newCoin);
        }

        public void UpdateCoin(CoinDto updateCoin)
        {
            Logger.Instance.Debug(TAG, string.Format("UpdateCoin: Updating coin {0}", updateCoin));
            updateCoinAsync(updateCoin);
        }

        public void DeleteCoin(CoinDto deleteCoin)
        {
            Logger.Instance.Debug(TAG, string.Format("DeleteCoin: Deleting coin {0}", deleteCoin));
            deleteCoinAsync(deleteCoin);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            loadCoinConversionAsync();
            loadCoinListAsync();
        }

        private async Task loadCoinConversionAsync()
        {
            string requestUrl = "https://min-api.cryptocompare.com/data/pricemulti?fsyms=BCH,BTC,DASH,ETC,ETH,IOTA,LTC,XMR,XRP,ZEC&tsyms=EUR";
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.CoinConversion);
        }

        private async Task loadCoinTrendAsync(CoinDto coin)
        {
            string requestUrl = string.Format("https://min-api.cryptocompare.com/data/histohour?fsym={0}&tsym={1}&limit={2}&aggregate=3&e=CCCAGG", coin.Type, "EUR", SettingsController.Instance.CoinHourTrend);
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.CoinTrend, coin);
        }

        private async Task loadCoinListAsync()
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnCoinDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_COINS_USER.Action);

            _downloadController.OnDownloadFinished += _coinDownloadFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.Coin);
        }

        private async Task addCoinAsync(CoinDto newCoin)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnCoinAddFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newCoin.CommandAdd);

            _downloadController.OnDownloadFinished += _coinAddFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.CoinAdd);
        }

        private async Task updateCoinAsync(CoinDto updateCoin)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnCoinUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateCoin.CommandUpdate);

            _downloadController.OnDownloadFinished += _coinUpdateFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.CoinUpdate);
        }

        private async Task deleteCoinAsync(CoinDto deleteCoin)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnCoinDeleteFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteCoin.CommandDelete);

            _downloadController.OnDownloadFinished += _coinDeleteFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.CoinDelete);
        }

        private void _coinConversionDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.CoinConversion)
            {
                return;
            }

            if (!response.Contains("BCH")
                || !response.Contains("BTC")
                || !response.Contains("DASH")
                || !response.Contains("ETC")
                || !response.Contains("ETH")
                || !response.Contains("IOTA")
                || !response.Contains("LTC")
                || !response.Contains("XMR")
                || !response.Contains("XRP")
                || !response.Contains("ZEC")
                || !response.Contains("EUR"))
            {
                Logger.Instance.Error(TAG, string.Format("Invalid response {0}", response));
                publishOnCoinConversionDownloadFinished(_coinConversionList, false, response);
                return;
            }

            IList<KeyValuePair<string, double>> coinConversionList = JsonDataToCoinConversionConverter.Instance.GetList(response);
            if (coinConversionList == null)
            {
                Logger.Instance.Error(TAG, "Converted coinConversionList is null!");
                publishOnCoinConversionDownloadFinished(_coinConversionList, false, response);
                return;
            }

            if (coinConversionList.Count == 0)
            {
                Logger.Instance.Error(TAG, "Converted coinConversionList is null!");
                publishOnCoinConversionDownloadFinished(_coinConversionList, false, response);
                return;
            }

            _coinConversionList = coinConversionList;

            foreach (CoinDto entry in _coinList)
            {
                entry.CurrentConversion = _coinConversionList
                    .Where(conversion => conversion.Key.Contains(entry.Type))
                    .Select(conversion => conversion)
                    .FirstOrDefault()
                    .Value;
            }

            publishOnCoinConversionDownloadFinished(_coinConversionList, true, response);
        }

        private void _coinTrendDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.CoinTrend)
            {
                return;
            }

            CoinDto currentCoin = (CoinDto)additional;
            currentCoin = JsonDataToCoinTrendConverter.Instance.UpdateTrend(currentCoin, response, "EUR");
            for (int index = 0; index < _coinList.Count; index++)
            {
                if (currentCoin.Id == _coinList.ElementAt(index).Id)
                {
                    _coinList.RemoveAt(index);
                    _coinList.Insert(index, currentCoin);
                    publishOnCoinDownloadFinished(_coinList, true, response);
                    break;
                }
            }
        }

        private void _coinDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.Coin)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _coinDownloadFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnCoinDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnCoinDownloadFinished(null, false, response);
                return;
            }

            IList<CoinDto> coinList = JsonDataToCoinConverter.Instance.GetList(response, _coinConversionList);
            if (coinList == null)
            {
                Logger.Instance.Error(TAG, "Converted coinList is null!");
                publishOnCoinDownloadFinished(null, false, response);
                return;
            }

            _coinList = coinList;

            publishOnCoinDownloadFinished(_coinList, true, response);

            foreach (CoinDto coin in _coinList)
            {
                loadCoinTrendAsync(coin);
            }
        }

        private void _coinAddFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.CoinAdd)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _coinAddFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnCoinAddFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Adding was not successful!");
                publishOnCoinAddFinished(false, response);
                return;
            }

            publishOnCoinAddFinished(true, response);
            loadCoinListAsync();
        }

        private void _coinUpdateFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.CoinUpdate)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _coinUpdateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnCoinUpdateFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Updating was not successful!");
                publishOnCoinUpdateFinished(false, response);
                return;
            }

            publishOnCoinUpdateFinished(true, response);
            loadCoinListAsync();
        }

        private void _coinDeleteFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.CoinDelete)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _coinDeleteFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnCoinDeleteFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Deleting was not successful!");
                publishOnCoinDeleteFinished(false, response);
                return;
            }

            publishOnCoinDeleteFinished(true, response);
            loadCoinListAsync();
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _coinConversionDownloadFinished;
            _downloadController.OnDownloadFinished -= _coinTrendDownloadFinished;
            _downloadController.OnDownloadFinished -= _coinDownloadFinished;
            _downloadController.OnDownloadFinished -= _coinAddFinished;
            _downloadController.OnDownloadFinished -= _coinUpdateFinished;
            _downloadController.OnDownloadFinished -= _coinDeleteFinished;
            _downloadController.Dispose();

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
