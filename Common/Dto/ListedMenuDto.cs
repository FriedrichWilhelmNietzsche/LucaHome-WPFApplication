namespace Common.Dto
{
    public class ListedMenuDto
    {
        private const string TAG = "ListedMenuDto";

        private int _id;
        private string _description;
        private int _rating;
        private bool _lastSuggestion;

        public ListedMenuDto(int id, string description, int rating, bool lastSuggestion)
        {
            _id = id;
            _description = description;
            _rating = rating;
            _lastSuggestion = lastSuggestion;
        }

        public int Id
        {
            get
            {
                return _id;
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

        public bool LastSuggestion
        {
            get
            {
                return _lastSuggestion;
            }
            set
            {
                _lastSuggestion = value;
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Id: {1} );(Description: {2} );(Rating: {3} );(LastSuggestion: {4} ))", TAG, _id, _description, _rating, _lastSuggestion);
        }
    }
}
