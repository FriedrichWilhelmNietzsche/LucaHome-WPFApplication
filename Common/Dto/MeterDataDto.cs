using Common.Enums;
using System;

namespace Common.Dto
{
    public class MeterDataDto
    {
        private const string TAG = "MeterDataDto";

        private int _id;
        private string _type;
        private int _typeId;

        private DateTime _saveDate;
        private string _meterId;
        private string _area;
        private double _value;
        private string _imageName;

        public MeterDataDto(
            int id,
            string type,
            int typeId,

            DateTime saveDate,
            string meterId,
            string area,
            double value,
            string imageName)
        {
            _id = id;
            _type = type;
            _typeId = typeId;

            _saveDate = saveDate;
            _meterId = meterId;
            _area = area;
            _value = value;
            _imageName = imageName;
        }

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
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

        public DateTime SaveDate
        {
            get
            {
                return _saveDate;
            }
            set
            {
                _saveDate = value;
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

        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public string ImageName
        {
            get
            {
                return _imageName;
            }
            set
            {
                _imageName = value;
            }
        }

        public string CommandAdd
        {
            get
            {
                return string.Format("{0}{1}&type={2}&typeId={3}&day={4}&month={5}&year={6}&hour={7}&minute={8}&meterId={9}&area={10}&value={11}&imageName={12}",
                    LucaServerAction.ADD_METER_DATA.Action, _id, _type, _typeId, _saveDate.Day, _saveDate.Month, _saveDate.Year, _saveDate.Hour, _saveDate.Minute, _meterId, _area, _value, _imageName);
            }
        }

        public string CommandUpdate
        {
            get
            {
                return string.Format("{0}{1}&type={2}&typeId={3}&day={4}&month={5}&year={6}&hour={7}&minute={8}&meterId={9}&area={10}&value={11}&imageName={12}",
                    LucaServerAction.UPDATE_METER_DATA.Action, _id, _type, _typeId, _saveDate.Day, _saveDate.Month, _saveDate.Year, _saveDate.Hour, _saveDate.Minute, _meterId, _area, _value, _imageName);
            }
        }

        public string CommandDelete
        {
            get
            {
                return string.Format("{0}{1}", LucaServerAction.DELETE_METER_DATA.Action, _id);
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Id: {1} );(Type: {2} );(TypeId: {3} );(SaveDate: {4} );(MeterId: {5} );(Area: {6} );(Value: {7} );(ImageName: {8} ))",
                    TAG, _id, _type, _typeId, _saveDate, _meterId, _area, _value, _imageName);
        }
    }
}
