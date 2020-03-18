using System.Threading.Tasks;
using System.Windows;
using FromSoftwareGameSaves.Utils;

namespace FromSoftwareGameSaves.ViewModel
{
    public sealed class DragDropFileViewModel
    {
        public async Task DoDragDrop(DragDropInfoViewModel<FileViewModel> dragDropInfoViewModel)
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
                var newFileViewModel = await dragDropInfoViewModel.TargetItem.AcceptCopyFromTreeViewItemAsync(dragDropInfoViewModel.SourceItem);
                if(newFileViewModel!= null)
                    await newFileViewModel.ExpandAllAsync();
            }
            else
            {
                if (dragDropInfoViewModel.TargetItem.Parent is FileViewModel treeViewItemViewModel && treeViewItemViewModel.IsDirectory == true)
                {
                    var newFileViewModel = await treeViewItemViewModel.AcceptCopyFromTreeViewItemAsync(dragDropInfoViewModel.SourceItem);
                    if (newFileViewModel != null)
                        await newFileViewModel.ExpandAllAsync();
                }
            }
        }       
    }
}
