using System.Windows.Media;
using FromSoftwareGameSaves.Model;
using FromSoftwareGameSaves.Utils;

namespace FromSoftwareGameSaves.ViewModel
{
    public class RootDirectoryViewModel : TreeViewItemViewModel
    {
        public RootDirectoryViewModel(ViewModelBase root, FromSoftwareFile fromSoftwareFile)
            : base(root, null, true)
        {
            FromSoftwareFile = fromSoftwareFile;
            CanBeEdited = false;
            ImagePath = ImageHelper.BuildImageSourceFromDatabase(fromSoftwareFile.GameName);
        }

        public string RootName => FromSoftwareFile.FileName;

        public ImageSource ImagePath { get; }
    }
}
