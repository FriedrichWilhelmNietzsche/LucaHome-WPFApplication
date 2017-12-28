using System.Collections.Generic;

namespace Common.Dto
{
    public class MoneyMeterDto
    {
        private const string TAG = "MoneyMeterDto";

        private int _typeId;
        private string _bank;
        private string _plan;
        private string _user;
        private IList<MoneyMeterDataDto> _moneyMeterDataList;

        public MoneyMeterDto(
            int typeId,
            string bank,
            string plan,
            string user,
            IList<MoneyMeterDataDto> moneyMeterDataList)
        {
            _typeId = typeId;
            _bank = bank;
            _plan = plan;
            _user = user;
            _moneyMeterDataList = moneyMeterDataList;
        }

        public int TypeId
        {
            get
            {
                return _typeId;
            }
            set
            {
                _typeId = value;
            }
        }

        public string Bank
        {
            get
            {
                return _bank;
            }
            set
            {
                _bank = value;
            }
        }

        public string Plan
        {
            get
            {
                return _plan;
            }
            set
            {
                _plan = value;
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

        public override string ToString()
        {
            return string.Format("( {0}: (TypeId: {1} );(Bank: {2} );(Plan: {3} );(User: {4} ))", TAG, _typeId, _bank, _plan, _user);
        }
    }
}
