using Common.Enums;
using System;

namespace Common.Dto
{
    public class MenuDto
    {
        private const string TAG = "MenuDto";

        private int _id;
        private string _title;
        private string _description;
        private DateTime _date;

        private bool _needsUpdate;

        public MenuDto(int id, string title, string description, DateTime date)
        {
            _id = id;
            _title = title;
            _description = description;
            _date = date;
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

        public string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&day={2}&month={3}&year={4}&title={5}&description={6}", LucaServerAction.UPDATE_MENU.Action, _date.DayOfWeek, _date.Day, _date.Month, _date.Year, _title, _description);
            }
        }

        public string CommandClear
        {
            get
            {
                return string.Format("{0}{1}", LucaServerAction.UPDATE_MENU.Action, _date.DayOfWeek);
            }
        }

        public override string ToString()
        {
            return string.Format("{{0}: {Id: {1}};{Title: {2}};{Description: {3}};{Date: {4}}}", TAG, _id, _title, _description, _date);
        }
    }
}
