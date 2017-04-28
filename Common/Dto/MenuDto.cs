using System;

namespace Common.Dto
{
    public class MenuDto
    {
        private const string TAG = "MenuDto";

        private string _title;
        private string _description;
        private DateTime _date;

        private bool _needsUpdate;

        public MenuDto(string title, string description, DateTime date)
        {
            _title = title;
            _description = description;
            _date = date;
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

        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{{0}: {Title: {1}};{Description: {2}};{Date: {3}}}", TAG, _title, _description, _date);
        }
    }
}
