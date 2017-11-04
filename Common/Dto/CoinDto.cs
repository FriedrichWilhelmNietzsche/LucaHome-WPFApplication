using Common.Enums;
using System;

namespace Common.Dto
{
    public class CoinDto
    {
        private const string TAG = "CoinDto";

        public enum Trend { NULL, FALL, RISE }

        private int _id;
        private string _user;
        private string _type;
        private double _amount;

        private double _currentConversion;
        private Trend _trend;

        public CoinDto(int id, string user, string type, double amount, double currentConversion, Trend trend)
        {
            _id = id;
            _user = user;
            _type = type;
            _amount = amount;

            _currentConversion = currentConversion;
            _trend = trend;
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

        public Trend CurrentTrend
        {
            get
            {
                return _trend;
            }
            set
            {
                _trend = value;
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

        public Uri TrendIcon
        {
            get
            {
                switch (_trend)
                {
                    case Trend.RISE:
                        return new Uri("/Common;component/Assets/Icons/Others/trending_up.png", UriKind.Relative);
                    case Trend.FALL:
                        return new Uri("/Common;component/Assets/Icons/Others/trending_down.png", UriKind.Relative);
                    default:
                        return new Uri("/Common;component/Assets/Icons/Others/trending_flat.png", UriKind.Relative);

                }
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
            return string.Format("( {0}: (User: {1} );( Type: {2});(Amount: {3} );(CurrentConversion: {4} );(Value: {5} );(Trend: {6} ))", TAG, _user, _type, _amount, _currentConversion, Value, _trend);
        }
    }
}
