using System;
using Common.Enums;

namespace Common.Dto
{
    public class WirelessSwitchDto : WirelessSocketDto
    {
        private const string TAG = "WirelessSwitchDto";

        private int _remoteId;
        private char _keyCode;
        private bool _action;

        public WirelessSwitchDto(int id, string name, string area, int remoteId, char keyCode, bool isActivated, bool action, DateTime lastTriggerDate, string lastTriggerUser)
            : base(id, name, area, "", isActivated, lastTriggerDate, lastTriggerUser)
        {
            _remoteId = remoteId;
            _keyCode = keyCode;
            _action = action;
        }
        
        public int RemoteId
        {
            get
            {
                return _remoteId;
            }
            set
            {
                _remoteId = value;
            }
        }
        
        public char KeyCode
        {
            get
            {
                return _keyCode;
            }
            set
            {
                _keyCode = value;
            }
        }

        public bool Action
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

        public override string CommandSet
        {
            get
            {
                throw new NotSupportedException(string.Format("%s has no implementation for CommandSet! Please use CommandToggle!", TAG));
            }
        }

        public string CommandToggle
        {
            get
            {
                return string.Format("{0}{1}", LucaServerAction.TOGGLE_SWITCH.Action, _name);
            }
        }

        public override string CommandAdd
        {
            get
            {
                return string.Format("{0}{1}&area={2}&remoteid={3}&keycode={4}", LucaServerAction.ADD_SWITCH.Action, _name, _area, _remoteId, _keyCode);
            }
        }

        public override string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&area={2}&remoteid={3}&keycode={4}", LucaServerAction.UPDATE_SWITCH.Action, _name, _area, _remoteId, _keyCode);
            }
        }

        public override string CommandDelete
        {
            get
            {
                return string.Format("{0}{1}", LucaServerAction.DELETE_SWITCH.Action, _name);
            }
        }

        public override Uri Drawable
        {
            get
            {
                if (_action)
                {
                    return new Uri("/Common;component/Assets/Icons/Sockets/switch_on.png", UriKind.Relative);
                }
                else
                {
                    return new Uri("/Common;component/Assets/Icons/Sockets/switch_off.png", UriKind.Relative);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Name: {1} );(Area: {2} );(RemoteId: {3} );(KeyCode: {4} );(Action: {5} ))", TAG, _name, _area, _remoteId, _keyCode, (_action ? "1" : "0"));
        }
    }
}
