////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace GpsMapRoutes.models
{
    /// <summary>
    /// Сенсор/Датчик/Точка
    /// Точка на маршруте
    /// </summary>
    public class SensorModel : AbstractModel
    {
        public SensorModel() { }
        public SensorModel(double lat, double lng, int pipe)
        {
            Lat = lat;
            Lng = lng;
            PipelineId = pipe;
        }

        /// <summary>
        /// Труба-владелец точки
        /// </summary>
        public PipelineModel Pipeline { get; set; }
        public int PipelineId { get; set; }

        private double lat;
        public double Lat
        {
            get => lat;
            set { lat = value; OnPropertyChanged(nameof(Lat)); }
        }

        private double lng;
        public double Lng
        {
            get => lng;
            set { lng = value; OnPropertyChanged(nameof(Lng)); }
        }

        /// <summary>
        /// Ручная установка дистанции, отсчитываемой от первой точки
        /// </summary>
        private double distance;
        public double Distance
        {
            get=> distance;
            set
            {
                distance = value;
                OnPropertyChanged(nameof(Distance));
            }
        }

    }
}