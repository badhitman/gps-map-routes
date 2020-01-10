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

        private ApplicationSensorViewModel ContextModel => DataContext as ApplicationSensorViewModel;

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ContextModel.OnPropertyChanged(nameof(ContextModel.PrevSensorDistance));
            ContextModel.OnPropertyChanged(nameof(ContextModel.MidleSensorDistance));
            ContextModel.OnPropertyChanged(nameof(ContextModel.NextSensorDistance));
            ContextModel.OnPropertyChanged(nameof(ContextModel.CalculationInfo));
            ContextModel.OnPropertyChanged(nameof(ContextModel.SelectedPositionDistance));
            ContextModel.OnPropertyChanged(nameof(ContextModel.Adjustment));
        }
    }
}
