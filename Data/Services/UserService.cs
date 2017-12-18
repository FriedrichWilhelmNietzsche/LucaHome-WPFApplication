using Common.Common;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Threading.Tasks;

namespace Data.Services
{
    public delegate void UserCheckedEventHandler(string response, bool success);

    public class UserService
    {
        private const string TAG = "UserService";

        private readonly DownloadController _downloadController;

        private static UserService _instance = null;
        private static readonly object _padlock = new object();

        private UserDto _tempUser = null;

        UserService()
        {
            _downloadController = new DownloadController();
            _downloadController.OnDownloadFinished += _validateUserFinished;
        }

        public event UserCheckedEventHandler OnUserCheckedFinished;
        private void publishOnUserCheckedFinished(string response, bool success)
        {
            OnUserCheckedFinished?.Invoke(response, success);
        }

        public static UserService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new UserService();
                    }

                    return _instance;
                }
            }
        }

        public UserDto User
        {
            get
            {
                return SettingsController.Instance.User;
            }
            set
            {
                if (value == null)
                {
                    Logger.Instance.Error(TAG, "Cannot add null value for User!");
                    return;
                }

                SettingsController.Instance.User = value;
            }
        }

        public bool UserSaved()
        {
            UserDto user = SettingsController.Instance.User;

            if (!user.Name.Equals("NA") && !user.Passphrase.Equals("NA"))
            {
                return true;
            }

            return false;
        }

        public void ValidateUser()
        {
            validateUserAsync(User);
        }

        public void ValidateUser(UserDto user)
        {
            validateUserAsync(user);
        }

        private async Task validateUserAsync(UserDto user)
        {
            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.VALIDATE_USER.Action);

            _tempUser = user;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.User);
        }

        private void _validateUserFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.User)
            {
                return;
            }

            if (response.Contains("Error"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnUserCheckedFinished(response, false);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Validation was not successful!");
                publishOnUserCheckedFinished(response, false);
                return;
            }

            SettingsController.Instance.User = _tempUser;
            _tempUser = null;
            publishOnUserCheckedFinished(response, true);
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _validateUserFinished;
            _downloadController.Dispose();
        }
    }
}
