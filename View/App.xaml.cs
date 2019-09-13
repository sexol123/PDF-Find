using Model;
using System.Windows;
using ViewModel;

using System.Windows.Threading;

namespace View
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Global exception handling  
            Current.DispatcherUnhandledException += AppDispatcherUnhandledException;

            if (e.Args.Length == 0)
            {
                new SettingsWindow().Show();
                return;
            }
            var reportDataModel = new ReportDataModel(new RegistryApplicationConfigurator());
            var path = e.Args[0];
            var report = reportDataModel.FindReport(path);
            if (report != null)
            {
                reportDataModel.OpenForPrint(path, report);
                return;
            }

            var groupConfiguration = reportDataModel.FindGroup(path);
            if (groupConfiguration == null)
            {
                reportDataModel.OpenInReader(path);
            }

            StateReport.Instance.CurrentReportConfiguration = new ReportConfiguration
            {
                Group = groupConfiguration,
                ReportName = ReportDataModel.GetReportName(path)
            };
            new PrinterSettingsWindow().Show();
        }

        public void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

#if DEBUG   // In debug mode do not custom-handle the exception, let Visual Studio handle it

            e.Handled = false;

#else 
            ShowUnhandledException(e);    
            
#endif
        }

        public void ShowUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {

            e.Handled = true;

            var message = e.Exception.InnerException ?? e.Exception;

            var errorMessage = "An \"PDF Find\"-application error occurred.\n" +
            "Please check whether your data is correct and repeat the action. " +
            "If this error occurs again, there seems to be a more serious malfunction in the application, " +
            $"and you better close it.\n\nError: {message.Message}\n\n" +
            "The application will be closed now.";

            MessageBox.Show(errorMessage, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);

            Current.Shutdown();
        }
    }
}
