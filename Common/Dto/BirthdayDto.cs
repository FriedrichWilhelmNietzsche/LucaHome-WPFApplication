using System;

namespace Common.Dto
{
    public class BirthdayDto
    {
        private const string TAG = "BirthdayDto";

        enum BirthdayType { PREVIOUS, TODAY, UPCOMING};

        private string _name;
        private DateTime _birthday;

        public BirthdayDto(string name, DateTime birthday)
        {
            _name = name;
            _birthday = birthday;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public DateTime Birthday
        {
            get
            {
                return _birthday;
            }
        }

        public int Age
        {
            get
            {
                int age = -1;
                    
                DateTime today = DateTime.Now;

                if(today.Month > _birthday.Month)
                {
                    age = today.Year - _birthday.Year;
                }
                else if(today.Month == _birthday.Month)
                {
                    if(today.Day >= _birthday.Day)
                    {
                        age = today.Year - _birthday.Year;
                    }
                    else
                    {
                        age = today.Year - _birthday.Year - 1;
                    }
                }
                else
                {
                    age = today.Year - _birthday.Year - 1;
                }

                return age;
            }
        }

        public bool HasBirthday
        {
            get
            {
                DateTime today = DateTime.Now;
                if(today.Day == _birthday.Day && today.Month == _birthday.Month)
                {
                    return true;
                }
                return false;
            }
        }

        public int CurrentBirthdayType
        {
            get
            {
                if (HasBirthday)
                {
                    return (int)BirthdayType.TODAY;
                }
                else
                {
                    DateTime today = DateTime.Now;
                    if (today.Month > _birthday.Month || (today.Month == _birthday.Month && today.Day > _birthday.Day))
                    {
                        return (int)BirthdayType.PREVIOUS;
                    }
                    else
                    {
                        return (int)BirthdayType.UPCOMING;
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{{0}: {Name: {1}};{Birthday: {2}};{Age: {3}};{HasBirthday: {4}}}", TAG, _name, _birthday, Age, HasBirthday);
        }
    }
}
