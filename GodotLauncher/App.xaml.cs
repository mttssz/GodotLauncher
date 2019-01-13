using GodotLauncher.Classes;
using System;
using System.Collections.Generic;
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
        private ApplicationConfig appConfig;

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // log the unhandled exception
            logger.Error(e.Exception, "An unhandled exception occurred");

            // also show a popup informing the user of the exception
            CommonUtils.PopupExceptionMessage("Unhandled exception", e.Exception);

            // mark it as handled
            //e.Handled = true;
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            logger.Info("GodotLauncher started");

            string versionsFileUrl = FindResource("VersionsFileUrl").ToString();

            if (!IsCurrentDirectoryWriteable())
                Current.Shutdown();

            SetupEnvironmentDirectories();

            OpenOrCreateConfigFile();

            if (!TryDownloadOrUpdateVersionsFile(versionsFileUrl))
                Current.Shutdown();

            versionStorage.AllVersions = JsonConverter<List<GodotVersion>>.Deserialize(Constants.VERSIONS_FILE);

            if (appConfig.GodotInstallPath != String.Empty)
                versionStorage.ReloadInstalledVersions($"{appConfig.GodotInstallPath}\\{Constants.MANIFEST_FILE}");


            // construct the new window object
            MainWindow wnd = new MainWindow(versionStorage, appConfig);

            // info log
            // show the window
            wnd.Show();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            JsonConverter<ApplicationConfig>.Serialize(appConfig, Constants.CONFIG_FILE);

            // info log
            logger.Info("GodotLauncher exiting");

            // flush the logger
            NLog.LogManager.Shutdown();
        }

        /// <summary>
        /// Tries to write a temporary file in the application's directory
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

        /// <summary>
        /// Sets up the enviroment directories for the app
        /// </summary>
        private void SetupEnvironmentDirectories()
        {
            if (!Directory.Exists(Constants.TEMP_DIRECTORY))
                Directory.CreateDirectory(Constants.TEMP_DIRECTORY);

            if (!Directory.Exists(Constants.CONFIG_DIRECTORY))
                Directory.CreateDirectory(Constants.CONFIG_DIRECTORY);
        }

        private bool TryDownloadOrUpdateVersionsFile(string url)
        {
            int minutesSinceLastUpdateCheck = (int)(DateTime.Now - appConfig.LastUpdateChecked).TotalMinutes;

            if (!File.Exists(Constants.CONFIG_FILE) || (appConfig.UpdateCheckInterval != 0 && minutesSinceLastUpdateCheck >= appConfig.UpdateCheckInterval))
            {
                try
                {
                    DownloadManager.DownloadFileSync(url, Constants.VERSIONS_FILE);
                }
                catch(Exception ex)
                {
                    logger.Error(ex);
                    CommonUtils.PopupExceptionMessage("Failed to download versions file", ex);

                    return false;
                }

                appConfig.LastUpdateChecked = DateTime.Now;
            }

            return true;
        }


        private void OpenOrCreateConfigFile()
        {
            if (File.Exists(Constants.CONFIG_FILE))
            {
                appConfig = JsonConverter<ApplicationConfig>.Deserialize(Constants.CONFIG_FILE);
            }
            else
            {
                appConfig = new ApplicationConfig
                {
                    UpdateCheckInterval = 0,
                    GodotInstallPath = String.Empty,
                    LastUpdateChecked = DateTime.Now,
                };

                JsonConverter<ApplicationConfig>.Serialize(appConfig, Constants.CONFIG_FILE);
            }
        }
    }
}