using System.Collections.Generic;

namespace Common.Enums
{
    public class LucaServerAction
    {
        public static readonly LucaServerAction NULL = new LucaServerAction(0, "");

        //BIRTHDAYS
        public static readonly LucaServerAction GET_BIRTHDAYS = new LucaServerAction(10, "getbirthdays");
        public static readonly LucaServerAction ADD_BIRTHDAY = new LucaServerAction(11, "addbirthday&id=");
        public static readonly LucaServerAction UPDATE_BIRTHDAY = new LucaServerAction(12, "updatebirthday&id=");
        public static readonly LucaServerAction DELETE_BIRTHDAY = new LucaServerAction(13, "deletebirthday&id=");

        //CAMERA
        public static readonly LucaServerAction START_MOTION = new LucaServerAction(20, "startmotion");
        public static readonly LucaServerAction STOP_MOTION = new LucaServerAction(21, "stopmotion");
        public static readonly LucaServerAction GET_MOTION_DATA = new LucaServerAction(22, "getmotiondata");
        public static readonly LucaServerAction SET_MOTION_CONTROL_TASK = new LucaServerAction(23, "setcontroltaskcamera&state=");

        //CHANGE
        public static readonly LucaServerAction GET_CHANGES = new LucaServerAction(30, "getchangesrest");

        //COINS
        public static readonly LucaServerAction GET_COINS_ALL = new LucaServerAction(130, "getcoinsall");
        public static readonly LucaServerAction GET_COINS_USER = new LucaServerAction(131, "getcoinsuser");
        public static readonly LucaServerAction ADD_COIN = new LucaServerAction(132, "addcoin&id=");
        public static readonly LucaServerAction UPDATE_COIN = new LucaServerAction(133, "updatecoin&id=");
        public static readonly LucaServerAction DELETE_COIN = new LucaServerAction(134, "deletecoin&id=");

        //INFORMATION
        public static readonly LucaServerAction GET_INFORMATIONS = new LucaServerAction(40, "getinformationsrest");

        //MAP_CONTENT
        public static readonly LucaServerAction GET_MAP_CONTENTS = new LucaServerAction(50, "getmapcontents");
        public static readonly LucaServerAction ADD_MAP_CONTENT = new LucaServerAction(51, "addmapcontent&id=");
        public static readonly LucaServerAction UPDATE_MAP_CONTENT = new LucaServerAction(52, "updatemapcontent&id=");
        public static readonly LucaServerAction DELETE_MAP_CONTENT = new LucaServerAction(53, "deletemapcontent&id=");

        //MENU
        public static readonly LucaServerAction GET_MENU = new LucaServerAction(60, "getmenu");
        public static readonly LucaServerAction UPDATE_MENU = new LucaServerAction(61, "updatemenu&weekday=");
        public static readonly LucaServerAction CLEAR_MENU = new LucaServerAction(62, "clearmenu&weekday=");
        public static readonly LucaServerAction GET_LISTED_MENU = new LucaServerAction(63, "getlistedmenu");

        //MOVIE
        public static readonly LucaServerAction GET_MOVIES = new LucaServerAction(70, "getmovies");
        public static readonly LucaServerAction START_MOVIE = new LucaServerAction(71, "startmovie&title=");
        public static readonly LucaServerAction ADD_MOVIE = new LucaServerAction(72, "addmovie&title=");
        public static readonly LucaServerAction UPDATE_MOVIE = new LucaServerAction(73, "updatemovie&title=");
        public static readonly LucaServerAction DELETE_MOVIE = new LucaServerAction(74, "deletemovie&title=");

        //SCHEDULES
        public static readonly LucaServerAction GET_SCHEDULES = new LucaServerAction(80, "getschedules");
        public static readonly LucaServerAction SET_SCHEDULE = new LucaServerAction(81, "setschedule&schedule=");
        public static readonly LucaServerAction ADD_SCHEDULE = new LucaServerAction(82, "addschedule&name=");
        public static readonly LucaServerAction UPDATE_SCHEDULE = new LucaServerAction(83, "updateschedule&name=");
        public static readonly LucaServerAction DELETE_SCHEDULE = new LucaServerAction(84, "deleteschedule&schedule=");

        //SHOPPING_LIST
        public static readonly LucaServerAction GET_SHOPPING_LIST = new LucaServerAction(90, "getshoppinglist");
        public static readonly LucaServerAction ADD_SHOPPING_ENTRY_F = new LucaServerAction(91, "addshoppingentry&id={0}&name={1}&group={2}&quantity={3}");
        public static readonly LucaServerAction UPDATE_SHOPPING_ENTRY_F = new LucaServerAction(92, "updateshoppingentry&id={0}&name={1}&group={2}&quantity={3}");
        public static readonly LucaServerAction DELETE_SHOPPING_ENTRY_F = new LucaServerAction(93, "deleteshoppingentry&id={0}");

        //SOCKETS
        public static readonly LucaServerAction GET_SOCKETS = new LucaServerAction(100, "getsockets");
        public static readonly LucaServerAction SET_SOCKET = new LucaServerAction(101, "setsocket&socket=");
        public static readonly LucaServerAction ADD_SOCKET = new LucaServerAction(102, "addsocket&name=");
        public static readonly LucaServerAction UPDATE_SOCKET = new LucaServerAction(103, "updatesocket&name=");
        public static readonly LucaServerAction DELETE_SOCKET = new LucaServerAction(104, "deletesocket&socket=");
        public static readonly LucaServerAction DEACTIVATE_ALL_SOCKETS = new LucaServerAction(105, "deactivateAllSockets");

        //TEMPERATURE
        public static readonly LucaServerAction GET_TEMPERATURES = new LucaServerAction(110, "getcurrenttemperaturerest");

        //USER
        public static readonly LucaServerAction VALIDATE_USER = new LucaServerAction(120, "validateuser");

        public static IEnumerable<LucaServerAction> Values
        {
            get
            {
                yield return NULL;

                yield return GET_BIRTHDAYS;
                yield return ADD_BIRTHDAY;
                yield return UPDATE_BIRTHDAY;
                yield return DELETE_BIRTHDAY;

                yield return START_MOTION;
                yield return STOP_MOTION;
                yield return GET_MOTION_DATA;
                yield return SET_MOTION_CONTROL_TASK;

                yield return GET_CHANGES;

                yield return GET_INFORMATIONS;

                yield return GET_MAP_CONTENTS;
                yield return ADD_MAP_CONTENT;
                yield return UPDATE_MAP_CONTENT;
                yield return DELETE_MAP_CONTENT;

                yield return GET_MENU;
                yield return UPDATE_MENU;
                yield return CLEAR_MENU;
                yield return GET_LISTED_MENU;

                yield return GET_MOVIES;
                yield return START_MOVIE;
                yield return ADD_MOVIE;
                yield return UPDATE_MOVIE;
                yield return DELETE_MOVIE;

                yield return GET_SCHEDULES;
                yield return SET_SCHEDULE;
                yield return ADD_SCHEDULE;
                yield return UPDATE_SCHEDULE;
                yield return DELETE_SCHEDULE;

                yield return GET_SHOPPING_LIST;
                yield return ADD_SHOPPING_ENTRY_F;
                yield return UPDATE_SHOPPING_ENTRY_F;
                yield return DELETE_SHOPPING_ENTRY_F;

                yield return GET_SOCKETS;
                yield return SET_SOCKET;
                yield return ADD_SOCKET;
                yield return UPDATE_SOCKET;
                yield return DELETE_SOCKET;
                yield return DEACTIVATE_ALL_SOCKETS;

                yield return GET_TEMPERATURES;

                yield return VALIDATE_USER;
            }
        }

        private readonly int _id;
        private readonly string _action;

        LucaServerAction(int id, string action)
        {
            _id = id;
            _action = action;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Action
        {
            get
            {
                return _action;
            }
        }

        public override string ToString()
        {
            return _action;
        }
    }
}
