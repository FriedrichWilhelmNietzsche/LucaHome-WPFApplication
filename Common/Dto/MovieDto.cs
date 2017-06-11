using System;
using Common.Enums;

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

        private WirelessSocketDto[] _sockets;

        public MovieDto(int id, string title, string genre, string description, int rating, int watched, WirelessSocketDto[] sockets)
        {
            _id = id;

            _title = title;
            _genre = genre;
            _description = description;
            _rating = rating;
            _watched = watched;

            _sockets = sockets;
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

        public WirelessSocketDto[] Sockets
        {
            get
            {
                return _sockets;
            }
            set
            {
                _sockets = value;
            }
        }

        public string CommandAdd
        {
            get
            {
                return string.Format("{0}{1}&genre={2}&description={3}&rating={4}&watched={5}&sockets={6}", LucaServerAction.ADD_MOVIE.Action, _title, _genre, _description, _rating, _watched, getSocketsString());
            }
        }

        public string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&genre={2}&description={3}&rating={4}&watched={5}&sockets={6}", LucaServerAction.UPDATE_MOVIE.Action, _title, _genre, _description, _rating, _watched, getSocketsString());
            }
        }

        public string CommandDelete
        {
            get
            {
                return string.Format("{0}{1}", LucaServerAction.DELETE_MOVIE.Action, _title);
            }
        }

        public override string ToString()
        {
            return string.Format("{{0}: {Id: {1}};{Title: {2}};{Genre: {3}};{Description: {4}};{Rating: {5}};{Watched: {6}};{Sockets: {7}}}", TAG, _id, _title, _genre, _description, _rating, _watched, _sockets);
        }

        private object getSocketsString()
        {
            string socketsString = "";

            foreach(WirelessSocketDto socket in _sockets)
            {
                socketsString += socket.Name + "|";
            }
            socketsString = socketsString.Substring(0, socketsString.Length - 1);

            return socketsString;
        }
    }
}
