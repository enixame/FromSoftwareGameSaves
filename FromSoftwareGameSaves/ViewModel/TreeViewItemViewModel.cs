using System;
using System.IO;
using System.Linq;
using System.Windows;
using FileSystemManager;
using FromSoftwareGameSaves.Repository;
using FromSoftwareGameSaves.Utils;
using FromSoftwareModel;
using File = FromSoftwareGameSaves.Model.File;

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

        private static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

        private bool _isExpanded;
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
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && Parent != null)
                    Parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (!HasDummyChild) return;

                Children.Remove(DummyChild);
                LoadChildren();
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
            get { return _isSelected; }
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

        public File File { get; protected set; }

        public string RootPath => string.IsNullOrEmpty(File?.Path) ? string.Empty : File.Path.Split(Path.DirectorySeparatorChar).First();

        #region IsInEditMode

        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set
            {
                _isInEditMode = value;
                OnPropertyChanged("IsInEditMode");
            }
        }

        #endregion // IsInEditMode

        public bool CanBeEdited { get; protected set; }

        #region LoadChildren

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// </summary>
        protected void LoadChildren()
        {
            if (!File.IsDirectory)
                return;

            foreach (var child in FileRepository.LoadChildren(File))
                Children.Add(new FileViewModel(Root, child, this));
        }

        #endregion // LoadChildren

        #region Parent

        public ITreeViewItemViewModel Parent { get; }

        #endregion // Parent

        #region Root

        public ViewModelBase Root { get; }

        #endregion // Root

        #endregion // Presentation Members

        #region actions

        public virtual void Commit()
        {
            // need to be implemented
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
            var filePath = Path.Combine(File.Path, File.FileName);

            try
            {
                var isDirectoryCreated = FileSystem.CreateDirectory(Path.Combine(FromSoftwareFileInfo.AppDataPath, filePath, newDirectory));
                if (!isDirectoryCreated) return;

                CreateNewFileAndExpandIt(newDirectory, filePath, true);
            }
            catch (Exception exception)
            {
                MessageBoxHelper.ShowMessage(exception.Message, "Cannot create folder", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateNewFileAndExpandIt(string fileName, string filePath, bool isInEditMode)
        {
            var isDirectoryLoaded = !HasDummyChild;

            if (isDirectoryLoaded)
            {
                var file = new File(fileName, true, filePath);
                var treeViewItemViewModel = new FileViewModel(Root, file, this) {IsSelected = true, IsInEditMode = isInEditMode };
                Children.Add(treeViewItemViewModel);
            }

            IsExpanded = true;

            if (isDirectoryLoaded) return;

            var newItem = Children.FirstOrDefault(child => child.File.FileName.Equals(fileName));
            if (newItem == null) return;

            newItem.IsSelected = true;
            newItem.IsInEditMode = isInEditMode;
        }

        public virtual void Refresh()
        {
            if (HasDummyChild)
                return;

            var wasExpanded = IsExpanded;

            IsExpanded = false;
            Children.Clear();

            LoadChildren();

            if (wasExpanded)
                IsExpanded = true;
        }

        #endregion // actions
    }
}
