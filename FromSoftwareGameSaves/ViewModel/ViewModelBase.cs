using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace FromSoftwareGameSaves.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected readonly Dispatcher UiDispatcher = Application.Current.Dispatcher;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
