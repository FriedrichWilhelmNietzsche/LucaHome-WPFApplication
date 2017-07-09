using Common.Enums;
using System;

namespace Common.Dto
{
    public class BirthdayDto
    {
        private const string TAG = "BirthdayDto";

        enum BirthdayType { PREVIOUS, TODAY, UPCOMING};

        private int _id;

        private string _name;
        private DateTime _birthday;

        public BirthdayDto(int id, string name, DateTime birthday)
        {
            _id = id;
            _name = name;
            _birthday = birthday;
        }
        
        public int Id
        {
            get
            {
                return _id;
            }
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
            set
            {
                _birthday = value;
            }
        }

        public Uri Icon
        {
            get
            {
                return new Uri("/Common;component/Assets/Icons/Others/birthday_hd.png", UriKind.Relative);
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

        public string CommandAdd
        {
            get
            {
                return string.Format("{0}{1}&name={2}&day={3}&month={4}&year={5}", LucaServerAction.ADD_BIRTHDAY.Action, _id, _name, _birthday.Day, _birthday.Month, _birthday.Year);
            }
        }

        public string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&name={2}&day={3}&month={4}&year={5}", LucaServerAction.UPDATE_BIRTHDAY.Action, _id, _name, _birthday.Day, _birthday.Month, _birthday.Year);
            }
        }

        public string CommandDelete
        {
            get
            {
                return string.Format("{0}{1}", LucaServerAction.DELETE_BIRTHDAY.Action, _id);
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Name: {1} );(Birthday: {2} );(Age: {3} );(HasBirthday: {4} ))", TAG, _name, _birthday, Age, HasBirthday);
        }
    }
}
