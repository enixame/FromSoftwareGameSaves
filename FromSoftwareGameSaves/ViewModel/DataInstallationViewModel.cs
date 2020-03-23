using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using FromSoftwareGameSaves.Commands;
using FromSoftwareGameSaves.Utils;
using FromSoftwareStorage;

namespace FromSoftwareGameSaves.ViewModel
{
    public sealed class DataInstallationViewModel : ViewModelBase
    {
        private readonly ViewModelBase _viewModel;
        private readonly string _propertyName;

        private string _password;
        private bool _hasPassword;

        public DataInstallationViewModel(ViewModelBase viewModel, string propertyName)
        {
            _viewModel = viewModel;
            _propertyName = propertyName;

            InstallCommand = new DelegateAsyncCommand(() =>
            {
                return Task.Run(async () => { await DoInstallAndRaiseAsync(); });
            });

            PasswordChangedCommand = new DelegateCommand<PasswordBox>(passwordBox =>
            {
                Password = passwordBox.Password;
            });
        }

        private async Task DoInstallAndRaiseAsync()
        {
            string password = null;
            if (HasPassword && !string.IsNullOrEmpty(Password) && !string.IsNullOrWhiteSpace(Password))
                password = Password;

            try
            {
                bool isInstalled = await Database.DatabaseProvider.InstallAsync(password);
                if (isInstalled)
                {
                    await UiDispatcher.BeginInvoke(DispatcherPriority.Normal,
                        new Action(RaisePropertyChangedToViewModel));
                }
            }
            catch (Exception exception)
            {
                MessageBoxHelper.ShowMessage(exception.Message, 
                    "Failed to install Database", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            
        }

        private void RaisePropertyChangedToViewModel()
        {
            _viewModel.OnPropertyChanged(_propertyName);
        }

        public DelegateAsyncCommand InstallCommand { get; }

        public DelegateCommand<PasswordBox> PasswordChangedCommand { get; }

        public string DataBaseInstallationFolder => Database.DatabaseProvider.DataBaseFileName;

        public bool HasPassword
        {
            get => _hasPassword;
            set
            {
                _hasPassword = value;
                OnPropertyChanged("HasPassword");
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }
    }
}
