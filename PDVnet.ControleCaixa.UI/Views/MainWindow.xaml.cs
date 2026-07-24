using System.Windows;

namespace PDVnet.ControleCaixa.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new ViewModels.MainViewModel();
    }
}