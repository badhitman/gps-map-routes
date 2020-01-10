using GpsMapRoutes.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GpsMapRoutes
{
    public class ApplicationSensorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ApplicationViewModel Owner { get; }

        public SensorModel PrewSensor => SelectedPositionSensorInList + 1 >= Owner.CurrentSensorsList.Count ? null : Owner.CurrentSensorsList[SelectedPositionSensorInList + 1];
        public int SelectedSensorId => Owner.SelectedSensor?.Id ?? 0;
        public int SelectedPositionSensorInList => SelectedSensorId <= 0 ? -1 : Owner.CurrentSensorsList.FindIndex(x => x.Id == SelectedSensorId);
        public double SelectedPositionDistance => SelectedSensorId <= 0 ? 0 : CurrentSensor.Distance;
        public SensorModel NextSensor => SelectedPositionSensorInList > 0 ? Owner.CurrentSensorsList[SelectedPositionSensorInList - 1] : null;

        public SensorModel CurrentSensor => Owner.CurrentSensorsList[Owner.CurrentSensorsList.FindIndex(x => x.Id == SelectedSensorId)];


        public double MinAdjustment => PrewSensor?.Distance ?? SelectedPositionDistance;
        public double MaxAdjustment => NextSensor?.Distance ?? SelectedPositionDistance;

        #region структурные заголовки
        public string PrevSensorTitle => PrewSensor is null || (PrewSensor is null && NextSensor is null) ? "Текущий" : "Предыдущий";
        public string MidleSensorTitle => PrewSensor is null || NextSensor is null ? "" : "Текущий";
        public string NextSensorTitle => PrewSensor is null && NextSensor is null ? "" : NextSensor is null ? "Текущий" : "Следующий";
        #endregion

        #region установленные пользователем distance
        public string PrevSensorDistance => PrewSensor is null || (PrewSensor is null && NextSensor is null) ? SelectedPositionDistance.ToString() : PrewSensor.Distance.ToString() + " (-" + (SelectedPositionDistance - PrewSensor.Distance).ToString() + ")";
        public string MidleSensorDistance => PrewSensor is null || NextSensor is null ? "" : SelectedPositionDistance.ToString();
        public string NextSensorDistance => PrewSensor is null && NextSensor is null ? "" : NextSensor is null ? SelectedPositionDistance.ToString() : "(+" + (NextSensor.Distance - SelectedPositionDistance) + ") " + NextSensor.Distance.ToString();
        #endregion

        public string CalculationInfo
        {
            get
            {
                string cal_info = string.Empty;

                if (PrewSensor is null && NextSensor is null)
                {
                    cal_info += "Предыдущих или следующих точек не обнаружено";
                }
                else if (!(PrewSensor is null) && !(NextSensor is null))
                {
                    var sCoord = new GeoCoordinate(PrewSensor.Lat, PrewSensor.Lng);
                    var eCoord = new GeoCoordinate(CurrentSensor.Lat, CurrentSensor.Lng);
                    cal_info += "от предыдущей ≈ " + Math.Round(sCoord.GetDistanceTo(eCoord), 2) + " метров.";
                    cal_info += "\nрасчётная дистанция ≈ " + Math.Round(PrewSensor.Distance + sCoord.GetDistanceTo(eCoord), 2) + " м.\n";

                    sCoord = new GeoCoordinate(NextSensor.Lat, NextSensor.Lng);
                    cal_info += "\nдо следующей ≈ " + Math.Round(sCoord.GetDistanceTo(eCoord), 2) + " метров.";
                    cal_info += "\nрасчётная дистанция ≈ " + Math.Round(NextSensor.Distance - sCoord.GetDistanceTo(eCoord), 2) + " м.\n";
                    cal_info += "\n\n";
                }
                else if (PrewSensor is null)
                {
                    var sCoord = new GeoCoordinate(NextSensor.Lat, NextSensor.Lng);
                    var eCoord = new GeoCoordinate(CurrentSensor.Lat, CurrentSensor.Lng);
                    cal_info += "\nдо следующей ≈ " + sCoord.GetDistanceTo(eCoord) + " метров.";
                }
                else if (NextSensor is null)
                {
                    var sCoord = new GeoCoordinate(PrewSensor.Lat, PrewSensor.Lng);
                    var eCoord = new GeoCoordinate(CurrentSensor.Lat, CurrentSensor.Lng);
                    cal_info += "от предыдущей ≈ " + sCoord.GetDistanceTo(eCoord) + " метров.";
                }

                return cal_info.Trim();
            }
        }

        protected double adjustment;
        public double Adjustment
        {
            get => adjustment;
            set
            {
                adjustment = value;
                OnPropertyChanged(nameof(Adjustment));
            }
        }

        public ApplicationSensorViewModel(ApplicationViewModel setOwner)
        {
            Owner = setOwner;
            Adjustment = SelectedPositionDistance;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
