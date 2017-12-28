using Common.Enums;
using System;

namespace Common.Dto
{
    public class MoneyMeterDataDto
    {
        private const string TAG = "MoneyMeterDataDto";

        private int _id;

        private int _typeId;

        private string _bank;
        private string _plan;
        private double _amount;
        private string _unit;

        private DateTime _saveDate;
        private string _user;

        public MoneyMeterDataDto(
            int id,

            int typeId,

            string bank,
            string plan,
            double amount,
            string unit,

            DateTime saveDate,
            string user)
        {
            _id = id;

            _typeId = typeId;

            _bank = bank;
            _plan = plan;
            _amount = amount;
            _unit = unit;

            _saveDate = saveDate;
            _user = user;
        }

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
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

        public string Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
            }
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

        public string CommandAdd
        {
            get
            {
                return string.Format("{0}{1}&typeid={2}&bank={3}&plan={4}&amount={5}&unit={6}&day={7}&month={8}&year={9}&username={10}",
                    LucaServerAction.ADD_MONEY_METER_DATA.Action, _id, _typeId, _bank, _plan, _amount, _unit, _saveDate.Day, _saveDate.Month, _saveDate.Year, _user);
            }
        }

        public string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&typeid={2}&bank={3}&plan={4}&amount={5}&unit={6}&day={7}&month={8}&year={9}&username={10}",
                    LucaServerAction.UPDATE_MONEY_METER_DATA.Action, _id, _typeId, _bank, _plan, _amount, _unit, _saveDate.Day, _saveDate.Month, _saveDate.Year, _user);
            }
        }

        public string CommandDelete
        {
            get
            {
                return string.Format("{0}{1}", LucaServerAction.DELETE_MONEY_METER_DATA.Action, _id);
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Id: {1} );(TypeId: {2} );(Bank: {3} );(Plan: {4} );(Amount: {5} );(Unit: {6} );(SaveDate: {7} );(User: {8} ))",
                    TAG, _id, _typeId, _bank, _plan, _amount, _unit, _saveDate, _user);
        }
    }
}
