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
                return string.Format("{0}{1}&name={2}&socket={3}&gpio={4}&switch={5}&weekday={6}&hour={7}&minute={8}&action={9}&isTimer={10}", LucaServerAction.ADD_SCHEDULE.Action, _id, _name, _socket.Name, "", _wirelessSwitch.Name, _time.DayOfWeek, _time.Hour, _time.Minute, (_wirelessAction == WirelessAction.Activate ? "1" : "0"), "1");
            }
        }

        public override string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&name={2}&socket={3}&gpio={4}&switch={5}&weekday={6}&hour={7}&minute={8}&action={9}&isTimer={10}&isactive={11}", LucaServerAction.UPDATE_SCHEDULE.Action, _id, _name, _socket.Name, "", _wirelessSwitch.Name, _time.DayOfWeek, _time.Hour, _time.Minute, (_wirelessAction == WirelessAction.Activate ? "1" : "0"), "1", (_isActive ? "1" : "0"));
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
