using Windows.UI.Xaml.Media.Imaging;

namespace Common.Dto
{
    public class ShoppingEntryDto
    {
        private const string TAG = "ShoppingEntryDto";

        private int _id;
        private string _name;
        private int _quantity;
        private BitmapImage _icon;

        public ShoppingEntryDto(int id, string name, int quantity, BitmapImage icon)
        {
            _id = id;
            _name = name;
            _quantity = quantity;
            _icon = icon;
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

        public BitmapImage Icon
        {
            get
            {
                return _icon;
            }
        }

        public override string ToString()
        {
            return string.Format("{{0}: {Id: {1}};{Name: {2}};{Quantity: {3}};{Icon: {4}}}", TAG, _id, _name, _quantity, _icon);
        }
    }
}
