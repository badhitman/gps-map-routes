using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GpsMapRoutes
{
    public class ApplicationSensorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ApplicationViewModel ownerWin;
        public ApplicationViewModel OwnerWin
        {
            get=> ownerWin;
            set
            {
                ownerWin = value;
                OnPropertyChanged(nameof(OwnerWin));
            }

        }

        public ApplicationSensorViewModel(ApplicationViewModel setOwnerWin)
        {
            OwnerWin = setOwnerWin;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
