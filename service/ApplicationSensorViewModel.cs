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
using System.Data.Entity;
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
        //public int SelectedSensorId => OwnerContext.SelectedSensor?.Id ?? 0;
        public int SelectedPositionSensorInList => OwnerContext.SelectedSensor.Id <= 0 ? -1 : OwnerContext.CurrentSensorsList.FindIndex(x => x.Id == OwnerContext.SelectedSensor.Id);
        public double SelectedSensorDistance => OwnerContext.SelectedSensor is null ? -1 : OwnerContext.SelectedSensor.Distance;
        public SensorModel NextSensor => SelectedPositionSensorInList > 0 ? OwnerContext.CurrentSensorsList[SelectedPositionSensorInList - 1] : null;

        public GMapRoute SelectedSegmentMapRoute
        {
            get
            {
                if (OwnerContext.SelectedSensor is null)
                {
                    return new GMapRoute(new PointLatLng[] { ApplicationViewModel.DefaultPoint }) { ZIndex = -1 };
                }

                if (PrewSensor is null && NextSensor is null)
                {
                    return new GMapRoute(new PointLatLng[] { new PointLatLng(OwnerContext.SelectedSensor.Lat, OwnerContext.SelectedSensor.Lng) }) { ZIndex = -1 };
                }

                List<PointLatLng> points = new List<PointLatLng>();

                if (!(PrewSensor is null))
                {
                    points.Add(new PointLatLng(PrewSensor.Lat, PrewSensor.Lng));
                }
                points.Add(new PointLatLng(OwnerContext.SelectedSensor.Lat, OwnerContext.SelectedSensor.Lng));
                if (!(NextSensor is null))
                {
                    points.Add(new PointLatLng(NextSensor.Lat, NextSensor.Lng));
                }

                return new GMapRoute(points) { ZIndex = -1 };
            }
        }

        public double MinAdjustment => PrewSensor?.Distance ?? SelectedSensorDistance;
        public double MaxAdjustment => NextSensor?.Distance ?? SelectedSensorDistance;

        #region структурные заголовки
        public string PrevSensorTitle => PrewSensor is null || (PrewSensor is null && NextSensor is null) ? "Текущий" : "Предыдущий";
        public string MidleSensorTitle => PrewSensor is null || NextSensor is null ? "" : "Текущий";
        public string NextSensorTitle => PrewSensor is null && NextSensor is null ? "" : NextSensor is null ? "Текущий" : "Следующий";
        #endregion

        #region установленные пользователем distance
        public string PrevSensorDistance => PrewSensor is null || (PrewSensor is null && NextSensor is null) ? SelectedSensorDistance.ToString() : PrewSensor.Distance.ToString() + " (-" + (SelectedSensorDistance - PrewSensor.Distance).ToString() + ")";
        public string MidleSensorDistance => PrewSensor is null || NextSensor is null ? "" : SelectedSensorDistance.ToString();
        public string NextSensorDistance => PrewSensor is null && NextSensor is null ? "" : NextSensor is null ? SelectedSensorDistance.ToString() : "(+" + (NextSensor.Distance - SelectedSensorDistance) + ") " + NextSensor.Distance.ToString();
        #endregion

        public string Information
        {
            get => OwnerContext.SelectedSensor?.Information;
            set
            {
                if (OwnerContext.SelectedSensor is null)
                {
                    return;
                }

                OwnerContext.SelectedSensor.Information = value;
                OnPropertyChanged(nameof(Information));

                OwnerContext.db.Sensors.Attach(OwnerContext.SelectedSensor);
                OwnerContext.db.Entry(OwnerContext.SelectedSensor).State = EntityState.Unchanged;
                OwnerContext.db.Entry(OwnerContext.SelectedSensor).Property(x => x.Information).IsModified = true;
                OwnerContext.db.SaveChangesAsync();
            }
        }

        public string DistanceMetadata
        {
            get
            {
                string ret_info = string.Empty;

                if (PrewSensor is null && NextSensor is null)
                {
                    ret_info += "\nПредыдущих или следующих точек не обнаружено";
                }
                else if (!(PrewSensor is null) && !(NextSensor is null))
                {
                    var sCoord = new GeoCoordinate(PrewSensor.Lat, PrewSensor.Lng);
                    var eCoord = new GeoCoordinate(OwnerContext.SelectedSensor.Lat, OwnerContext.SelectedSensor.Lng);
                    ret_info += "\nот предыдущей ≈ " + Math.Round(sCoord.GetDistanceTo(eCoord), 2) + " метров.";
                    ret_info += "\nрасчётная дистанция -> ≈ " + Math.Round(PrewSensor.Distance + sCoord.GetDistanceTo(eCoord), 2) + " м.\n";

                    sCoord = new GeoCoordinate(NextSensor.Lat, NextSensor.Lng);
                    ret_info += "\nдо следующей ≈ " + Math.Round(sCoord.GetDistanceTo(eCoord), 2) + " метров.";
                    ret_info += "\nрасчётная дистанция <- ≈ " + Math.Round(NextSensor.Distance - sCoord.GetDistanceTo(eCoord), 2) + " м.\n";
                }
                else if (PrewSensor is null)
                {
                    var sCoord = new GeoCoordinate(NextSensor.Lat, NextSensor.Lng);
                    var eCoord = new GeoCoordinate(OwnerContext.SelectedSensor.Lat, OwnerContext.SelectedSensor.Lng);
                    ret_info += "\nдо следующей ≈ " + sCoord.GetDistanceTo(eCoord) + " метров.";
                }
                else if (NextSensor is null)
                {
                    var sCoord = new GeoCoordinate(PrewSensor.Lat, PrewSensor.Lng);
                    var eCoord = new GeoCoordinate(OwnerContext.SelectedSensor.Lat, OwnerContext.SelectedSensor.Lng);
                    ret_info += "от предыдущей ≈ " + sCoord.GetDistanceTo(eCoord) + " метров.";
                    ret_info += "\nрасчётная дистанция ≈ " + Math.Round(PrewSensor.Distance - sCoord.GetDistanceTo(eCoord), 2) + " м.\n";
                }

                return ret_info.Trim();
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
                    cal_info += "\nОтклонение (" + (adjustment < SelectedSensorDistance ? "назад" : "вперёд") + "): " +
                        "\nGPS: " + Math.Round(full_calck_distance / 100 * adjustment_percent_factor, 2) + "/" + full_calck_distance + " м." +
                        "\nMth: " + Math.Round(full_manual_distance / 100 * adjustment_percent_factor, 2) + "/" + full_manual_distance + " м.";
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
                if (adjustment < SelectedSensorDistance)
                {
                    var sCoord = new GeoCoordinate(PrewSensor.Lat, PrewSensor.Lng);
                    var eCoord = new GeoCoordinate(OwnerContext.SelectedSensor.Lat, OwnerContext.SelectedSensor.Lng);

                    full_calck_distance = Math.Round(sCoord.GetDistanceTo(eCoord), 2);
                    full_manual_distance = OwnerContext.SelectedSensor.Distance - PrewSensor.Distance;
                    adjustment_percent_factor = (SelectedSensorDistance - adjustment) / (full_manual_distance / 100);
                    if (adjustment_percent_factor >= 100)
                    {
                        OwnerWindow.MainMap.Position = new PointLatLng(PrewSensor.Lat, PrewSensor.Lng);
                    }
                    else
                    {
                        λ = (full_manual_distance / 100 * (100 - adjustment_percent_factor)) / (full_manual_distance / 100 * adjustment_percent_factor); // am/bm
                        OwnerWindow.MainMap.Position = new PointLatLng((PrewSensor.Lat + λ * OwnerContext.SelectedSensor.Lat) / (1 + λ), (PrewSensor.Lng + λ * OwnerContext.SelectedSensor.Lng) / (1 + λ));
                    }
                }
                else if (adjustment > SelectedSensorDistance)
                {
                    var sCoord = new GeoCoordinate(NextSensor.Lat, NextSensor.Lng);
                    var eCoord = new GeoCoordinate(OwnerContext.SelectedSensor.Lat, OwnerContext.SelectedSensor.Lng);

                    full_calck_distance = Math.Round(sCoord.GetDistanceTo(eCoord), 2);
                    full_manual_distance = NextSensor.Distance - OwnerContext.SelectedSensor.Distance;
                    adjustment_percent_factor = (adjustment - SelectedSensorDistance) / (full_manual_distance / 100);
                    if (adjustment_percent_factor >= 100)
                    {
                        OwnerWindow.MainMap.Position = new PointLatLng(NextSensor.Lat, NextSensor.Lng);
                    }
                    else
                    {
                        λ = (full_manual_distance / 100 * adjustment_percent_factor) / (full_manual_distance / 100 * (100 - adjustment_percent_factor)); // am/bm
                        OwnerWindow.MainMap.Position = new PointLatLng((OwnerContext.SelectedSensor.Lat + λ * NextSensor.Lat) / (1 + λ), (OwnerContext.SelectedSensor.Lng + λ * NextSensor.Lng) / (1 + λ));
                    }
                }
                else // adjustment == SelectedPositionDistance
                {
                    OwnerWindow.MainMap.Position = new PointLatLng(OwnerContext.SelectedSensor.Lat, OwnerContext.SelectedSensor.Lng);
                }

                OnPropertyChanged(nameof(CalculationInfo));
                // OnPropertyChanged(nameof(DistanceMetadata));
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
                      Adjustment = SelectedSensorDistance;
                  },
                (obj) => (true)));
            }
        }
        #endregion

        public ApplicationSensorViewModel(ApplicationViewModel setOwner, SensorWindow sensorWindow)
        {
            OwnerContext = setOwner;
            OwnerWindow = sensorWindow;
            OwnerWindow.MainMap.OnPositionChanged += delegate (PointLatLng point) { OnPropertyChanged(nameof(CalculationInfo)); };
            OwnerWindow.MainMap.Loaded += delegate (object sender, System.Windows.RoutedEventArgs e) { Adjustment = SelectedSensorDistance; };
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
