using GodotLauncher.Classes;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace GodotLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private GodotVersionStorage versionStorage = new GodotVersionStorage();

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // log the unhandled exception
            logger.Error(e.Exception, "An unhandled exception occurred");

            // also show a popup informing the user of the exception
            CommonUtils.PopupExceptionMessage("Unhandled exception", e.Exception);

            // mark it as handled
            e.Handled = true;
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            logger.Info("GodotLauncher started");

            if (!IsCurrentDirectoryWriteable())
                Current.Shutdown();

            // construct the new window object
            MainWindow wnd = new MainWindow(versionStorage);

            // info log

            // show the window
            wnd.Show();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            // info log
            logger.Info("GodotLauncher exiting");

            // flush the logger
            NLog.LogManager.Shutdown();
        }

        /// <summary>
        /// Tries to write a temporary file in the application's directo
        /// </summary>
        private bool IsCurrentDirectoryWriteable()
        {
            try
            {
                using (var file = File.Create("tmp"))
                { }

                if (File.Exists("tmp"))
                    File.Delete("tmp");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                CommonUtils.PopupExceptionMessage("Write check error", ex);

                return false;
            }
            return true;
        }
    }
}