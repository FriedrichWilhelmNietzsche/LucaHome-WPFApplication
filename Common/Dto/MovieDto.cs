using Common.Enums;
using System;

namespace Common.Dto
{
    public class MovieDto
    {
        private const string TAG = "MovieDto";

        private int _id;

        private string _title;
        private string _genre;
        private string _description;
        private int _rating;
        private int _watched;

        public MovieDto(int id, string title, string genre, string description, int rating, int watched)
        {
            _id = id;

            _title = title;
            _genre = genre;
            _description = description;
            _rating = rating;
            _watched = watched;
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

        public string Genre
        {
            get
            {
                return _genre;
            }
            set
            {
                _genre = value;
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

        public string RatingString
        {
            get
            {
                return string.Format("{0}/5", _rating);
            }
        }

        public int Watched
        {
            get
            {
                return _watched;
            }
        }

        public Uri Icon
        {
            get
            {
                return new Uri("/Common;component/Assets/Icons/Others/movie_hd.png", UriKind.Relative);
            }
        }

        public string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&title={2}&genre={3}&description={4}&rating={5}&watched={6}", LucaServerAction.UPDATE_MOVIE.Action, _id, _title, _genre, _description, _rating, _watched);
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Id: {1} );(Title: {2} );(Genre: {3} );(Description: {4} );(Rating: {5} );(Watched: {6} ))", TAG, _id, _title, _genre, _description, _rating, _watched);
        }
    }
}
