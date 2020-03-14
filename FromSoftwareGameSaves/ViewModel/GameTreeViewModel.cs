using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using FromSoftwareGameSaves.Commands;
using FromSoftwareGameSaves.Utils;
using FromSoftwareModel;
using File = FromSoftwareGameSaves.Model.File;

namespace FromSoftwareGameSaves.ViewModel
{
    public class GameTreeViewModel : ViewModelBase
    {
        private const string ExplorerProcessName = "explorer.exe";

        private ITreeViewItemViewModel _selectedItem;

        private readonly DragDropFileViewModel _dragDropFileViewModel = new DragDropFileViewModel();

        public GameTreeViewModel(IEnumerable<File> roots)
        {
            GameRootsDirectory = new ReadOnlyCollection<RootDirectoryViewModel>(
                (from file in roots
                 select new RootDirectoryViewModel(this, file)).ToList());

            Edit = new DelegateCommand(arg =>
            {
                var selectedModel = SelectedItem;
                if (selectedModel != null && selectedModel.CanBeEdited && !selectedModel.IsInEditMode)
                    selectedModel.IsInEditMode = true;
            });

            Commit = new DelegateCommand(arg =>
            {
                var selectedModel = SelectedItem;
                if (selectedModel == null || !selectedModel.IsInEditMode) return;

                selectedModel.IsInEditMode = false;
                selectedModel.Commit();
            });

            Cancel = new DelegateCommand(arg =>
            {
                var selectedModel = SelectedItem;
                if (selectedModel == null || !selectedModel.IsInEditMode) return;

                selectedModel.IsInEditMode = false;
                selectedModel.Cancel();
            });

            New = new DelegateCommand(arg =>
            {
                var selectedModel = SelectedItem;
                selectedModel?.New();
            });

            Delete = new DelegateCommand(arg =>
            {
                var selectedModel = SelectedItem;
                selectedModel?.Delete();
            });

            Refresh = new DelegateCommand(arg =>
            {
                var selectedModel = SelectedItem;
                selectedModel?.Refresh();
            });

            OpenInExplorer = new DelegateCommand(arg =>
            {
                var selectedModel = SelectedItem;
                if (selectedModel == null) return;

                var path = Path.Combine(FromSoftwareFileInfo.AppDataPath, selectedModel.File.Path, selectedModel.File.FileName);

                try
                {
                    System.Diagnostics.Process.Start(ExplorerProcessName, path);
                }
                catch (Exception exception)
                {
                     MessageBoxHelper.ShowMessage(exception.Message, "Cannot open folder", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            TreeViewItemRigthClick = new DelegateCommand<TreeViewItemViewModel>(item =>
            {
                if (item == null) return;           
                item.IsSelected = true;
            });

            DropCommand = new DelegateCommand<DragDropInfoViewModel<FileViewModel>>(arg =>
            {
                _dragDropFileViewModel.DoDragDrop(arg);
            });
        }

        public ReadOnlyCollection<RootDirectoryViewModel> GameRootsDirectory { get; }

        public DelegateCommand Edit { get; }

        public DelegateCommand Delete { get; }

        public DelegateCommand Commit { get; }

        public DelegateCommand Cancel { get; }

        public DelegateCommand New { get; }

        public DelegateCommand Refresh { get; }

        public DelegateCommand OpenInExplorer { get; }

        public DelegateCommand<DragDropInfoViewModel<FileViewModel>> DropCommand { get; }

        public DelegateCommand<TreeViewItemViewModel> TreeViewItemRigthClick { get; }

        /// <summary>
        /// Gets ot sets selected item.
        /// </summary>
        public ITreeViewItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != null && _selectedItem.IsInEditMode)
                {
                    _selectedItem.Cancel();
                    _selectedItem.IsInEditMode = false;
                }

                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
    }
}
