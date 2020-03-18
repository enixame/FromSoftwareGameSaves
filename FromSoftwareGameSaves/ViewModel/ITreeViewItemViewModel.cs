using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using FromSoftwareGameSaves.Model;

namespace FromSoftwareGameSaves.ViewModel
{
    public interface ITreeViewItemViewModel : INotifyPropertyChanged
    {
        ObservableCollection<ITreeViewItemViewModel> Children { get; }

        ITreeViewItemViewModel Parent { get; }

        ViewModelBase Root { get; }

        FromSoftwareFile FromSoftwareFile { get; }
        string RootPath { get; }

        bool HasDummyChild { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        bool IsInEditMode { get; set; }
        bool CanBeEdited { get; }
        bool? IsDirectory { get; }

        Task CommitAsync();
        void Cancel();
        void Delete();
        void New();
        Task RefreshAsync();
    }
}
