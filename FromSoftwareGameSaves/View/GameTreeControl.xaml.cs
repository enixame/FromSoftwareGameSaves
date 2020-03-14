using System.Windows.Controls;
using FromSoftwareGameSaves.Repository;
using FromSoftwareGameSaves.ViewModel;

namespace FromSoftwareGameSaves.View
{
    /// <summary>
    /// Interaction logic for GameTreeControl.xaml
    /// </summary>
    public partial class GameTreeControl : UserControl
    {
        public GameTreeControl()
        {
            InitializeComponent();

            var roots = FileRepository.LoadRootFiles();
            var viewModel = new GameTreeViewModel(roots);
            DataContext = viewModel;
        }
    }
}
