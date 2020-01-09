using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace WpfApp.models
{
    public abstract class AbstractModel : INotifyPropertyChanged
    {
        [Key]
        public int Id { get; set; }

        private int orderIndex;
        public int OrderIndex
        {
            get=> orderIndex;
            set
            {
                orderIndex = value;
                OnPropertyChanged(nameof(OrderIndex));
            }
        }

        private string information;
        [ConcurrencyCheck]
        public string Information
        {
            get { return information; }
            set
            {
                information = value;
                OnPropertyChanged(nameof(Information));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #region StripedRowBackground
        private static bool StripedRowBackgroundToggler = true;
        public string StripedRowBackground
        {
            get
            {
                StripedRowBackgroundToggler = !StripedRowBackgroundToggler;

                return StripedRowBackgroundToggler ? "LightGray" : "";
            }
        }
        #endregion
                
        /*public override bool Equals(object other)
        {
            if (Id == 0 || other is null || other.GetType() != this.GetType())
                return false;

            AbstractModel norm_other = (AbstractModel)other;
            if (norm_other.Id == 0)
                return false;

            return this.Id.Equals(norm_other.Id);
        }

        public override int GetHashCode()
        {
            if (Id == 0)
                return 0;

            return (this.GetType().Name + this.Id.ToString()).GetHashCode();
        }

        public static bool operator ==(AbstractModel a1, AbstractModel a2)
        {
            if (a1 is null && a2 is null)
                return true;
            else if (a1 is null || a2 is null)
                return false;
            //
            return a1.Equals(a2);
        }

        public static bool operator !=(AbstractModel a1, AbstractModel a2)
        {
            if (a1 is null && a2 is null)
                return true;
            else if (a1 is null && !(a2 is null) || a2 is null && !(a1 is null))
                return true;
            //
            return !a1.Equals(a2);
        }*/
    }
}
