using System;

namespace Common.Dto
{
    public class ChangeDto
    {
        private const string TAG = "ChangeDto";

        private int _id;

        private string _type;
        private DateTime _dateTime;
        private string _user;

        public ChangeDto(int id, string type, DateTime dateTime, string user)
        {
            _id = id;
            _type = type;
            _dateTime = dateTime;
            _user = user;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }

        public DateTime DateTime
        {
            get
            {
                return _dateTime;
            }
        }

        public string User
        {
            get
            {
                return _user;
            }
        }

        public override string ToString()
        {
            return string.Format("{{0}: {Type: {1}};{DateTime: {2}};{User: {3}}}", TAG, _type, _dateTime, _user);
        }
    }
}
