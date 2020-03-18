using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FromSoftwareFileManager;
using FromSoftwareGameSaves.Commands;
using FromSoftwareGameSaves.Model;
using FromSoftwareGameSaves.Repository;
using FromSoftwareGameSaves.Utils;

namespace FromSoftwareGameSaves.ViewModel
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary> 
    public class TreeViewItemViewModel : ViewModelBase, ITreeViewItemViewModel
    {
        #region Data

        protected static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

        protected bool IsExpandedField;
        private bool _isSelected;
        private bool _isInEditMode;

        #endregion // Data

        #region Constructors

        protected TreeViewItemViewModel(ViewModelBase root, ITreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            Root = root;
            Parent = parent;

            Children = new ObservableCollection<ITreeViewItemViewModel>();

            if (lazyLoadChildren)
                Children.Add(DummyChild);
        }

        // This is used to create the DummyChild instance.
        private TreeViewItemViewModel()
        {
        }

        #endregion // Constructors

        #region Presentation Members

        #region Children

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<ITreeViewItemViewModel> Children { get; }

        #endregion // Children

        #region HasLoadedChildren

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild => Children.Count == 1 && Children[0] == DummyChild;

        #endregion // HasLoadedChildren

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => IsExpandedField;
            set
            {
                if (value != IsExpandedField)
                {
                    IsExpandedField = value;
                    OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (IsExpandedField && Parent != null)
                    Parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (!HasDummyChild)
                    return;

                Children.Remove(DummyChild);

                LoadChildrenAsync().ConfigureAwait(false);
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected

        public virtual bool? IsDirectory { get; } = null;

        public FromSoftwareFile FromSoftwareFile { get; protected set; }

        public string RootPath => string.IsNullOrEmpty(FromSoftwareFile?.Path) ? string.Empty : FromSoftwareFile.Path.Split(Path.DirectorySeparatorChar).First();

        #region IsInEditMode

        public bool IsInEditMode
        {
            get => _isInEditMode;
            set
            {
                _isInEditMode = value;
                OnPropertyChanged("IsInEditMode");
            }
        }

        #endregion // IsInEditMode

        public bool CanBeEdited { get; protected set; }

        #region LoadChildrenAsync

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// </summary>
        protected async Task LoadChildrenAsync()
        {
            if (!FromSoftwareFile.IsDirectory)
                return;

            foreach (var child in await FileRepository.LoadChildrenAsync(FromSoftwareFile))
            {
                await UiDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Children.Add(new FileViewModel(Root, child, this));
                }));
            }
        }

        #endregion // LoadChildrenAsync

        #region Parent

        public ITreeViewItemViewModel Parent { get; }

        #endregion // Parent

        #region Root

        public ViewModelBase Root { get; }

        #endregion // Root

        #endregion // Presentation Members

        #region actions

        public virtual Task CommitAsync()
        {
            // need to be implemented
            return Task.CompletedTask;
        }

        public virtual void Cancel()
        {
            // need to be implemented
        }

        public virtual void Delete()
        {
            // need to be implemented
        }

        public virtual void New()
        {
            const string newDirectory = "New Directory";
            var filePath = Path.Combine(FromSoftwareFile.Path, FromSoftwareFile.FileName);

            try
            {
                var isDirectoryCreated = FileSystem.CreateDirectory(Path.Combine(FromSoftwareFile.RootDirectory, filePath, newDirectory));
                if (!isDirectoryCreated) return;

                CreateNewFileAndExpandIt(FromSoftwareFile, newDirectory, filePath, true);
            }
            catch (Exception exception)
            {
                MessageBoxHelper.ShowMessage(exception.Message, "Cannot create folder", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateNewFileAndExpandIt(GameFile gameFile, string fileName, string filePath, bool isInEditMode)
        {
            var isDirectoryLoaded = !HasDummyChild;

            if (isDirectoryLoaded)
            {
                var file = new FromSoftwareFile(gameFile.RootDirectory, gameFile.FileSearchPattern,  fileName, true, filePath);
                var treeViewItemViewModel = new FileViewModel(Root, file, this) {IsSelected = true, IsInEditMode = isInEditMode };
                Children.Add(treeViewItemViewModel);

                treeViewItemViewModel.ExpandAsync().ConfigureAwait(false);
            }
        }

        protected async Task ExpandAsync()
        {
            IsExpandedField = true;
            OnPropertyChanged("IsExpanded");

            await UiDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { Children.Remove(DummyChild); }));
            await LoadChildrenAsync();
        }

        public async Task RefreshAsync()
        {
            if (HasDummyChild)
                return;

            var wasExpanded = IsExpandedField;
            IsExpandedField = false;
            OnPropertyChanged("IsExpanded");

            await UiDispatcher.BeginInvoke(new Action(() => { Children.Clear(); }));
            await LoadChildrenAsync();

            if (wasExpanded)
            {
                IsExpandedField = true;
                OnPropertyChanged("IsExpanded");
            }
        }

        #endregion // actions
    }
}
