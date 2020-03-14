using System.Windows;
using FromSoftwareGameSaves.Utils;

namespace FromSoftwareGameSaves.ViewModel
{
    public sealed class DragDropFileViewModel
    {
        public void DoDragDrop(DragDropInfoViewModel<FileViewModel> dragDropInfoViewModel)
        {
            var targetItemRootPath = dragDropInfoViewModel.TargetItem.RootPath;
            var sourceItemRootPath = dragDropInfoViewModel.SourceItem.RootPath;

            if (targetItemRootPath != sourceItemRootPath)
            {
                MessageBoxHelper.ShowMessage($"Cannot copy from {sourceItemRootPath} to {targetItemRootPath} !", "Action not allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dragDropInfoViewModel.TargetItem.IsDirectory == true)
            {
                var fileViewModel = dragDropInfoViewModel.TargetItem.AcceptCopy(dragDropInfoViewModel.SourceItem);
                fileViewModel?.ExpandAll();
            }
            else
            {
                var treeViewItemViewModel = dragDropInfoViewModel.TargetItem.Parent as FileViewModel;
                if (treeViewItemViewModel != null && treeViewItemViewModel.IsDirectory == true)
                {
                    var fileViewModel = treeViewItemViewModel.AcceptCopy(dragDropInfoViewModel.SourceItem);
                    fileViewModel?.ExpandAll();
                }
            }
        }       
    }
}
