using System;
using System.Collections.Generic;

namespace Common.Dto
{
    public class MoneyDateDto
    {
        private const string TAG = "MoneyDateDto";

        private DateTime _saveDate;
        private string _user;
        private IList<MoneyMeterDataDto> _moneyMeterDataList;

        public MoneyDateDto(
            DateTime saveDate,
            string user,
            IList<MoneyMeterDataDto> moneyMeterDataList)
        {
            _saveDate = saveDate;
            _user = user;
            _moneyMeterDataList = moneyMeterDataList;
        }

        public DateTime SaveDate
        {
            get
            {
                return _saveDate;
            }
            set
            {
                _saveDate = value;
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

        public IList<MoneyMeterDataDto> MoneyMeterDataList
        {
            get
            {
                return _moneyMeterDataList;
            }
            set
            {
                _moneyMeterDataList = value;
            }
        }

        public double Amount
        {
            get
            {
                double amount = 0;
                foreach (MoneyMeterDataDto moneyMeterData in _moneyMeterDataList)
                {
                    amount += moneyMeterData.Amount;
                }
                return amount;
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (SaveDate: {1} );(User: {2} ))", TAG, _saveDate, _user);
        }
    }
}
