using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PDVnet.ControleCaixa.UI.Views;
using PDVnet.ControleCaixa.UI.ViewModels;
using ControleCaixa.Business.Services;
using ControleCaixa.Data;
using ControleCaixa.Model.Interfaces;

namespace PDVnet.ControleCaixa.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public App()
    {
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddTransient<ICategoriaRepository, CategoriaRepository>();
        services.AddTransient<IMovimentacaoRepository, MovimentacaoRepository>();

        services.AddTransient<ICategoriaService, CategoriaService>();
        services.AddTransient<IMovimentacaoService, MovimentacaoService>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<MovimentacoesViewModel>();
        services.AddTransient<CategoriasViewModel>();
        services.AddTransient<MovimentacaoFormViewModel>();

        services.AddTransient<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var culture = new System.Globalization.CultureInfo("pt-BR");
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(culture.IetfLanguageTag)));

        DispatcherUnhandledException += (sender, args) =>
        {
            MessageBox.Show(
                $"Ocorreu um erro inesperado:\n\n{args.Exception.Message}",
                "Erro - PDVnet Controle de Caixa",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            args.Handled = true; 
        };

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
