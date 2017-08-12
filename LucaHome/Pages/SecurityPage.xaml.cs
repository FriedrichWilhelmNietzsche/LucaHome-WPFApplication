using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.CustomEventArgs;
using Data.Services;
using LucaHome.Controller;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using static Common.Dto.SecurityDto;

/*
 * Really helpful link
 * https://www.dotnetperls.com/listview-wpf
 * https://stackoverflow.com/questions/2796470/wpf-create-a-dialog-prompt
 * https://stackoverflow.com/questions/1545258/changing-the-start-up-location-of-a-wpf-window
 */

namespace LucaHome.Pages
{
    public partial class SecurityPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "BirthdayPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly SecurityService _securityService;

        private readonly CameraController _cameraController;

        private string _registeredEventSearchKey;
        private IList<RegisteredEventDto> _registeredEventList;

        private string _cameraButtonContent;
        private string _cameraControlButtonContent;
        private bool _cameraControlButtonEnabled;

        public SecurityPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _securityService = SecurityService.Instance;

            _cameraController = new CameraController();

            _cameraControlButtonEnabled = false;

            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string RegisteredEventSearchKey
        {
            get
            {
                return _registeredEventSearchKey;
            }
            set
            {
                _registeredEventSearchKey = value;
                OnPropertyChanged("RegisteredEventSearchKey");

                if (_registeredEventSearchKey != string.Empty)
                {
                    RegisteredEventList = _securityService.FoundRegisteredEvents(_registeredEventSearchKey);
                }
                else
                {
                    RegisteredEventList = _securityService.Security.RegisteredMotionEvents;
                }
            }
        }

        public IList<RegisteredEventDto> RegisteredEventList
        {
            get
            {
                return _registeredEventList;
            }
            set
            {
                _registeredEventList = value;
                OnPropertyChanged("RegisteredEventList");
            }
        }

        public string CameraButtonContent
        {
            get
            {
                return _cameraButtonContent;
            }
            set
            {
                _cameraButtonContent = value;
                OnPropertyChanged("CameraButtonContent");
            }
        }

        public string CameraControlButtonContent
        {
            get
            {
                return _cameraControlButtonContent;
            }
            set
            {
                _cameraControlButtonContent = value;
                OnPropertyChanged("CameraControlButtonContent");
            }
        }

        public bool CameraControlButtonEnabled
        {
            get
            {
                return _cameraControlButtonEnabled;
            }
            set
            {
                _cameraControlButtonEnabled = value;
                OnPropertyChanged("CameraControlButtonEnabled");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _securityService.OnSecurityDownloadFinished += _securityDownloadFinished;
            setUI(_securityService.Security);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _securityService.OnSecurityDownloadFinished -= _securityDownloadFinished;

            _cameraController.StopProcessing();
            _cameraController.ImageReady -= _cameraControllerImageReady;
        }

        private void ButtonViewEvent_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonViewEvent_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                string eventName = (string)senderButton.Tag;
                _securityService.OpenFile(eventName);
            }
        }

        private void ButtonCamera_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonCamera_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (_securityService.Security.IsCameraActive)
            {
                _cameraController.StopProcessing();
            }
            _securityService.ToggleCameraState();
        }

        private void ButtonCameraControl_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonCameraControl_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _securityService.ToggleCameraControlState();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonAdd_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.Navigate(new BirthdayAddPage(_navigationService));
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _securityService.LoadSecurity();
        }

        private void _securityDownloadFinished(SecurityDto security, bool success, string response)
        {
            _logger.Debug(string.Format("_securityDownloadFinished with model {0} was successful: {1}", security, success));
            setUI(security);
        }

        private void _cameraControllerImageReady(object sender, ImageReadyEventArsgs imageReadyEventArsgs)
        {
            _logger.Debug(string.Format("_cameraControllerImageReady with sender {0} has imageReadyEventArsgs: {1}", sender, imageReadyEventArsgs));
            SecurityImage.Source = imageReadyEventArsgs.Image;
        }

        private void setUI(SecurityDto security)
        {
            RegisteredEventSearchKey = "";

            if (security == null)
            {
                _logger.Error("security is null!");
                return;
            }

            RegisteredEventList = security.RegisteredMotionEvents;

            CameraButtonContent = security.IsCameraActive ? "Disable camera" : "Enable camera";
            CameraControlButtonContent = security.IsCameraControlActive ? "Disable control" : "Enable control";
            CameraControlButtonEnabled = security.IsCameraActive;

            if (security.IsCameraActive && security.CameraUrl != string.Empty)
            {
                _cameraController.Initialize("http://" + security.CameraUrl);
                _cameraController.ImageReady += _cameraControllerImageReady;
                _cameraController.StartProcessing();
            }
            else
            {
                _cameraController.StopProcessing();
                _cameraController.ImageReady -= _cameraControllerImageReady;
            }
        }
    }
}
