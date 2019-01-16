using GodotLauncher.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Threading;

using GodotLauncher.DataClasses;

namespace GodotLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static GodotVersionService versionService = new GodotVersionService();
        private static ApplicationConfig config;

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // log the unhandled exception
            logger.Error(e.Exception, "An unhandled exception occurred");

            // also show a popup informing the user of the exception
            CommonUtilsService.PopupExceptionMessage("Unhandled exception", e.Exception);

            // mark it as handled
            //e.Handled = true;
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            logger.Info("GodotLauncher started");

            if (!InitialSetup())
                Shutdown();

            // construct the new window object
            MainWindow wnd = new MainWindow(config, versionService);
            
            // info log
            // show the window
            wnd.Show();
        }

        private bool InitialSetup()
        {
            if (!CheckIfCurrentDirectoryIsWritable())
                return false;

            if (!TryToCreateDirectories())
                return false;

            if (!ReadOrCreateConfigFile())
                return false;

            if (!TryToDownloadOrUpdateVersionsFile())
                return false;

            if (!LoadVersionsFile())
                return false;

            return true;
        }

        private bool CheckIfCurrentDirectoryIsWritable()
        {
            try
            {
                using (var file = File.Create("tmp"))
                { }

                File.Delete("tmp");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                CommonUtilsService.PopupExceptionMessage("I/O error", ex);

                return false;
            }

            return true;
        }

        private bool TryToCreateDirectories()
        {
            try
            {
                if (!Directory.Exists("config"))
                    Directory.CreateDirectory("config");

                if (!Directory.Exists("temp"))
                    Directory.CreateDirectory("temp");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                CommonUtilsService.PopupExceptionMessage("I/O error", ex);

                return false;
            }

            return true;
        }

        private bool ReadOrCreateConfigFile()
        {
            string configFile = "config\\config.json";
            try
            {
                if (!File.Exists(configFile))
                {
                    config = new ApplicationConfig
                    {
                        GodotInstallLocation = String.Empty,
                        Show32BitVersions = true,
                        Show64BitVersions = true,
                        ShowMonoVersions = false,
                        ShowUnstableVersions = false,
                        LastUpdateChecked = DateTime.Now,
                        LastSelectedVersion = -1,
                        OnGodotLaunch = Constants.DO_NOTHING_ON_LAUNCH,
                        UseProxy = false,
                        ProxyUrl = String.Empty,
                        ProxyPort = 0,
                    };

                    JsonConverterService<ApplicationConfig>.Serialize(config, configFile);
                }

                config = JsonConverterService<ApplicationConfig>.Deserialize(configFile);

                if(config.UseProxy && (config.ProxyPort == 0 || config.ProxyUrl == String.Empty))
                {
                    config.UseProxy = false;

                    JsonConverterService<ApplicationConfig>.Serialize(config, configFile);
                }

                if(config.GodotInstallLocation != String.Empty && !Directory.Exists(config.GodotInstallLocation))
                {
                    Directory.CreateDirectory(config.GodotInstallLocation);

                    using (var file = File.CreateText($"{config.GodotInstallLocation}\\manifest.json"))
                    {
                        file.Write("[]");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                CommonUtilsService.PopupExceptionMessage("Error reading config", ex);

                return false;
            }

            return true;
        }

        private bool TryToDownloadOrUpdateVersionsFile()
        {
            string url;

            if (File.Exists("config\\versions.json") && (DateTime.Now - config.LastUpdateChecked).TotalDays < 1)
                return true;

            try
            {
                url = FindResource("VersionsFileUrl").ToString();

                if(config.UseProxy)
                    DownloadManagerService.DownloadFileSyncWithProxy(url, "config\\versions.json", config.ProxyUrl, config.ProxyPort);
                else
                    DownloadManagerService.DownloadFileSync(url, "config\\versions.json");

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                CommonUtilsService.PopupExceptionMessage("Error while downloading versions", ex);
            }

            config.LastUpdateChecked = DateTime.Now;

            return true;
        }

        private bool LoadVersionsFile()
        {
            try
            {
                versionService.AllVersions = JsonConverterService<List<GodotVersion>>.Deserialize("config\\versions.json");

                string installedManifest = $"{config.GodotInstallLocation}\\manifest.json";
                if (config.GodotInstallLocation != String.Empty && File.Exists(installedManifest))
                    versionService.InstalledVersions = JsonConverterService<List<GodotVersionInstalled>>.Deserialize(installedManifest);
                else
                    versionService.InstalledVersions = new List<GodotVersionInstalled>();
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                CommonUtilsService.PopupExceptionMessage("Error loading versions", ex);

                return false;
            }

            versionService.AllVersions.Reverse();

            return true;
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            // info log
            logger.Info("GodotLauncher exiting");

            // flush the logger
            NLog.LogManager.Shutdown();
        }
    }
}