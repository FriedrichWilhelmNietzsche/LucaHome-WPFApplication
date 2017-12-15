using Common.Enums;
using System;

namespace Common.Dto
{
    public class TimerDto : ScheduleDto
    {
        private const string TAG = "TimerDto";

        public TimerDto(int id, string name, WirelessSocketDto socket, WirelessSwitchDto wirelessSwitch, DateTime time, WirelessAction action, bool isActive)
            : base(id, name, socket, wirelessSwitch, time, action, isActive) { }

        public TimerDto(int id, string name, WirelessAction action, bool isActive)
            : base(id, name, null, null, new DateTime(), action, isActive) { }

        public override string CommandAdd
        {
            get
            {
                return string.Format("{0}{1}&socket={2}&gpio={3}&switch={4}&weekday={5}&hour={6}&minute={7}&onoff={8}&isTimer={9}", LucaServerAction.ADD_SCHEDULE.Action, _name, _socket.Name, "", _wirelessSwitch.Name, _time.DayOfWeek, _time.Hour, _time.Minute, (_wirelessAction == WirelessAction.Activate ? "1" : "0"), "1");
            }
        }

        public override string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&socket={2}&gpio={3}&switch={4}&weekday={5}&hour={6}&minute={7}&onoff={8}&isTimer={9}&isactive={10}", LucaServerAction.UPDATE_SCHEDULE.Action, _name, _socket.Name, "", _wirelessSwitch.Name, _time.DayOfWeek, _time.Hour, _time.Minute, (_wirelessAction == WirelessAction.Activate ? "1" : "0"), "1", (_isActive ? "1" : "0"));
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
                + "};{Action: " + _wirelessAction.ToString()
                + "};{IsActive: " + (_isActive ? "1" : "0")
                + "}}";
        }
    }
}
