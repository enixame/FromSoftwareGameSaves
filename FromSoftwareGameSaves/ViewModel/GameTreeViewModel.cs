using FromSoftwareGameSaves.Commands;
using FromSoftwareGameSaves.Repository;
using FromSoftwareGameSaves.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FromSoftwareGameSaves.Model;

namespace FromSoftwareGameSaves.ViewModel
{
    public class GameTreeViewModel : ViewModelBase
    {
        private const string ExplorerProcessName = "explorer.exe";

        private ITreeViewItemViewModel _selectedItem;

        private readonly DragDropFileViewModel _dragDropFileViewModel = new DragDropFileViewModel();
        private ObservableCollection<RootDirectoryViewModel> _gameRootsDirectory = new ObservableCollection<RootDirectoryViewModel>();

        public GameTreeViewModel()
        {
            LoadGameDirectoriesAsync();
 
            Edit = new DelegateCommand(arg =>
            {
                var selectedModel = SelectedItem;
                if (selectedModel != null && selectedModel.CanBeEdited && !selectedModel.IsInEditMode)
                    selectedModel.IsInEditMode = true;
            });

            Commit = new DelegateAsyncCommand(() =>
            {
                var selectedModel = SelectedItem;
                if (selectedModel == null || !selectedModel.IsInEditMode) 
                    return Task.CompletedTask;

                selectedModel.IsInEditMode = false;
                return selectedModel.Commit();
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

            Refresh = new DelegateAsyncCommand(() =>
            {
                var selectedModel = SelectedItem;
                return selectedModel?.RefreshAsync();
            });

            OpenInExplorer = new DelegateAsyncCommand(() =>
            {
                var selectedModel = SelectedItem;
                if (selectedModel == null) return Task.CompletedTask;

                var path = Path.Combine(selectedModel.FromSoftwareFile.RootDirectory, selectedModel.FromSoftwareFile.Path, selectedModel.FromSoftwareFile.FileName);

                return Task.Run(() =>
                {
                    try
                    {
                        System.Diagnostics.Process.Start(ExplorerProcessName, path);
                    }
                    catch (Exception exception)
                    {
                        MessageBoxHelper.ShowMessage(exception.Message, "Cannot open folder", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            });

            TreeViewItemRigthClick = new DelegateCommand<TreeViewItemViewModel>(item =>
            {
                if (item == null) return;           
                item.IsSelected = true;
            });

            DropCommand = new DelegateAsyncCommand<DragDropInfoViewModel<FileViewModel>>(arg => _dragDropFileViewModel.DoDragDrop(arg));
        }

        private void LoadGameDirectoriesAsync()
        {
            Task.Run(async () =>
            {
                IList<FromSoftwareFile> loadGameRootDirectories = await FileRepository.LoadGemDirectoriesAsync();
                foreach (FromSoftwareFile fromSoftwareFile in loadGameRootDirectories)
                    await UiDispatcher.BeginInvoke(DispatcherPriority.Normal,
                        new Action(() => { _gameRootsDirectory.Add(new RootDirectoryViewModel(this, fromSoftwareFile)); }));
            });
        }

        public ObservableCollection<RootDirectoryViewModel> GameRootsDirectory
        {
            get => _gameRootsDirectory;
            set
            {
                _gameRootsDirectory = value;
                OnPropertyChanged("GameRootsDirectory");
            }
        }

        public DelegateCommand Edit { get; }

        public DelegateCommand Delete { get; }

        public DelegateAsyncCommand Commit { get; }

        public DelegateCommand Cancel { get; }

        public DelegateCommand New { get; }

        public DelegateAsyncCommand Refresh { get; }

        public DelegateAsyncCommand OpenInExplorer { get; }

        public DelegateAsyncCommand<DragDropInfoViewModel<FileViewModel>> DropCommand { get; }

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
