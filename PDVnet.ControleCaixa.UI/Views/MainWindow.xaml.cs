using System.Windows;

using PDVnet.ControleCaixa.UI.ViewModels;

namespace PDVnet.ControleCaixa.UI;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}
