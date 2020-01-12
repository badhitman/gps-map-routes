////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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

        public string Information
        {
            get => CurrentSensor?.Information;
            set
            {
                if (CurrentSensor is null)
                    return;

                CurrentSensor.Information = value;
                OnPropertyChanged(nameof(Information));
            }
        }

        public string CalculationInfo
        {
            get
            {
                string cal_info = OwnerWindow.MainMap.Position.ToString();

                cal_info += "\n";

                if (full_calck_distance == 0)
                {
                    cal_info += "\nБез отклонений";
                }
                else
                {
                    cal_info += "\nОтклонение (" + (adjustment < SelectedPositionDistance ? "назад" : "вперёд") + "): " +
                        "\nAuto: " + Math.Round(full_calck_distance / 100 * adjustment_percent_factor, 2) + "/" + full_calck_distance + " м." +
                        "\nMan: " + Math.Round(full_manual_distance / 100 * adjustment_percent_factor, 2) + "/" + full_manual_distance + " м.";
                }

                cal_info += "\n";

                if (PrewSensor is null && NextSensor is null)
                {
                    cal_info += "\nПредыдущих или следующих точек не обнаружено";
                }
                else if (!(PrewSensor is null) && !(NextSensor is null))
                {
                    var sCoord = new GeoCoordinate(PrewSensor.Lat, PrewSensor.Lng);
                    var eCoord = new GeoCoordinate(CurrentSensor.Lat, CurrentSensor.Lng);
                    cal_info += "\nот предыдущей ≈ " + Math.Round(sCoord.GetDistanceTo(eCoord), 2) + " метров.";
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

        double full_calck_distance, full_manual_distance, adjustment_percent_factor;
        protected double adjustment; double λ;
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
                }
                else
                {
                    OwnerWindow.MainMap.Markers.Add(route);
                }
                OwnerWindow.MainMap.ZoomAndCenterMarkers(null);

                // общая расчётная длинна между точками
                full_calck_distance = 0;
                λ = 0;

                full_manual_distance = 0;
                adjustment_percent_factor = 0;
                if (adjustment < SelectedPositionDistance)
                {
                    var sCoord = new GeoCoordinate(PrewSensor.Lat, PrewSensor.Lng);
                    var eCoord = new GeoCoordinate(CurrentSensor.Lat, CurrentSensor.Lng);

                    full_calck_distance = Math.Round(sCoord.GetDistanceTo(eCoord), 2);
                    full_manual_distance = CurrentSensor.Distance - PrewSensor.Distance;
                    adjustment_percent_factor = (SelectedPositionDistance - adjustment) / (full_manual_distance / 100);
                    if (adjustment_percent_factor >= 100)
                    {
                        OwnerWindow.MainMap.Position = new PointLatLng(PrewSensor.Lat, PrewSensor.Lng);
                    }
                    else
                    {
                        λ = (full_manual_distance / 100 * (100 - adjustment_percent_factor)) / (full_manual_distance / 100 * adjustment_percent_factor); // am/bm
                        OwnerWindow.MainMap.Position = new PointLatLng((PrewSensor.Lat + λ * CurrentSensor.Lat) / (1 + λ), (PrewSensor.Lng + λ * CurrentSensor.Lng) / (1 + λ));
                    }
                }
                else if (adjustment > SelectedPositionDistance)
                {
                    var sCoord = new GeoCoordinate(NextSensor.Lat, NextSensor.Lng);
                    var eCoord = new GeoCoordinate(CurrentSensor.Lat, CurrentSensor.Lng);

                    full_calck_distance = Math.Round(sCoord.GetDistanceTo(eCoord), 2);
                    full_manual_distance = NextSensor.Distance - CurrentSensor.Distance;
                    adjustment_percent_factor = (adjustment - SelectedPositionDistance) / (full_manual_distance / 100);
                    if (adjustment_percent_factor >= 100)
                    {
                        OwnerWindow.MainMap.Position = new PointLatLng(NextSensor.Lat, NextSensor.Lng);
                    }
                    else
                    {
                        λ = (full_manual_distance / 100 * adjustment_percent_factor) / (full_manual_distance / 100 * (100 - adjustment_percent_factor)); // am/bm
                        OwnerWindow.MainMap.Position = new PointLatLng((CurrentSensor.Lat + λ * NextSensor.Lat) / (1 + λ), (CurrentSensor.Lng + λ * NextSensor.Lng) / (1 + λ));
                    }
                }
                else // adjustment == SelectedPositionDistance
                {
                    OwnerWindow.MainMap.Position = new PointLatLng(CurrentSensor.Lat, CurrentSensor.Lng);
                }
                OnPropertyChanged(nameof(CalculationInfo));
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
            OnPropertyChanged(nameof(CalculationInfo));
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
