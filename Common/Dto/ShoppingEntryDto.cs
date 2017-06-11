using Common.Enums;

namespace Common.Dto
{
    public class ShoppingEntryDto
    {
        private const string TAG = "ShoppingEntryDto";

        private int _id;
        private string _name;
        private ShoppingEntryGroup _group;
        private int _quantity;

        public ShoppingEntryDto(int id, string name, ShoppingEntryGroup group, int quantity)
        {
            _id = id;
            _name = name;
            _group = group;
            _quantity = quantity;
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
        }

        public ShoppingEntryGroup Group
        {
            get
            {
                return _group;
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
            }
        }

        public void IncreaseQuantity()
        {
            _quantity++;
        }

        public void DecreaseQuantity()
        {
            _quantity--;
            if (_quantity < 0)
            {
                _quantity = 0;
            }
        }

        public string CommandAdd
        {
            get
            {
                return string.Format(LucaServerAction.ADD_SHOPPING_ENTRY_F.Action, _id, _name, _group.Description, _quantity);
            }
        }

        public string CommandUpdate
        {
            get
            {
                return string.Format(LucaServerAction.UPDATE_SHOPPING_ENTRY_F.Action, _id, _name, _group.Description, _quantity);
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
            return string.Format("{{0}: {Id: {1}};{Name: {2}};{Group: {3}};{Quantity: {4}}}", TAG, _id, _name, _group, _quantity);
        }
    }
}
