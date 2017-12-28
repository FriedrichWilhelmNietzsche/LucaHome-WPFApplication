using Common.Enums;
using System;

namespace Common.Dto
{
    public class ShoppingEntryDto
    {
        private const string TAG = "ShoppingEntryDto";

        private int _id;
        private string _name;
        private ShoppingEntryGroup _group;
        private int _quantity;
        private string _unit;

        public ShoppingEntryDto(int id, string name, ShoppingEntryGroup group, int quantity, string unit)
        {
            _id = id;
            _name = name;
            _group = group;
            _quantity = quantity;
            _unit = unit;
        }

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
            set
            {
                _name = value;
            }
        }

        public ShoppingEntryGroup Group
        {
            get
            {
                return _group;
            }
            set
            {
                _group = value;
            }
        }

        public int Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
                if (_quantity < 1)
                {
                    _quantity = 1;
                }
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

        public Uri Icon
        {
            get
            {
                return _group.Icon;
            }
        }

        public string CommandAdd
        {
            get
            {
                return string.Format(LucaServerAction.ADD_SHOPPING_ENTRY_F.Action, _id, _name, _group.Description, _quantity, _unit);
            }
        }

        public string CommandUpdate
        {
            get
            {
                return string.Format(LucaServerAction.UPDATE_SHOPPING_ENTRY_F.Action, _id, _name, _group.Description, _quantity, _unit);
            }
        }

        public string CommandDelete
        {
            get
            {
                return string.Format(LucaServerAction.DELETE_SHOPPING_ENTRY_F.Action, _id);
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Id: {1} );(Name: {2} );(Group: {3} );(Quantity: {4} );(Unit: {5} ))", TAG, _id, _name, _group, _quantity, _unit);
        }
    }
}
