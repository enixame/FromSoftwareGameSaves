using FromSoftwareStorage;

namespace FromSoftwareGameSaves.ViewModel
{
    public sealed class MainViewModel : ViewModelBase    
    {
        public ViewModelBase SelectedViewModel
        {
            get
            {
                if (Database.DatabaseProvider.IsDatabaseInstalled)
                    return new GameTreeViewModel();

                return new DataInstallationViewModel(this, nameof(SelectedViewModel));
            }
        }

    }
}
