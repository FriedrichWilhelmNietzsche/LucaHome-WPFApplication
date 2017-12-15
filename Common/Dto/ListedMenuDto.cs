using Common.Enums;

namespace Common.Dto
{
    public class ListedMenuDto
    {
        private const string TAG = "ListedMenuDto";

        private int _id;
        private string _title;
        private string _description;
        private int _rating;
        private int _useCounter;

        public ListedMenuDto(int id, string title, string description, int rating, int useCounter)
        {
            _id = id;
            _title = title;
            _description = description;
            _rating = rating;
            _useCounter = useCounter;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public int Rating
        {
            get
            {
                return _rating;
            }
            set
            {
                _rating = value;
            }
        }

        public int UseCounter
        {
            get
            {
                return _useCounter;
            }
            set
            {
                _useCounter = value;
            }
        }

        public string CommandAdd
        {
            get
            {
                return string.Format("{0}{1}&title={2}&description={3}&rating={4}", LucaServerAction.ADD_LISTEDMENU.Action, _id, _title, _description, _rating);
            }
        }

        public string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&title={2}&description={3}&rating={4}", LucaServerAction.UPDATE_LISTEDMENU.Action, _id, _title, _description, _rating);
            }
        }

        public string CommandDelete
        {
            get
            {
                return string.Format("{0}{1}", LucaServerAction.DELETE_LISTEDMENU.Action, _id);
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Id: {1} );(Title: {2} );(Description: {3} );(Rating: {4} );(UseCounter: {5} ))", TAG, _id, _title, _description, _rating, _useCounter);
        }
    }
}
