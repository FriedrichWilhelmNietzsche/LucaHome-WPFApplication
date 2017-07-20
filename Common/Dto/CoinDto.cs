using Common.Enums;
using System;

namespace Common.Dto
{
    public class CoinDto
    {
        private const string TAG = "CoinDto";

        private int _id;
        private string _user;
        private string _type;
        private double _amount;

        private double _currentConversion;

        public CoinDto(int id, string user, string type, double amount, double currentConversion)
        {
            _id = id;
            _user = user;
            _type = type;
            _amount = amount;

            _currentConversion = currentConversion;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public double Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
            }
        }

        public double CurrentConversion
        {
            get
            {
                return _currentConversion;
            }
            set
            {
                _currentConversion = value;
            }
        }

        public string CurrentConversionString
        {
            get
            {
                return string.Format("{0:0.00} €", CurrentConversion);
            }
        }

        public double Value
        {
            get
            {
                return _amount * _currentConversion;
            }
        }

        public string ValueString
        {
            get
            {
                return string.Format("{0:0.00} €", Value);
            }
        }

        public Uri Icon
        {
            get
            {
                return new Uri(string.Format("/Common;component/Assets/Icons/Coins/{0}.png", _type), UriKind.Relative);
            }
        }

        public string CommandAdd
        {
            get
            {
                return string.Format("{0}{1}&username={2}&type={3}&amount={4}", LucaServerAction.ADD_COIN.Action, _id, _user, _type, _amount);
            }
        }

        public string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&username={2}&type={3}&amount={4}", LucaServerAction.UPDATE_COIN.Action, _id, _user, _type, _amount);
            }
        }

        public string CommandDelete
        {
            get
            {
                return string.Format("{0}{1}", LucaServerAction.DELETE_COIN.Action, _id);
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (User: {1} );( Type: {2});(Amount: {3} );(CurrentConversion: {4} );(Value: {5} ))", TAG, _user, _type, _amount, _currentConversion, Value);
        }
    }
}
