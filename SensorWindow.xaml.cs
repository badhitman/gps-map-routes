﻿using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Windows;

namespace GpsMapRoutes
{
    /// <summary>
    /// Логика взаимодействия для SensorWindow.xaml
    /// </summary>
    public partial class SensorWindow : Window
    {
        private ApplicationSensorViewModel ContextModel => DataContext as ApplicationSensorViewModel;

        public SensorWindow(ApplicationViewModel sensorContext)
        {
            InitializeComponent();
            DataContext = new ApplicationSensorViewModel(sensorContext, this);
            MainMap.MapProvider = GMapProviders.YandexMap;            
        }

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
