using Common.Enums;
using System;
using System.Collections.Generic;

namespace Common.Dto
{
    public class MeterDataDto
    {
        private const string TAG = "MeterDataDto";

        private int _typeId;
        private string _type;

        private string _meterId;
        private string _area;

        private IList<int> _idList;
        private IList<double> _valueList;
        private IList<DateTime> _saveDateList;
        private IList<string> _imageNameList;

        public MeterDataDto(
            int typeId,
            string type,
            string meterId,
            string area,

            IList<int> idList,
            IList<double> valueList,
            IList<DateTime> saveDateList,
            IList<string> imageNameList)
        {
            _typeId = typeId;
            _type = type;

            _meterId = meterId;
            _area = area;

            _idList = idList;
            _valueList = valueList;
            _saveDateList = saveDateList;
            _imageNameList = imageNameList;
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
                    LucaServerAction.ADD_METER_DATA.Action, _id, _type, _typeId, _saveDate.Day, _saveDate.Month, _saveDate.Year, _saveDate.Hour, _saveDate.Minute, _meterId, _area, _value, _imageName);
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
