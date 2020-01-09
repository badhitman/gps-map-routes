using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GpsMapRoutes
{
    public class ApplicationSensorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ApplicationViewModel owner;
        public ApplicationViewModel Owner
        {
            get => owner;
            set
            {
                owner = value;
                OnPropertyChanged(nameof(Owner));
            }

        }

        protected double adjustment;
        public double Adjustment
        {
            get => adjustment;
            set
            {
                adjustment = value;
                OnPropertyChanged(nameof(Adjustment));
            }
        }

        public ApplicationSensorViewModel(ApplicationViewModel setOwner)
        {
            Owner = setOwner;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
