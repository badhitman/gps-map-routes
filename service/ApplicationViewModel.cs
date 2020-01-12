////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using GpsMapRoutes.models;
using GpsMapRoutes.service;
using GpsMapRoutes.service.commands;

namespace GpsMapRoutes
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public PipelinesContext db { get; }
        public MainWindow OwnerWin { get; }
        public static PointLatLng DefaultPoint = new PointLatLng(55.755812, 37.617820);

        private bool autoPositionCenter = false;

        private void SaveSensorState()
        {
            if (SelectedSensor is null)
            {
                return;
            }

            SelectedSensor.Lat = Lat;
            SelectedSensor.Lng = Lng;
            SelectedSensor.Distance = Distance;

            db.Sensors.Attach(SelectedSensor);
            db.Entry(SelectedSensor).State = EntityState.Unchanged;
            db.Entry(SelectedSensor).Property(x => x.Lat).IsModified = true;
            db.Entry(SelectedSensor).Property(x => x.Lng).IsModified = true;
            db.Entry(SelectedSensor).Property(x => x.Distance).IsModified = true;
            db.SaveChangesAsync();
            
            ReloadPipe();

            if (autoPositionCenter)
            {
                OwnerWin.MainMap.Position = new PointLatLng(Lat, Lng);
            }
        }

        #region commands

        #region pipelines
        private RelayCommand addPipelineCommand;
        public RelayCommand AddPipelineCommand
        {
            get
            {
                return addPipelineCommand ??
                  (addPipelineCommand = new RelayCommand(obj =>
                  {
                      PipelineModel p = new PipelineModel() { Information = "Новый трубопровод", Sensors = new List<SensorModel>() };
                      db.Pipelines.Add(p);
                      SelectedPipeline = p;
                  }));
            }
        }

        private RelayCommand delPipelineCommand;
        public RelayCommand DelPipelineCommand
        {
            get
            {
                return delPipelineCommand ??
            (delPipelineCommand = new RelayCommand(obj =>
            {
                PipelineModel p = obj as PipelineModel;
                if (p != null)
                {
                    Pipelines.Remove(p);
                }
            },
            (obj) => Pipelines.Count > 0));
            }
        }
        #endregion

        #region sensors
        private RelayCommand addSensorCommand;
        public RelayCommand AddSensorCommand
        {
            get
            {
                return addSensorCommand ??
                  (addSensorCommand = new RelayCommand(obj =>
                  {
                      int currentPiplineId = SelectedPipeline.Id;

                      SensorModel s = new SensorModel(Lat, Lng, SelectedPipeline.Id)
                      {
                          Distance = Distance,
                          OrderIndex = db.Sensors.Any(x => x.PipelineId == currentPiplineId) ? db.Sensors.Where(x => x.PipelineId == currentPiplineId).Max(x => x.OrderIndex) + 1 : 1
                      };

                      db.Sensors.Add(s);
                      db.SaveChanges();
                      Lat = 0;
                      Lng = 0;
                      Distance = 0;
                      ReloadPipe();
                  },
                (obj) => !(SelectedPipeline is null)));
            }
        }

        private RelayCommand upSensorCommand;
        public RelayCommand UpSensorCommand
        {
            get
            {
                return upSensorCommand ??
                  (upSensorCommand = new RelayCommand(obj =>
                  {
                      SensorModel selectedSensor = obj as SensorModel;
                      if (selectedSensor is null)
                          return;

                      int positionSensor = CurrentSensors.ToList().FindIndex(x => x.Id == selectedSensor.Id);
                      if (positionSensor == 0)
                          return;

                      SensorModel prevSensor = db.Sensors.Find(CurrentSensors[positionSensor - 1].Id);
                      SensorModel nextSensor = db.Sensors.Find(CurrentSensors[positionSensor].Id);
                      prevSensor.OrderIndex--;
                      nextSensor.OrderIndex++;
                      db.Entry(prevSensor).State = EntityState.Modified;
                      db.Entry(nextSensor).State = EntityState.Modified;
                      db.SaveChanges();

                      OnPropertyChanged(nameof(CurrentSensors));
                      ReloadPipe();
                  },
                (obj) => !(SelectedSensor is null)));
            }
        }

        private RelayCommand downSensorCommand;
        public RelayCommand DownSensorCommand
        {
            get
            {
                return downSensorCommand ??
                  (downSensorCommand = new RelayCommand(obj =>
                  {
                      SensorModel selectedSensor = obj as SensorModel;
                      if (selectedSensor is null)
                          return;

                      int positionSensor = CurrentSensors.ToList().FindIndex(x => x.Id == selectedSensor.Id);
                      if (positionSensor == CurrentSensors.Count - 1)
                          return;

                      SensorModel prevSensor = db.Sensors.Find(CurrentSensors[positionSensor].Id);
                      SensorModel nextSensor = db.Sensors.Find(CurrentSensors[positionSensor + 1].Id);
                      prevSensor.OrderIndex--;
                      nextSensor.OrderIndex++;
                      db.Entry(prevSensor).State = EntityState.Modified;
                      db.Entry(nextSensor).State = EntityState.Modified;
                      db.SaveChanges();

                      OnPropertyChanged(nameof(CurrentSensors));
                      ReloadPipe();
                  },
                (obj) => !(SelectedSensor is null)));
            }
        }

        private RelayCommand removeSensorCommand;
        public RelayCommand RemoveSensorCommand
        {
            get
            {
                return removeSensorCommand ??
                  (removeSensorCommand = new RelayCommand(obj =>
                  {
                      SensorModel selectedSensor = obj as SensorModel;
                      if (selectedSensor is null)
                          return;

                      db.Sensors.Remove(selectedSensor);
                      db.SaveChanges();

                      OnPropertyChanged(nameof(CurrentSensors));
                      ReloadPipe();
                  },
                (obj) => !(SelectedSensor is null)));
            }
        }

        private RelayCommand openSensorCommand;
        public RelayCommand OpenSensorCommand
        {
            get
            {
                return openSensorCommand ??
                  (openSensorCommand = new RelayCommand(obj =>
                  {
                      SensorModel selectedSenderSensor = obj as SensorModel;
                      if (selectedSensor is null)
                          return;

                      OwnerWin.MainMap.Position = new PointLatLng(selectedSensor.Lat, selectedSensor.Lng);

                      SensorWindow sensorEditWindow = new SensorWindow(this);

                      lat = selectedSenderSensor.Lat;
                      lng = selectedSenderSensor.Lng;
                      distance = selectedSenderSensor.Distance;
                      OnPropertyChanged(nameof(Lat)); OnPropertyChanged(nameof(Lng)); OnPropertyChanged(nameof(Distance));

                      autoPositionCenter = true;
                      sensorEditWindow.ShowDialog();
                      autoPositionCenter = false;

                      lat = 0;
                      lng = 0;
                      distance = 0;
                      OnPropertyChanged(nameof(Lat)); OnPropertyChanged(nameof(Lng)); OnPropertyChanged(nameof(Distance));
                  },
                (obj) => !(SelectedSensor is null)));
            }
        }
        #endregion

        #endregion

        #region collections
        public ObservableCollection<PipelineModel> Pipelines { get; set; }

        /// <summary>
        /// Отсортированый список (вверху конец маршрута, а внизу начало)
        /// </summary>
        public List<SensorModel> CurrentSensorsList => SelectedPipeline?.Sensors?.OrderByDescending(x => x.OrderIndex).ToList() ?? new List<SensorModel>();
        /// <summary>
        /// Отсортированый список (вверху конец маршрута, а внизу начало)
        /// </summary>
        public ObservableCollection<SensorModel> CurrentSensors
        {
            get
            {
                int selectedSensorId = SelectedSensor?.Id ?? 0;

                ObservableCollection<SensorModel> currentSensors = new ObservableCollection<SensorModel>(CurrentSensorsList);
                if (selectedSensorId > 0)
                {
                    OwnerWin.sensorsList.SelectedItem = CurrentSensorsList[CurrentSensorsList.FindIndex(x => x.Id == selectedSensorId)];
                }

                return currentSensors;
            }
        }
        #endregion

        #region props
        private PipelineModel selectedPipeline;
        public PipelineModel SelectedPipeline
        {
            get { return selectedPipeline; }
            set
            {
                selectedPipeline = value;
                Lat = 0;
                Lng = 0;
                PipeName = value?.Information;

                OnPropertyChanged(nameof(SelectedPipeline));
                // формируем свежий список точек
                OnPropertyChanged(nameof(CurrentSensors));

                OnPropertyChanged(nameof(SelectedPipelineTitle));
                ReloadPipe();
            }
        }
        public string SelectedPipelineTitle => SelectedPipeline?.Title ?? "Выберите маршрут";

        private bool isPipelineSelected = false;
        /// <summary>
        /// Используется для контроля доступности (вкл/выкл) контрола Expander.
        /// Для включения и отключения доступности любого контрола (с зависимостью выбран ли сейчас какой-либо трубопровод)
        /// </summary>
        public bool IsPipelineSelected
        {
            get
            {
                bool newStateProp = !(SelectedPipeline is null);
                if (isPipelineSelected != newStateProp)
                {
                    isPipelineSelected = newStateProp;
                    OnPropertyChanged(nameof(IsPipelineSelected));
                }
                return isPipelineSelected;
            }
        }

        private string pipeName;
        public string PipeName
        {
            get => pipeName;
            set
            {
                pipeName = value;
                if (!(SelectedPipeline is null))
                    SelectedPipeline.Information = pipeName;

                OnPropertyChanged(nameof(PipeName));
                db.SaveChanges();
            }
        }

        private double lat;
        public double Lat
        {
            get => lat;
            set
            {
                lat = value;
                OnPropertyChanged(nameof(Lat));
                SaveSensorState();
            }
        }

        private double lng;
        public double Lng
        {
            get => lng;
            set
            {
                lng = value;
                OnPropertyChanged(nameof(Lng));
                SaveSensorState();
            }
        }

        private double distance;
        public double Distance
        {
            get => distance;
            set
            {
                distance = value;
                OnPropertyChanged(nameof(Distance));
                SaveSensorState();
            }
        }

        private SensorModel selectedSensor;
        public SensorModel SelectedSensor
        {
            get => selectedSensor;
            set
            {
                selectedSensor = value;
                OnPropertyChanged(nameof(SelectedSensor));
            }
        }

        private PointLatLng currentMapPoint;
        public PointLatLng CurrentMapPoint
        {
            get => currentMapPoint;
            set
            {
                currentMapPoint = value;
                OnPropertyChanged(nameof(CurrentMapPoint));
            }
        }

        private string status;
        public string Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        #endregion

        void ReloadPipe()
        {
            OnPropertyChanged(nameof(IsPipelineSelected));

            PipelineModel p = SelectedPipeline;
            OwnerWin.MainMap.Markers.Clear();
            OnPropertyChanged(nameof(CurrentSensors));
            if (p is null)
            {
                GMapMarker marker = new GMapMarker(DefaultPoint);
                marker.ZIndex = 55;
                marker.Shape = new CustomMarkerDemo(OwnerWin, marker, "Данные отсутствуют");
                OwnerWin.MainMap.Markers.Add(marker);

                OwnerWin.MainMap.ZoomAndCenterMarkers(null);
                return;
            }

            if (p.Sensors.Count == 0)
            {
                GMapMarker marker = new GMapMarker(DefaultPoint);
                marker.ZIndex = 55;
                marker.Shape = new CustomMarkerDemo(OwnerWin, marker, "Данные отсутствуют");
                OwnerWin.MainMap.Markers.Add(marker);

                OwnerWin.MainMap.ZoomAndCenterMarkers(null);
            }
            else
            {
                GMapRoute mRoute = new GMapRoute(CurrentSensors.Select(x => new PointLatLng(x.Lat, x.Lng)));
                {
                    mRoute.ZIndex = -1;
                }

                OwnerWin.MainMap.Markers.Add(mRoute);

                OwnerWin.MainMap.ZoomAndCenterMarkers(null);
            }
        }

        public ApplicationViewModel(MainWindow setWin)
        {
            db = new PipelinesContext();
            db.Pipelines.Include(x => x.Sensors).Load();
            Pipelines = db.Pipelines.Local;
            OwnerWin = setWin;
            OwnerWin.MainMap.OnPositionChanged += MainMap_OnPositionChanged;
        }

        public void MainMap_OnPositionChanged(PointLatLng point)
        {
            Status = point.Lat.ToString(CultureInfo.InvariantCulture) + ", " + point.Lng.ToString(CultureInfo.InvariantCulture);
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
