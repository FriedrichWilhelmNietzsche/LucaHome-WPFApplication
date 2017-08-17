using System.Windows.Media.Imaging;

namespace Common.Dto
{
    public class NovelDto
    {
        private const string TAG = "NovelDto";

        private string _author;
        private string[] _books;
        private BitmapImage _icon;

        public NovelDto(string author, string[] books, BitmapImage icon)
        {
            _author = author;
            _books = books;
            _icon = icon;
        }

        public string Author
        {
            get
            {
                return _author;
            }
        }

        public string[] Books
        {
            get
            {
                return _books;
            }
            set
            {
                _books = value;
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
            return string.Format("( {0}: (Author: {1} );(Books: {2} );(Icon: {3} ))", TAG, _author, _books, _icon);
        }
    }
}
