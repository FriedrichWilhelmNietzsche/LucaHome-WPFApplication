namespace Common.Dto
{
    public class UserDto
    {
        private const string TAG = "UserDto";

        private string _user;
        private string _passphrase;

        public UserDto(string user, string passphrase)
        {
            _user = user;
            _passphrase = passphrase;
        }

        public string User
        {
            get
            {
                return _user;
            }
        }

        public string Passphrase
        {
            get
            {
                return _passphrase;
            }
        }

        public override string ToString()
        {
            return string.Format("{{0}: {User: {1}};{Passphrase: {2}}}", TAG, _user, "-/-");
        }
    }
}
