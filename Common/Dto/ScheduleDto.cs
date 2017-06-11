using Common.Enums;
using System;

namespace Common.Dto
{
    public class ScheduleDto
    {
        private const string TAG = "ScheduleDto";

        public enum Weekday { Null = -1, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }
        public enum SocketAction {  Null, Activate, Deactivate }

        protected int _id;

        protected string _name;
        protected string _information;
        protected WirelessSocketDto _socket;
        protected Weekday _weekday;
        protected DateTime _time;
        protected SocketAction _action;
        protected bool _isActive;

        public ScheduleDto(int id, string name, string information, WirelessSocketDto socket, Weekday weekday, DateTime time, SocketAction action, bool isActive)
        {
            _id = id;
            _name = name;
            _information = information;
            _socket = socket;
            _weekday = weekday;
            _time = time;
            _action = action;
            _isActive = isActive;
        }

        public ScheduleDto(int id, string name, WirelessSocketDto socket, Weekday weekday, DateTime time, SocketAction action, bool isActive)
            : this(id, name, "", socket, weekday, time, action, isActive) { }

        public ScheduleDto(int id, string name, string information, SocketAction action, bool isActive)
            : this(id, name, information, null, Weekday.Sunday, new DateTime(), action, isActive) { }

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
        }

        public string Information
        {
            get
            {
                return _information;
            }
            set
            {
                _information = value;
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

        public Weekday WeekDay
        {
            get
            {
                return _weekday;
            }
            set
            {
                _weekday = value;
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

        public SocketAction Action
        {
            get
            {
                return _action;
            }
            set
            {
                _action = value;
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

        public virtual string CommandAdd()
        {
            return string.Format("{0}{1}&socket={2}&gpio={3}&weekday={4}&hour={5}&minute={6}&onoff={7}&isTimer={8}&playSound={9}&playRaspberry={10}", LucaServerAction.ADD_SCHEDULE.Action, _name, _socket.Name, "", _weekday, _time.Hour, _time.Minute, (_action == SocketAction.Activate ? "1" : "0"), "0", "0", "1");
        }

        public virtual string CommandUpdate()
        {
            return string.Format("{0}{1}&socket={2}&gpio={3}&weekday={4}&hour={5}&minute={6}&onoff={7}&isTimer={8}&playSound={9}&playRaspberry={10}&isactive={11}", LucaServerAction.UPDATE_SCHEDULE.Action, _name, _socket.Name, "", _weekday, _time.Hour, _time.Minute, (_action == SocketAction.Activate ? "1" : "0"), "0", "0", "1", (_isActive ? "1" : "0"));
        }

        public virtual string CommandDelete()
        {
            return string.Format("{0}{1}", LucaServerAction.DELETE_SCHEDULE.Action, _name);
        }

        public override string ToString()
        {
            return "{" + TAG
                + ": {Id: " + _id.ToString()
                + "};{Name: " + _name
                + "};{WirelessSocket: " + (_socket == null ? "" : _socket.ToString())
                + "};{Weekday: " + _weekday.ToString()
                + "};{Time: " + _time.ToString()
                + "};{Action: " + _action.ToString()
                + "};{IsActive: " + (_isActive ? "1" : "0")
                + "}}";
        }
    }
}
