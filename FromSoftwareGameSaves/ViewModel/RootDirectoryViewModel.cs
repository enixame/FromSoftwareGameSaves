using System.Windows.Media;
using FromSoftwareGameSaves.Model;
using FromSoftwareGameSaves.Utils;

namespace FromSoftwareGameSaves.ViewModel
{
    public class RootDirectoryViewModel : TreeViewItemViewModel
    {
        public RootDirectoryViewModel(ViewModelBase root, File file)
            : base(root, null, true)
        {
            File = file;
            CanBeEdited = false;
            ImagePath = ImageHelper.BuildImageSource(file.FileName, ImageHelper.ImageExtensionJpg);
        }

        public string RootName => File.FileName;

        public ImageSource ImagePath { get; }
    }
}
