using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FromSoftwareFileManager;
using FromSoftwareGameSaves.Model;
using FromSoftwareGameSaves.Utils;

namespace FromSoftwareGameSaves.ViewModel
{
    public sealed class FileViewModel : TreeViewItemViewModel
    {
        private string _fileName;

        public FileViewModel(ViewModelBase root, FromSoftwareFile fromSoftwareFile, ITreeViewItemViewModel parent) 
            : base(root, parent, fromSoftwareFile.IsDirectory)
        {
            FromSoftwareFile = fromSoftwareFile;
            CanBeEdited = true;
            _backupFileName = fromSoftwareFile.FileName;
            FileName = fromSoftwareFile.FileName;
        }

        private string _backupFileName;

        public string FileName  
        {
            get => _fileName;
            set
            {
                if (value == _fileName)
                    return;

                _fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        public override bool? IsDirectory => FromSoftwareFile.IsDirectory;

        public override void Cancel()
        {
            _fileName = _backupFileName;
            OnPropertyChanged("FileName");
        }

        public override void Commit()
        {
            var pathSource = Path.Combine(FromSoftwareFile.Path, FromSoftwareFile.FileName);
            var pathDest = Path.Combine(FromSoftwareFile.Path, _fileName);

            try
            {
                var oldPath = Path.Combine(FromSoftwareFile.RootDirectory, pathSource);
                var newPath = Path.Combine(FromSoftwareFile.RootDirectory, pathDest);

                var isDirectoryChanged = FileSystem.Rename(oldPath, newPath);
                if (!isDirectoryChanged)
                {
                    Cancel();
                    return;
                }

                FromSoftwareFile = new FromSoftwareFile(FromSoftwareFile.RootDirectory, FromSoftwareFile.FileSearchPattern, _fileName, FromSoftwareFile.IsDirectory, FromSoftwareFile.Path);
                _backupFileName = _fileName;

                RefreshPath(this);
            }
            catch (Exception exception)
            {
                Cancel();
                MessageBoxHelper.ShowMessage(exception.Message, "Cannot rename folder", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private static void RefreshPath(ITreeViewItemViewModel treeViewItemViewModel)
        {
            if (!treeViewItemViewModel.FromSoftwareFile.IsDirectory)
                return;

            if (treeViewItemViewModel.Children == null)
                return;

            if (!treeViewItemViewModel.Children.Any())
                return;

            if (treeViewItemViewModel.HasDummyChild)
                return;

            var childPath = Path.Combine(treeViewItemViewModel.FromSoftwareFile.Path, treeViewItemViewModel.FromSoftwareFile.FileName);

            foreach (var child in treeViewItemViewModel.Children)
            {
                child.FromSoftwareFile.Path = childPath;
                RefreshPath(child);
            }
        }

        public override void New()
        {
            if (IsDirectory != true)
                return;

            base.New(); 
        }

        public override void Delete()
        {
            var parentItem = Parent;
            if (parentItem == null)
                return;

            var messageBoxResult = MessageBoxHelper.ShowMessage($"Do you want to delete {FromSoftwareFile.FileName} ?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.No)
                return;

            var pathToDelete = Path.Combine(FromSoftwareFile.RootDirectory, FromSoftwareFile.Path, FromSoftwareFile.FileName);

            try
            {
                if (!FileSystem.Delete(pathToDelete)) return;

                parentItem.IsSelected = true;
                parentItem.Children.Remove(this);
            }
            catch (Exception exception)
            {
                MessageBoxHelper.ShowMessage(exception.Message, "Cannot copy", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task<FileViewModel> AcceptCopyAsync(ITreeViewItemViewModel treeViewItemViewModel)
        {
            var pathSource = Path.Combine(treeViewItemViewModel.FromSoftwareFile.Path, treeViewItemViewModel.FromSoftwareFile.FileName);
            var pathDest = Path.Combine(FromSoftwareFile.Path, FromSoftwareFile.FileName, treeViewItemViewModel.FromSoftwareFile.FileName);

            var messageBoxResult = MessageBoxHelper.ShowMessage($"Do you want to copy {pathSource} to {pathDest} ?", "Copy", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.No)
                return null;

            try
            {
                if (!FileSystem.Copy(Path.Combine(treeViewItemViewModel.FromSoftwareFile.RootDirectory, pathSource),
                    Path.Combine(FromSoftwareFile.RootDirectory, pathDest), FromSoftwareFile.FileSearchPattern,
                    treeViewItemViewModel.IsDirectory ?? true))
                    return null;

                IsSelected = true;

                var newItem = Children.OfType<FileViewModel>().FirstOrDefault(child => child.FromSoftwareFile.FileName.Equals(treeViewItemViewModel.FromSoftwareFile.FileName));

                if (newItem == null)
                {
                    FromSoftwareFile fromSoftwareFile = new FromSoftwareFile(FromSoftwareFile.RootDirectory, FromSoftwareFile.FileSearchPattern, treeViewItemViewModel.FromSoftwareFile.FileName,
                        treeViewItemViewModel.IsDirectory ?? true, Path.Combine(FromSoftwareFile.Path, FromSoftwareFile.FileName));

                    newItem = new FileViewModel(treeViewItemViewModel.Root, fromSoftwareFile, this);
                    await UiDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        Children.Add(newItem);
                    }));
                }

                return newItem;
            }
            catch (Exception exception)
            {
                MessageBoxHelper.ShowMessage(exception.Message, "Cannot copy", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task ExpandAllAsync()
        {
            if (!FromSoftwareFile.IsDirectory)
                return;

            if (Children == null)
                return;

            if (!Children.Any())
                return;

            if (!HasDummyChild)
                return;

            await ExpandAsync();

            foreach (var child in Children.OfType<FileViewModel>())
            {
                await child.ExpandAllAsync();
            }
        }
    }
}
