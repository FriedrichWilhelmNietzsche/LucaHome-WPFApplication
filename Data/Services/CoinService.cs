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
    public delegate void CoinDownloadEventHandler(IList<CoinDto> birthdayList, bool success, string response);
    public delegate void CoinAddEventHandler(bool success, string response);
    public delegate void CoinUpdateEventHandler(bool success, string response);
    public delegate void CoinDeleteEventHandler(bool success, string response);

    public class CoinService
    {
        private const string TAG = "CoinService";
        private readonly Logger _logger;

        private const int TIMEOUT = 30 * 60 * 1000;

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;
        private readonly JsonDataToCoinConverter _jsonDataToCoinConverter;

        private static CoinService _instance = null;
        private static readonly object _padlock = new object();

        private IList<CoinDto> _coinList = new List<CoinDto>();

        private Timer _downloadTimer;

        CoinService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
            _jsonDataToCoinConverter = new JsonDataToCoinConverter();

            _downloadController.OnDownloadFinished += _coinDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event CoinDownloadEventHandler OnCoinDownloadFinished;
        public event CoinAddEventHandler OnCoinAddFinished;
        public event CoinUpdateEventHandler OnCoinUpdateFinished;
        public event CoinDeleteEventHandler OnCoinDeleteFinished;

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

        public void LoadCoinList()
        {
            _logger.Debug("LoadCoinList");
            loadCoinListAsync();
        }

        public void AddCoin(CoinDto newCoin)
        {
            _logger.Debug(string.Format("AddCoin: Adding new coin {0}", newCoin));
            addCoinAsync(newCoin);
        }

        public void UpdateCoin(CoinDto updateCoin)
        {
            _logger.Debug(string.Format("UpdateCoin: Updating coin {0}", updateCoin));
            updateCoinAsync(updateCoin);
        }

        public void DeleteCoin(CoinDto deleteCoin)
        {
            _logger.Debug(string.Format("DeleteCoin: Deleting coin {0}", deleteCoin));
            deleteCoinAsync(deleteCoin);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadCoinListAsync();
        }

        private async Task loadCoinListAsync()
        {
            _logger.Debug("loadCoinListAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnCoinDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_COINS_USER.Action;
            _logger.Debug(string.Format("RequestUrl {0}", requestUrl));

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.Coin);
        }

        private async Task addCoinAsync(CoinDto newCoin)
        {
            _logger.Debug(string.Format("addCoinAsync: Adding new coin {0}", newCoin));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnCoinAddFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newCoin.CommandAdd);

            _downloadController.OnDownloadFinished += _coinAddFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.CoinAdd);
        }

        private async Task updateCoinAsync(CoinDto updateCoin)
        {
            _logger.Debug(string.Format("updateCoinAsync: Updating coin {0}", updateCoin));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnCoinUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateCoin.CommandUpdate);

            _downloadController.OnDownloadFinished += _coinUpdateFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.CoinUpdate);
        }

        private async Task deleteCoinAsync(CoinDto deleteCoin)
        {
            _logger.Debug(string.Format("deleteCoinAsync: Deleting coin {0}", deleteCoin));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnCoinDeleteFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteCoin.CommandDelete);

            _downloadController.OnDownloadFinished += _coinDeleteFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.CoinDelete);
        }

        private void _coinDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_coinDownloadFinished");

            if (downloadType != DownloadType.Coin)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnCoinDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnCoinDownloadFinished(null, false, response);
                return;
            }

            IList<CoinDto> coinList = _jsonDataToCoinConverter.GetList(response);
            if (coinList == null)
            {
                _logger.Error("Converted coinList is null!");

                OnCoinDownloadFinished(null, false, response);
                return;
            }

            _coinList = coinList;

            OnCoinDownloadFinished(_coinList, true, response);
        }

        private void _coinAddFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_coinAddFinished");

            if (downloadType != DownloadType.CoinAdd)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _coinAddFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnCoinAddFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Adding was not successful!");

                OnCoinAddFinished(false, response);
                return;
            }

            OnCoinAddFinished(true, response);

            loadCoinListAsync();
        }

        private void _coinUpdateFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_coinUpdateFinished");

            if (downloadType != DownloadType.CoinUpdate)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _coinUpdateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnCoinUpdateFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Updating was not successful!");

                OnCoinUpdateFinished(false, response);
                return;
            }

            OnCoinUpdateFinished(true, response);

            loadCoinListAsync();
        }

        private void _coinDeleteFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_coinDeleteFinished");

            if (downloadType != DownloadType.CoinDelete)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _coinDeleteFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnCoinDeleteFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Deleting was not successful!");

                OnCoinDeleteFinished(false, response);
                return;
            }

            OnCoinDeleteFinished(true, response);

            loadCoinListAsync();
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _coinDownloadFinished;
            _downloadController.OnDownloadFinished -= _coinAddFinished;
            _downloadController.OnDownloadFinished -= _coinUpdateFinished;
            _downloadController.OnDownloadFinished -= _coinDeleteFinished;

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();

            _downloadController.Dispose();
        }
    }
}
