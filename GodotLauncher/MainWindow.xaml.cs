using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using GodotLauncher.Classes;
using System.Windows.Media;

namespace GodotLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly GodotVersionStorage versionStorage;
        private readonly ApplicationConfig appConfig;

        public MainWindow(GodotVersionStorage versionStorage, ApplicationConfig appConfig)
        {
            InitializeComponent();
            this.versionStorage = versionStorage;
            this.appConfig = appConfig;

            FillVersionComboBox();
        }

        private void FillVersionComboBox()
        {
            GodotVersionsCombobox.Items.Clear();

            GodotVersionsCombobox.DisplayMemberPath = "Value";
            GodotVersionsCombobox.SelectedValuePath = "Key";

            bool lastSelectedVisible = true;

            foreach(var version in versionStorage.AllVersions)
            {
                if (!appConfig.Show32BitVersions && version.BitNum == 32)
                {
                    if (appConfig.LastUsedVersion == version.VersionId)
                        lastSelectedVisible = false;
                    continue;
                }

                if (!appConfig.Show64BitVersions && version.BitNum == 64)
                {
                    if (appConfig.LastUsedVersion == version.VersionId)
                        lastSelectedVisible = false;
                    continue;
                }

                if (!appConfig.ShowMonoVersions && version.IsMono)
                {
                    if (appConfig.LastUsedVersion == version.VersionId)
                        lastSelectedVisible = false;
                    continue;
                }

                if (!appConfig.ShowUnstableVersions && !version.IsStable)
                {
                    if (appConfig.LastUsedVersion == version.VersionId)
                        lastSelectedVisible = false;
                    continue;
                }

                string versionName = $"{version.VersionName} - x{version.BitNum}";

                if (version.IsMono)
                    versionName += " - Mono";

                bool isInstalled = versionStorage.InstalledVersions.ContainsKey(version.VersionId);

                if (isInstalled)
                {
                    versionName += " - Installed";
                }
                else
                {
                    versionName += " - Not installed";
                }

                var item = new ComboBoxItem();
                item.Foreground = isInstalled ? Brushes.Black : Brushes.SlateGray;
                item.Content = new KeyValuePair<int, string>(version.VersionId, versionName);

                GodotVersionsCombobox.Items.Insert(0, new KeyValuePair<int, string>(version.VersionId, versionName));
                
            }

            if (lastSelectedVisible)
                GodotVersionsCombobox.SelectedValue = appConfig.LastUsedVersion;
            else
                GodotVersionsCombobox.SelectedIndex = -1;
        }

        private void Start_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            appConfig.LastUsedVersion = (int)GodotVersionsCombobox.SelectedValue;
        }

        private void Configure_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var cfgWindow = new ConfigureWindow(appConfig);
            cfgWindow.Owner = this;

            string oldPath = appConfig.GodotInstallPath;

            cfgWindow.ShowDialog();

            if(oldPath != appConfig.GodotInstallPath)
            {
                versionStorage.ReloadInstalledVersions($"{appConfig.GodotInstallPath}\\{Constants.MANIFEST_FILE}");
            }

            if (cfgWindow.ReloadNeeded)
            {
                FillVersionComboBox();
            }
        }
    }
}