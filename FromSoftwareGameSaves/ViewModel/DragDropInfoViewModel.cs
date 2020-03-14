namespace FromSoftwareGameSaves.ViewModel
{
    public sealed class DragDropInfoViewModel<TDataItem>
        where TDataItem : class
    {
        public TDataItem SourceItem { get;}
        public TDataItem TargetItem { get; }

        public DragDropInfoViewModel(TDataItem sourceItem, TDataItem targetItem)
        {
            SourceItem = sourceItem;
            TargetItem = targetItem;
        }
    }
}
