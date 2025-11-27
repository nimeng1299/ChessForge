using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ChessForge.ViewModels;
using SukiUI.Controls;
using System.Threading.Tasks;

namespace ChessForge.Views
{
    public partial class MainWindow : SukiWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Closed += (sender, e) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.Close();
                }
            };
        }
    }
}