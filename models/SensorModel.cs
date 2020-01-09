////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace GpsMapRoutes.models
{
    public class SensorModel : AbstractModel
    {
        public SensorModel() { }
        public SensorModel(double lat, double lng, int pipe)
        {
            Lat = lat;
            Lng = lng;
            PipelineId = pipe;
        }

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
    }
}