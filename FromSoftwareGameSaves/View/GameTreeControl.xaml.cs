using System.Windows.Controls;
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

            var viewModel = new GameTreeViewModel();
            DataContext = viewModel;
        }
    }
}
