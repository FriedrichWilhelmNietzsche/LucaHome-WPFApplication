using System;

namespace Common.Dto
{
    public class MovieDto
    {
        private const string TAG = "MovieDto";

        private string _title;
        private string _genre;
        private string _description;
        private int _rating;
        private int _watched;

        public MovieDto(string title, string genre, string description, int rating, int watched)
        {
            _title = title;
            _genre = genre;
            _description = description;
            _rating = rating;
            _watched = watched;
        }

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public string Genre
        {
            get
            {
                return _genre;
            }
        }

        public string Description
        {
            get
            {
                return _description;
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

        public int Watched
        {
            get
            {
                return _watched;
            }
        }

        public override string ToString()
        {
            return string.Format("{{0}: {Title: {1}};{Genre: {2}};{Description: {3}};{Rating: {4}};{Watched: {5}}}", TAG, _title, _genre, _description, _rating, _watched);
        }
    }
}
