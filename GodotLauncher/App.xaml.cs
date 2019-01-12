using System.IO;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;
using GodotLauncher.Classes;

namespace GodotLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static EnvironmentSettings environment = new EnvironmentSettings();

        private void App_OnDispatcherUnhandledException( object sender, DispatcherUnhandledExceptionEventArgs e )
        {
            // log the unhandled exception
            logger.Error( e.Exception, "An unhandled exception occurred" );

            // also show a popup informing the user of the exception
            MessageBox.Show( e.Exception.Message, "Unhandled exception",
                MessageBoxButton.OK,
                MessageBoxImage.Error );
            
            // mark it as handled
            e.Handled = true;
        }

        private void App_OnStartup( object sender, StartupEventArgs e )
        {
            // construct the new window object
            MainWindow wnd = new MainWindow();
            
            // info log
            logger.Info( "GodotLauncher started" );
            
            // show the window
            wnd.Show();
        }

        private void App_OnExit( object sender, ExitEventArgs e )
        {
            // info log
            logger.Info( "GodotLauncher exiting" );
            
            // flush the logger
            NLog.LogManager.Shutdown();
        }
    }
}