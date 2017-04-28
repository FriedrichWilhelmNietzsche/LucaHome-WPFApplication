namespace Common.Dto
{
    public class WirelessSocketDto
    {
        private const string TAG = "WirelessSocketDto";

        private string _name;
        private string _area;
        private string _code;
        private bool _isActivated;

        private string _shortName;

        public WirelessSocketDto(string name, string area, string code, bool isActivated)
        {
            _name = name;
            _area = area;
            _code = code;
            _isActivated = isActivated;

            if(_name.Length >= 3)
            {
                _shortName = _name.Substring(0, 3);
            }
            else
            {
                _shortName = _name;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Area
        {
            get
            {
                return _area;
            }
        }

        public string Code
        {
            get
            {
                return _code;
            }
        }

        public bool IsActivated
        {
            get
            {
                return _isActivated;
            }
        }

        public string ShortName
        {
            get
            {
                return _shortName;
            }
        }

        public override string ToString()
        {
            return string.Format("{{0}: {Name: {1}};{Area: {2}};{Code: {3}};{IsActivated: {4}}}", TAG, _name, _area, _code, (_isActivated ? "1" : "0"));
        }
    }
}
