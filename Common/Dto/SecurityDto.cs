using System.Collections.Generic;

namespace Common.Dto
{
    public class SecurityDto
    {
        private const string TAG = "SecurityDto";

        private bool _isCameraActive;
        private bool _isCameraControlActive;

        private string _cameraUrl;

        private IList<RegisteredEventDto> _registeredMotionEvents;

        public SecurityDto(bool isCameraActive, bool isCameraControlActive, string cameraUrl, IList<RegisteredEventDto> registeredMotionEvents)
        {
            _isCameraActive = isCameraActive;
            _isCameraControlActive = isCameraControlActive;
            _cameraUrl = cameraUrl;
            _registeredMotionEvents = registeredMotionEvents;
        }

        public bool IsCameraActive
        {
            get
            {
                return _isCameraActive;
            }
        }

        public bool IsCameraControlActive
        {
            get
            {
                return _isCameraControlActive;
            }
        }

        public string CameraUrl
        {
            get
            {
                return _cameraUrl;
            }
        }

        public IList<RegisteredEventDto> RegisteredMotionEvents
        {
            get
            {
                return _registeredMotionEvents;
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (IsCameraActive: {1} );(IsCameraControlActive: {2} );(CameraUrl: {3} );(RegisteredMotionEvents: {4} ))", TAG, IsCameraActive, IsCameraControlActive, CameraUrl, RegisteredMotionEvents);
        }

        public class RegisteredEventDto
        {
            private const string TAG = "RegisteredEventDto";

            private int _id;
            private string _name;

            public RegisteredEventDto(int id, string name)
            {
                _id = id;
                _name = name;
            }

            public int Id
            {
                get
                {
                    return _id;
                }
                set
                {
                    _id = value;
                }
            }

            public string Name
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                }
            }
        }
    }
}
