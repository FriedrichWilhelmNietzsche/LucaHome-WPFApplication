using System;
using Common.Common;
using Common.Enums;
using System.ComponentModel;

namespace Common.Dto
{
    public class WirelessSocketDto : INotifyPropertyChanged
    {
        private const string TAG = "WirelessSocketDto";

        protected int _typeId;

        protected string _name;
        protected string _area;
        protected string _code;
        protected bool _isActivated;

        protected DateTime _lastTriggerDate;
        protected string _lastTriggerUser;

        protected string _shortName;

        public WirelessSocketDto(int typeId, string name, string area, string code, bool isActivated, DateTime lastTriggerDate, string lastTriggerUser)
        {
            _typeId = typeId;

            _name = name;
            _area = area;
            _code = code;
            _isActivated = isActivated;

            _lastTriggerDate = lastTriggerDate;
            _lastTriggerUser = lastTriggerUser;

            _shortName = createShortName(_name);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int TypeId
        {
            get
            {
                return _typeId;
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

        public string Area
        {
            get
            {
                return _area;
            }
            set
            {
                _area = value;
            }
        }

        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        public bool IsActivated
        {
            get
            {
                return _isActivated;
            }
            set
            {
                _isActivated = value;

                OnPropertyChanged("IsActivated");
                OnPropertyChanged("ActivationString");
                OnPropertyChanged("Drawable");
            }
        }

        public DateTime LastTriggerDate
        {
            get
            {
                return _lastTriggerDate;
            }
            set
            {
                _lastTriggerDate = value;
            }
        }

        public string LastTriggerUser
        {
            get
            {
                return _lastTriggerUser;
            }
            set
            {
                _lastTriggerUser = value;
            }
        }

        public string ActivationString
        {
            get
            {
                return _isActivated ? "On" : "Off";
            }
        }

        public string ShortName
        {
            get
            {
                return _shortName;
            }
        }

        public virtual string CommandSet
        {
            get
            {
                return string.Format("{0}{1}{2}", LucaServerAction.SET_SOCKET.Action, _typeId, ((_isActivated) ? Constants.STATE_ON : Constants.STATE_OFF));
            }
        }

        public virtual string CommandAdd
        {
            get
            {
                return string.Format("{0}{1}&name={2}&area={3}&code={4}", LucaServerAction.ADD_SOCKET.Action, _typeId, _name, _area, _code);
            }
        }

        public virtual string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&name={2}&area={3}&code={4}&isactivated={5}", LucaServerAction.UPDATE_SOCKET.Action, _typeId, _name, _area, _code, (_isActivated ? "1" : "0"));
            }
        }

        public virtual string CommandDelete
        {
            get
            {
                return string.Format("{0}{1}", LucaServerAction.DELETE_SOCKET.Action, _typeId);
            }
        }

        public virtual Uri Drawable
        {
            get
            {
                if (_name.Contains("TV"))
                {
                    if (_isActivated)
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/tv_on.png", UriKind.Relative);
                    }
                    else
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/tv_off.png", UriKind.Relative);
                    }
                }
                else if (_name.Contains("Light"))
                {
                    if (_name.Contains("Sleeping"))
                    {
                        if (_isActivated)
                        {
                            return new Uri("/Common;component/Assets/Icons/Sockets/bed_light_on.png", UriKind.Relative);
                        }
                        else
                        {
                            return new Uri("/Common;component/Assets/Icons/Sockets/bed_light_off.png", UriKind.Relative);
                        }
                    }
                    else
                    {
                        if (_isActivated)
                        {
                            return new Uri("/Common;component/Assets/Icons/Sockets/light_on.png", UriKind.Relative);
                        }
                        else
                        {
                            return new Uri("/Common;component/Assets/Icons/Sockets/light_off.png", UriKind.Relative);
                        }
                    }
                }
                else if (_name.Contains("Sound"))
                {
                    if (_name.Contains("Sleeping"))
                    {
                        if (_isActivated)
                        {
                            return new Uri("/Common;component/Assets/Icons/Sockets/bed_sound_on.png", UriKind.Relative);
                        }
                        else
                        {
                            return new Uri("/Common;component/Assets/Icons/Sockets/bed_sound_off.png", UriKind.Relative);
                        }
                    }
                    else if (_name.Contains("Living"))
                    {
                        if (_isActivated)
                        {
                            return new Uri("/Common;component/Assets/Icons/Sockets/sound_on.png", UriKind.Relative);
                        }
                        else
                        {
                            return new Uri("/Common;component/Assets/Icons/Sockets/sound_off.png", UriKind.Relative);
                        }
                    }
                }
                else if (_name.Contains("PC") || _name.Contains("WorkStation"))
                {
                    if (_isActivated)
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/laptop_on.png", UriKind.Relative);
                    }
                    else
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/laptop_off.png", UriKind.Relative);
                    }
                }
                else if (_name.Contains("Printer"))
                {
                    if (_isActivated)
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/printer_on.png", UriKind.Relative);
                    }
                    else
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/printer_off.png", UriKind.Relative);
                    }
                }
                else if (_name.Contains("Storage"))
                {
                    if (_isActivated)
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/storage_on.png", UriKind.Relative);
                    }
                    else
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/storage_off.png", UriKind.Relative);
                    }
                }
                else if (_name.Contains("Heating"))
                {
                    if (_name.Contains("Bed"))
                    {
                        if (_isActivated)
                        {
                            return new Uri("/Common;component/Assets/Icons/Sockets/bed_heating_on.png", UriKind.Relative);
                        }
                        else
                        {
                            return new Uri("/Common;component/Assets/Icons/Sockets/bed_heating_off.png", UriKind.Relative);
                        }
                    }
                }
                else if (_name.Contains("Farm"))
                {
                    if (_isActivated)
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/watering_on.png", UriKind.Relative);
                    }
                    else
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/watering_off.png", UriKind.Relative);
                    }
                }
                else if (_name.Contains("MediaServer"))
                {
                    if (_isActivated)
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/mediaserver_on.png", UriKind.Relative);
                    }
                    else
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/mediaserver_off.png", UriKind.Relative);
                    }
                }
                else if (_name.Contains("GameConsole"))
                {
                    if (_isActivated)
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/gameconsole_on.png", UriKind.Relative);
                    }
                    else
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/gameconsole_off.png", UriKind.Relative);
                    }
                }
                else if (_name.Contains("RaspberryPi"))
                {
                    if (_isActivated)
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/raspberry_on.png", UriKind.Relative);
                    }
                    else
                    {
                        return new Uri("/Common;component/Assets/Icons/Sockets/raspberry_off.png", UriKind.Relative);
                    }
                }

                return new Uri("/Common;component/Assets/Icons/Sockets/socket.png", UriKind.Relative);
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Name: {1} );(Area: {2} );(Code: {3} );(IsActivated: {4} ))", TAG, _name, _area, _code, (_isActivated ? "1" : "0"));
        }

        protected string createShortName(string name)
        {
            string shortName;

            if (name.Contains("_"))
            {
                shortName = name.Substring(0, name.IndexOf("_"));
            }
            else
            {
                if (name.Length > 3)
                {
                    shortName = name.Substring(0, 3);
                }
                else
                {
                    shortName = name.Substring(0, name.Length);
                }
            }

            if (shortName.Length > 3)
            {
                shortName = shortName.Substring(0, 3);
            }

            return shortName;
        }
    }
}
