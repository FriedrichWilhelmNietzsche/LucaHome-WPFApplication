using Common.Enums;
using System;

namespace Common.Dto
{
    public class ScheduleDto
    {
        private const string TAG = "ScheduleDto";

        public enum WirelessAction { Null = -1, Deactivate, Activate }

        protected int _id;

        protected string _name;

        protected WirelessSocketDto _socket;
        protected WirelessSwitchDto _wirelessSwitch;

        protected DateTime _time;

        protected WirelessAction _wirelessAction;

        protected bool _isActive;

        public ScheduleDto(int id, string name, WirelessSocketDto socket, WirelessSwitchDto wirelessSwitch, DateTime time, WirelessAction wirelessAction, bool isActive)
        {
            _id = id;

            _name = name;

            _socket = socket;
            _wirelessSwitch = wirelessSwitch;

            _time = time;

            _wirelessAction = wirelessAction;

            _isActive = isActive;
        }

        public ScheduleDto(int id, string name, WirelessAction wirelessAction, bool isActive)
            : this(id, name, null, null, DateTime.Now, wirelessAction, isActive) { }

        public int Id
        {
            get
            {
                return _id;
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

        public WirelessSocketDto Socket
        {
            get
            {
                return _socket;
            }
            set
            {
                _socket = value;
            }
        }

        public string SocketName
        {
            get
            {
                return _socket.Name;
            }
        }

        public WirelessSwitchDto WirelessSwitch
        {
            get
            {
                return _wirelessSwitch;
            }
            set
            {
                _wirelessSwitch = value;
            }
        }

        public string SwitchName
        {
            get
            {
                return _wirelessSwitch.Name;
            }
        }

        public DateTime Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
            }
        }

        public WirelessAction Action
        {
            get
            {
                return _wirelessAction;
            }
            set
            {
                _wirelessAction = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }

        public string ActiveString
        {
            get
            {
                return _isActive ? "Active" : "Inactive";
            }
        }

        public Uri Icon
        {
            get
            {
                return new Uri("/Common;component/Assets/Icons/Others/scheduler_hd.png", UriKind.Relative);
            }
        }

        public virtual string CommandSet
        {
            get
            {
                return string.Format("{0}{1}&isactive={2}", LucaServerAction.SET_SCHEDULE.Action, _id, (_isActive ? "1" : "0"));
            }
        }

        public virtual string CommandAdd
        {
            get
            {
                return string.Format("{0}{1}&name={2}&socket={3}&gpio={4}&switch={5}&weekday={6}&hour={7}&minute={8}&$action={9}&isTimer={10}", LucaServerAction.ADD_SCHEDULE.Action, _id, _name, _socket.Name, "", _wirelessSwitch.Name, _time.DayOfWeek, _time.Hour, _time.Minute, (_wirelessAction == WirelessAction.Activate ? "1" : "0"), "0");
            }
        }

        public virtual string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&name={2}&socket={3}&gpio={4}&switch={5}&weekday={6}&hour={7}&minute={8}&$action={9}&isTimer={10}&isactive={11}", LucaServerAction.UPDATE_SCHEDULE.Action, _id, _name, _socket.Name, "", _wirelessSwitch.Name, _time.DayOfWeek, _time.Hour, _time.Minute, (_wirelessAction == WirelessAction.Activate ? "1" : "0"), "0", (_isActive ? "1" : "0"));
            }
        }

        public virtual string CommandDelete
        {
            get
            {
                return string.Format("{0}{1}", LucaServerAction.DELETE_SCHEDULE.Action, _id);
            }
        }

        public override string ToString()
        {
            return "{" + TAG
                + ": {Id: " + _id.ToString()
                + "};{Name: " + _name
                + "};{WirelessSocket: " + (_socket == null ? "" : _socket.ToString())
                + "};{WirelessSwitch: " + (_wirelessSwitch == null ? "" : _wirelessSwitch.ToString())
                + "};{Time: " + _time.ToString()
                + "};{WirelessAction: " + _wirelessAction.ToString()
                + "};{IsActive: " + (_isActive ? "1" : "0")
                + "}}";
        }
    }
}
