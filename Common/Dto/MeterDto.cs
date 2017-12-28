using System.Collections.Generic;

namespace Common.Dto
{
    public class MeterDto
    {
        private const string TAG = "MeterDto";

        private int _typeId;
        private string _type;
        private string _meterId;
        private string _area;
        private IList<MeterDataDto> _meterDataList;

        public MeterDto(
            int typeId,
            string type,
            string meterId,
            string area,
            IList<MeterDataDto> meterDataList)
        {
            _typeId = typeId;
            _type = type;
            _meterId = meterId;
            _area = area;
            _meterDataList = meterDataList;
        }

        public int TypeId
        {
            get
            {
                return _typeId;
            }
            set
            {
                _typeId = value;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public string MeterId
        {
            get
            {
                return _meterId;
            }
            set
            {
                _meterId = value;
            }
        }

        public string Area
        {
            get
            {
                return _area;
            }
            set
            {
                _area = value;
            }
        }

        public IList<MeterDataDto> MeterDataList
        {
            get
            {
                return _meterDataList;
            }
            set
            {
                _meterDataList = value;
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Type: {1} );(TypeId: {2} );(MeterId: {3} );(Area: {4} ))", TAG, _type, _typeId, _meterId, _area);
        }
    }
}
