using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using GodotLauncher.DataClasses;
using GodotLauncher.Services;

namespace GodotLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private ApplicationConfig config;
        private GodotVersionService versionService;

        private const string EMPTY_COMBOBOX_PLACEHOLDER = "No Godot versions found.";

        public MainWindow(ApplicationConfig config, GodotVersionService versionService)
        {
            InitializeComponent();

            this.config = config;
            this.versionService = versionService;

            FillComboboxWithData();
        }

        private void FillComboboxWithData()
        {
            InstalledVersionsCB.Items.Clear();

            if (versionService.InstalledVersions == null || versionService.InstalledVersions.Count == 0)
            {
                InstalledVersionsCB.IsEnabled = false;
                StartButton.IsEnabled = false;
                InstalledVersionsCB.Foreground = Brushes.DarkGray;

                InstalledVersionsCB.Items.Add(new KeyValuePair<int, string>(-1, EMPTY_COMBOBOX_PLACEHOLDER));
                InstalledVersionsCB.SelectedIndex = 0;

                StartButton.Content = FindResource("StartIconInactive");
            }
            else
            {
                InstalledVersionsCB.IsEnabled = true;
                StartButton.IsEnabled = true;
                InstalledVersionsCB.Foreground = Brushes.Black;

                StartButton.Content = FindResource("StartIcon");

                foreach(var version in versionService.InstalledVersions)
                {
                    string versionName = $"Godot {version.VersionName} x{version.BitNum}";

                    if(version.IsMono)
                        versionName += " Mono";

                    InstalledVersionsCB.Items.Add(new KeyValuePair<int, string>(version.VersionId, versionName));
                }

                InstalledVersionsCB.SelectedValue = config.LastSelectedVersion == -1 ? 0 : config.LastSelectedVersion;

                if(InstalledVersionsCB.SelectedIndex == -1)
                    InstalledVersionsCB.SelectedIndex = 0;
            }
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ConfigureWindow(config, versionService)
            {
                Owner = this,
            };

            window.ShowDialog();

            FillComboboxWithData();
        }

        private void DownloadsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new DownloadsWindow(config, versionService)
            {
                Owner = this,
            };

            window.ShowDialog();

            FillComboboxWithData();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int selId = (int)InstalledVersionsCB.SelectedValue;
            config.LastSelectedVersion = selId;

            var selectedVersion = versionService.InstalledVersions.FirstOrDefault( x=> x.VersionId == selId);

            Process.Start(selectedVersion.InstallPath);

            if(config.OnGodotLaunch == Constants.CLOSE_ON_LAUNCH)
                Close();
            else if(config.OnGodotLaunch == Constants.MINIMIZE_ON_LAUNCH)
                WindowState = WindowState.Minimized;

            JsonConverterService<ApplicationConfig>.Serialize(config, "config\\config.json");
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AboutWindow()
            {
                Owner = this
            };

            window.ShowDialog();
        }
    }
}