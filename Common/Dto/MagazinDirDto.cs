using System.Windows.Media.Imaging;

namespace Common.Dto
{
    public class MagazinDirDto
    {
        private const string TAG = "MagazinDirDto";

        private string _dirName;
        private string[] _dirContent;
        private BitmapImage _icon;

        public MagazinDirDto(string dirName, string[] dirContent, BitmapImage icon)
        {
            _dirName = dirName;
            _dirContent = dirContent;
            _icon = icon;
        }

        public string DirName
        {
            get
            {
                return _dirName;
            }
        }

        public string[] DirContent
        {
            get
            {
                return _dirContent;
            }
            set
            {
                _dirContent = value;
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
            return string.Format("( {0}: (DirName: {1} );(DirContent: {2} );(Icon: {3} ))", TAG, _dirName, _dirContent, _icon);
        }
    }
}
