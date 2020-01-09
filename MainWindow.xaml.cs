using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GpsMapRoutes.CustomMarkers;
using GpsMapRoutes.models;

namespace GpsMapRoutes
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ApplicationViewModel();
            MainMap.OnPositionChanged += (DataContext as ApplicationViewModel).MainMap_OnPositionChanged;
            MainMap.MapProvider = GMapProviders.YandexMap;
            
        }

        private void PipelinesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainMap.Markers.Clear();
            PipelineModel p = (PipelineModel)PipelinesList.SelectedItem;

            if (p is null)
            {
                GMapMarker marker = new GMapMarker(new PointLatLng(55.755812, 37.617820));
                marker.ZIndex = 55;
                marker.Shape = new CustomMarkerDemo(this, marker, "Данные отсутствуют");
                MainMap.Markers.Add(marker);

                MainMap.ZoomAndCenterMarkers(null);
                return;
            }

            if (p.Sensors.Count == 0)
            {
                GMapMarker marker = new GMapMarker(new PointLatLng(55.755812, 37.617820));
                marker.ZIndex = 55;
                marker.Shape = new CustomMarkerDemo(this, marker, "Данные отсутствуют");
                MainMap.Markers.Add(marker);

                MainMap.ZoomAndCenterMarkers(null);
            }
            else
            {
                GMapRoute mRoute = new GMapRoute(p.Sensors.OrderByDescending(x=>x.OrderIndex).Select(x => new PointLatLng(x.Lat, x.Lng)));
                {
                    mRoute.ZIndex = -1;
                }

                MainMap.Markers.Add(mRoute);

                MainMap.ZoomAndCenterMarkers(null);
            }
        }
    }
}
