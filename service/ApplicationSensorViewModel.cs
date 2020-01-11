using GMap.NET;
using GMap.NET.WindowsPresentation;
using GpsMapRoutes.models;
using GpsMapRoutes.service.commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Runtime.CompilerServices;

namespace GpsMapRoutes
{
    public class ApplicationSensorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ApplicationViewModel OwnerContext { get; }
        public SensorWindow OwnerWindow { get; }

        public SensorModel PrewSensor => SelectedPositionSensorInList + 1 >= OwnerContext.CurrentSensorsList.Count ? null : OwnerContext.CurrentSensorsList[SelectedPositionSensorInList + 1];
        public int SelectedSensorId => OwnerContext.SelectedSensor?.Id ?? 0;
        public int SelectedPositionSensorInList => SelectedSensorId <= 0 ? -1 : OwnerContext.CurrentSensorsList.FindIndex(x => x.Id == SelectedSensorId);
        public double SelectedPositionDistance => SelectedSensorId <= 0 ? -1 : CurrentSensor.Distance;
        public SensorModel NextSensor => SelectedPositionSensorInList > 0 ? OwnerContext.CurrentSensorsList[SelectedPositionSensorInList - 1] : null;

        public SensorModel CurrentSensor => OwnerContext.CurrentSensorsList[OwnerContext.CurrentSensorsList.FindIndex(x => x.Id == SelectedSensorId)];

        public GMapRoute SelectedSegmentMapRoute
        {
            get
            {
                if (CurrentSensor is null)
                {
                    return new GMapRoute(new PointLatLng[] { ApplicationViewModel.DefaultPoint }) { ZIndex = -1 };
                }

                if (PrewSensor is null && NextSensor is null)
                {
                    return new GMapRoute(new PointLatLng[] { new PointLatLng(CurrentSensor.Lat, CurrentSensor.Lng) }) { ZIndex = -1 };
                }

                List<PointLatLng> points = new List<PointLatLng>();

                if (!(PrewSensor is null))
                {
                    points.Add(new PointLatLng(PrewSensor.Lat, PrewSensor.Lng));
                }
                points.Add(new PointLatLng(CurrentSensor.Lat, CurrentSensor.Lng));
                if (!(NextSensor is null))
                {
                    points.Add(new PointLatLng(NextSensor.Lat, NextSensor.Lng));
                }

                return new GMapRoute(points) { ZIndex = -1 };
            }
        }

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

        public string CurrentMapPosition
        {
            get => OwnerWindow.MainMap.Position.ToString();
        }

        protected double adjustment;
        public double Adjustment
        {
            get => adjustment;
            set
            {
                adjustment = value;
                OnPropertyChanged(nameof(Adjustment));

                OwnerWindow.MainMap.Markers.Clear();
                GMapRoute route = SelectedSegmentMapRoute;
                if (route.Points.Count == 1)
                {
                    OwnerWindow.MainMap.Markers.Add(new GMapMarker(route.Points[0]));
                    OwnerWindow.MainMap.ZoomAndCenterMarkers(null);
                    OwnerWindow.MainMap.Position = route.Points[0];
                }
                else
                {
                    OwnerWindow.MainMap.Markers.Add(route);
                    OwnerWindow.MainMap.ZoomAndCenterMarkers(null);
                    OwnerWindow.MainMap.Position = new PointLatLng(CurrentSensor.Lat, CurrentSensor.Lng);
                }

                
            }
        }

        #region commands
        private RelayCommand resetAdjustmentCommand;
        public RelayCommand ResetAdjustmentCommand
        {
            get
            {
                return resetAdjustmentCommand ??
                  (resetAdjustmentCommand = new RelayCommand(obj =>
                  {
                      Adjustment = SelectedPositionDistance;
                  },
                (obj) => (true)));
            }
        }
        #endregion

        public ApplicationSensorViewModel(ApplicationViewModel setOwner, SensorWindow sensorWindow)
        {
            OwnerContext = setOwner;
            OwnerWindow = sensorWindow;
            OwnerWindow.MainMap.OnPositionChanged += MainMap_OnPositionChanged;
            OwnerWindow.MainMap.Loaded += MainMap_Loaded;

        }

        private void MainMap_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Adjustment = SelectedPositionDistance;
        }

        private void MainMap_OnPositionChanged(PointLatLng point)
        {
            OnPropertyChanged(nameof(CurrentMapPosition));
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
