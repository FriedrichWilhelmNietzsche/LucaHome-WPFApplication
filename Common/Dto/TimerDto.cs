using Common.Enums;
using System;

namespace Common.Dto
{
    public class TimerDto : ScheduleDto
    {
        private const string TAG = "TimerDto";

        public TimerDto(int id, string name, string information, WirelessSocketDto socket, Weekday weekday, DateTime time, SocketAction action, bool isActive)
            : base(id, name, information, socket, weekday, time, action, isActive) { }

        public TimerDto(int id, string name, WirelessSocketDto socket, Weekday weekday, DateTime time, SocketAction action, bool isActive)
            : base(id, name, "", socket, weekday, time, action, isActive) { }

        public TimerDto(int id, string name, string information, SocketAction action, bool isActive)
            : base(id, name, information, null, Weekday.Sunday, new DateTime(), action, isActive) { }

        public override string CommandAdd
        {
            get
            {
                return string.Format("{0}{1}&socket={2}&gpio={3}&weekday={4}&hour={5}&minute={6}&onoff={7}&isTimer={8}&playSound={9}&playRaspberry={10}", LucaServerAction.ADD_SCHEDULE.Action, _name, _socket.Name, "", _weekday, _time.Hour, _time.Minute, (_action == SocketAction.Activate ? "1" : "0"), "1", "0", "1");
            }
        }

        public override string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&socket={2}&gpio={3}&weekday={4}&hour={5}&minute={6}&onoff={7}&isTimer={8}&playSound={9}&playRaspberry={10}&isactive={11}", LucaServerAction.UPDATE_SCHEDULE.Action, _name, _socket.Name, "", _weekday, _time.Hour, _time.Minute, (_action == SocketAction.Activate ? "1" : "0"), "1", "0", "1", (_isActive ? "1" : "0"));
            }
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
