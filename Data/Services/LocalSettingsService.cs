using Common.Dto;
using Common.Tools;
using Data.Common;
using Windows.Storage;

/* Reference Help
 * https://docs.microsoft.com/de-de/windows/uwp/app-settings/store-and-retrieve-app-data
 */

namespace Data.Services
{
    public class LocalSettingsService
    {
        private const string TAG = "LocalSettingsService";
        private Logger _logger;

        private ApplicationDataContainer _localSettings;

        public LocalSettingsService()
        {
            _logger = new Logger(TAG);
            _localSettings = ApplicationData.Current.LocalSettings;
        }

        public UserDto User
        {
            get
            {
                object userName = _localSettings.Values[LocalSettings.USER_NAME];
                object passPhrase = _localSettings.Values[LocalSettings.PASS_PHRASE];

                if (userName != null && passPhrase != null)
                {
                    if (userName is string && passPhrase is string)
                    {
                        UserDto user = new UserDto((string)userName, (string)passPhrase);
                        _logger.Debug(string.Format("Returning user {0} from localSettings!", user));
                        return user;
                    }
                    _logger.Warning("UserName and/or passPhrase are found, but are not strings!");
                }

                _logger.Warning("No user found!");
                return null;
            }
            set
            {
                UserDto newUser = value;
                _logger.Debug(string.Format("Received new user {0} to save to localSettings!", newUser));

                _localSettings.Values[LocalSettings.USER_NAME] = newUser.Name;
                _localSettings.Values[LocalSettings.PASS_PHRASE] = newUser.Passphrase;
            }
        }

    }
}
