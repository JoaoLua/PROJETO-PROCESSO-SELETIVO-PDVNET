using System.Windows;

namespace PDVnet.ControleCaixa.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Captura exceções não tratadas na thread principal (UI)
        DispatcherUnhandledException += (sender, args) =>
        {
            MessageBox.Show(
                $"Ocorreu um erro inesperado:\n\n{args.Exception.Message}",
                "Erro - PDVnet Controle de Caixa",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            args.Handled = true; // Impede que a aplicação feche abruptamente
        };
    }
}
