using System;
using System.IO;
using System.Linq;
using System.Windows;
using FileSystemManager;
using FromSoftwareGameSaves.Utils;
using FromSoftwareModel;
using File = FromSoftwareGameSaves.Model.File;

namespace FromSoftwareGameSaves.ViewModel
{
    public sealed class FileViewModel : TreeViewItemViewModel
    {
        private string _fileName;

        public FileViewModel(ViewModelBase root, File file, ITreeViewItemViewModel parent) 
            : base(root, parent, file.IsDirectory)
        {
            File = file;
            CanBeEdited = true;
            _backupFileName = file.FileName;
            FileName = file.FileName;
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

        public override bool? IsDirectory => File.IsDirectory;

        public override void Cancel()
        {
            _fileName = _backupFileName;
            OnPropertyChanged("FileName");
        }

        public override void Commit()
        {
            var pathSource = Path.Combine(File.Path, File.FileName);
            var pathDest = Path.Combine(File.Path, _fileName);

            try
            {
                var oldPath = Path.Combine(FromSoftwareFileInfo.AppDataPath, pathSource);
                var newPath = Path.Combine(FromSoftwareFileInfo.AppDataPath, pathDest);

                var isDirectoryChanged = FileSystem.Rename(oldPath, newPath);
                if (!isDirectoryChanged)
                {
                    Cancel();
                    return;
                }

                File = new File(_fileName, File.IsDirectory, File.Path);
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
            if (!treeViewItemViewModel.File.IsDirectory)
                return;

            if (treeViewItemViewModel.Children == null)
                return;

            if (!treeViewItemViewModel.Children.Any())
                return;

            if (treeViewItemViewModel.HasDummyChild)
                return;

            var childPath = Path.Combine(treeViewItemViewModel.File.Path, treeViewItemViewModel.File.FileName);

            foreach (var child in treeViewItemViewModel.Children)
            {
                child.File.Path = childPath;
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

            var messageBoxResult = MessageBoxHelper.ShowMessage($"Do you want to delete {File.FileName} ?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.No)
                return;

            var pathToDelete = Path.Combine(FromSoftwareFileInfo.AppDataPath, File.Path, File.FileName);

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

        public FileViewModel AcceptCopy(ITreeViewItemViewModel treeViewItemViewModel)
        {
            var pathSource = Path.Combine(treeViewItemViewModel.File.Path, treeViewItemViewModel.File.FileName);
            var pathDest = Path.Combine(File.Path, File.FileName, treeViewItemViewModel.File.FileName);

            var messageBoxResult = MessageBoxHelper.ShowMessage($"Do you want to copy {pathSource} to {pathDest} ?", "Copy", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.No)
                return null;

            try
            {
                if (!FileSystem.Copy(Path.Combine(FromSoftwareFileInfo.AppDataPath, pathSource), Path.Combine(FromSoftwareFileInfo.AppDataPath, pathDest), FromSoftwareFileInfo.FileSearchpattern, treeViewItemViewModel.IsDirectory ?? true))
                    return null;

                IsSelected = true;
                Refresh();

                var newItem = Children.OfType<FileViewModel>().FirstOrDefault(child => child.File.FileName.Equals(treeViewItemViewModel.File.FileName));
                return newItem;
            }
            catch (Exception exception)
            {
                MessageBoxHelper.ShowMessage(exception.Message, "Cannot copy", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public void ExpandAll()
        {
            if (!File.IsDirectory)
                return;

            if (Children == null)
                return;

            if (!Children.Any())
                return;

            IsExpanded = true;
            foreach (var child in Children.OfType<FileViewModel>())
            {
                child.ExpandAll();
            }
        }
    }
}
