using GMap.NET.MapProviders;
using System.Windows;

namespace GpsMapRoutes
{
    /// <summary>
    /// Логика взаимодействия для SensorWindow.xaml
    /// </summary>
    public partial class SensorWindow : Window
    {
        public SensorWindow(ApplicationSensorViewModel sensorContext)
        {
            DataContext = sensorContext;
            InitializeComponent();
            MainMap.MapProvider = GMapProviders.YandexMap;
            MainMap.Position = new GMap.NET.PointLatLng(sensorContext.CurrentSensor.Lat, sensorContext.CurrentSensor.Lng);
        }
    }
}
