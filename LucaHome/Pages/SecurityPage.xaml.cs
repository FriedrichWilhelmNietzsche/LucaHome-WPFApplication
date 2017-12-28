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
        private const string TAG = "SecurityPage";
        
        private readonly CameraController _cameraController;
        private readonly NavigationService _navigationService;

        private string _registeredEventSearchKey;
        private IList<RegisteredEventDto> _registeredEventList;

        private string _cameraButtonContent;
        private string _cameraControlButtonContent;
        private bool _cameraControlButtonEnabled;

        public SecurityPage(NavigationService navigationService)
        {
            _cameraController = new CameraController();
            _cameraControlButtonEnabled = false;

            _navigationService = navigationService;

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
                RegisteredEventList = SecurityService.Instance.FoundRegisteredEvents(_registeredEventSearchKey);
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
            SecurityService.Instance.OnSecurityDownloadFinished += _securityDownloadFinished;
            setUI(SecurityService.Instance.Security);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SecurityService.Instance.OnSecurityDownloadFinished -= _securityDownloadFinished;
            _cameraController.StopProcessing();
            _cameraController.ImageReady -= _cameraControllerImageReady;
        }

        private void ButtonViewEvent_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                string eventName = (string)senderButton.Tag;
                SecurityService.Instance.OpenFile(eventName);
            }
        }

        private void ButtonCamera_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (SecurityService.Instance.Security.IsCameraActive)
            {
                _cameraController.StopProcessing();
            }
            SecurityService.Instance.ToggleCameraState();
        }

        private void ButtonCameraControl_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            SecurityService.Instance.ToggleCameraControlState();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.Navigate(new BirthdayAddPage(_navigationService));
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            SecurityService.Instance.LoadSecurity();
        }

        private void _securityDownloadFinished(SecurityDto security, bool success, string response)
        {
            setUI(security);
        }

        private void _cameraControllerImageReady(object sender, ImageReadyEventArsgs imageReadyEventArsgs)
        {
            SecurityImage.Source = imageReadyEventArsgs.Image;
        }

        private void setUI(SecurityDto security)
        {
            RegisteredEventSearchKey = "";

            if (security == null)
            {
                Logger.Instance.Error(TAG, "security is null!");
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
