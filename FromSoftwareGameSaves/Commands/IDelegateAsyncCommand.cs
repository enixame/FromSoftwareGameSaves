using System.Threading.Tasks;
using System.Windows.Input;

namespace FromSoftwareGameSaves.Commands
{
    public interface IDelegateAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }

    public interface IDelegateAsyncCommand<in T> : ICommand
    {
        Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
    }
}