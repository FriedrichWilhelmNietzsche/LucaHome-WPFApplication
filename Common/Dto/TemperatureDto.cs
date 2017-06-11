using System;

namespace Common.Dto
{
    public class TemperatureDto
    {
        private const string TAG = "TemperatureDto";

        public enum TemperatureType { DUMMY, RASPBERRY, CITY };

        private double _temperature;
        private string _area;
        private DateTime _lastUpdate;
        private string _sensorPath;
        private TemperatureType _temperatureType;
        private string _graphPath;

        public TemperatureDto(double temperature, string area, DateTime lastUpdate, string sensorPath, TemperatureType temperatureType, string graphPath)
        {
            _temperature = temperature;
            _area = area;
            _lastUpdate = lastUpdate;
            _sensorPath = sensorPath;
            _temperatureType = temperatureType;
            _graphPath = graphPath;
        }

        public double Temperature
        {
            get
            {
                return _temperature;
            }
        }

        public string TemperatureString
        {
            get
            {
                return string.Format("{0} °C", _temperature);
            }
        }

        public string Area
        {
            get
            {
                return _area;
            }
        }

        public DateTime LastUpdate
        {
            get
            {
                return _lastUpdate;
            }
            set
            {
                _lastUpdate = value;
            }
        }

        public string SensorPath
        {
            get
            {
                return _sensorPath;
            }
        }

        public TemperatureType GetTemperatureType
        {
            get
            {
                return _temperatureType;
            }
        }

        public string GraphPath
        {
            get
            {
                return _graphPath;
            }
        }

        public override string ToString()
        {
            return string.Format("{{0}: {Temperature: {1}};{Area: {2}};{LastUpdate: {3}};{SensorPath: {4}};{TemperatureType: {5}};{GraphPath: {6}}}", TAG, TemperatureString, _area, _lastUpdate, _sensorPath, _temperatureType, _graphPath);
        }
    }
}
