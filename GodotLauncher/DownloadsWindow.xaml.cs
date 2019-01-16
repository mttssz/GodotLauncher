using GodotLauncher.DataClasses;
using GodotLauncher.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GodotLauncher
{
    /// <summary>
    /// Interaction logic for DownloadsWindow.xaml
    /// </summary>
    public partial class DownloadsWindow : Window
    {
        private ApplicationConfig config;
        private GodotVersionService versionService;

        public DownloadsWindow(ApplicationConfig config, GodotVersionService versionService)
        {
            InitializeComponent();

            this.config = config;
            this.versionService = versionService;

            BuildVersionsTree();
            FillCheckboxes();
        }

        private void BuildVersionsTree()
        {
            GodotVersionsTree.Items.Clear();

            GodotVersionsTree.SelectedValuePath = "Key";
            GodotVersionsTree.DisplayMemberPath = "Value";

            var versionNames = versionService.AllVersions.Select(x => x.VersionName).Distinct();

            foreach (var name in versionNames)
            {
                var rootNode = new TreeViewItem { Header = name };
                var versions = versionService.AllVersions.Where(x => x.VersionName == name).OrderBy(x => x.BitNum);
                int count = 0;

                rootNode.DisplayMemberPath = "Value";

                foreach (var v in versions)
                {
                    string vName = $"Godot {v.VersionName} x{v.BitNum}";

                    if (!config.Show32BitVersions && v.BitNum == 32)
                        continue;
                    if (!config.Show64BitVersions && v.BitNum == 64)
                        continue;
                    if (!config.ShowMonoVersions && v.IsMono)
                        continue;
                    if (!config.ShowUnstableVersions && !v.IsStable)
                        continue;

                    if (v.IsMono)
                        vName += " Mono";

                    if (CheckIfVersionIsInstalled(v.VersionId))
                        vName += " ✓";

                    var temp = new TreeViewItem();

                    int index = rootNode.Items.Add(new KeyValuePair<int, string>(v.VersionId, vName));

                    count++;
                }

                if (count != 0)
                    GodotVersionsTree.Items.Add(rootNode);
            }
        }

        private void FillCheckboxes()
        {
            X32Checkbox.IsChecked = config.Show32BitVersions;
            X64Checkbox.IsChecked = config.Show64BitVersions;
            MonoCheckbox.IsChecked = config.ShowMonoVersions;
            UnstableCheckbox.IsChecked = config.ShowUnstableVersions;
        }

        private void GodotVersionsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
                return;

            if (e.NewValue.GetType() != typeof(KeyValuePair<int, string>))
            {
                UninstallButton.IsEnabled = OpenFolderButon.IsEnabled = InstallButton.IsEnabled = false;

                return;
            }
            var selVal = (KeyValuePair<int, string>)e.NewValue;
            var selectedVersion = versionService.AllVersions.FirstOrDefault(x => x.VersionId == selVal.Key);
            bool isInstalled = CheckIfVersionIsInstalled(selectedVersion.VersionId);

            UninstallButton.IsEnabled = isInstalled;
            OpenFolderButon.IsEnabled = isInstalled;
            InstallButton.IsEnabled = !isInstalled;
        }

        private bool CheckIfVersionIsInstalled(int id)
        {
            if (!versionService.InstalledVersions.Exists(x => x.VersionId == id))
                return false;

            return true;
        }

        private void Checkbox_ValueChanged(object sender, RoutedEventArgs e)
        {
            config.Show32BitVersions = X32Checkbox.IsChecked.Value;
            config.Show64BitVersions = X64Checkbox.IsChecked.Value;
            config.ShowMonoVersions = MonoCheckbox.IsChecked.Value;
            config.ShowUnstableVersions = UnstableCheckbox.IsChecked.Value;

            BuildVersionsTree();

            UninstallButton.IsEnabled = false;
            OpenFolderButon.IsEnabled = false;
            InstallButton.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            JsonConverterService<ApplicationConfig>.Serialize(config, "config\\config.json");
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            if (config.GodotInstallLocation == String.Empty)
            {
                CommonUtilsService.PopupWarningMessage("No install location", "Please specify a Godot install location");
                return;
            }

            BusyIndicator.IsBusy = true;

            var temp = (KeyValuePair<int, string>)GodotVersionsTree.SelectedItem;

            var selectedVersion = versionService.AllVersions.FirstOrDefault(x => x.VersionId == temp.Key);

            var worker = new BackgroundWorker();

            worker.DoWork += (s, ev) => DownloadAndExtractVersion(selectedVersion);
            worker.RunWorkerCompleted += (s, ev) => BusyIndicator.IsBusy = false;
            worker.RunWorkerAsync();
        }

        private void DownloadAndExtractVersion(GodotVersion selectedVersion)
        {
            string fileName = DownloadManagerService.DownloadFileSync(selectedVersion.VersionUrl, "temp", true);

            var extractedFiles = ZipService.UnzipFile(fileName, $"{config.GodotInstallLocation}\\{selectedVersion.VersionName}");

            File.Delete(fileName);

            foreach (var file in extractedFiles)
            {
                versionService.InstalledVersions.Add(new GodotVersionInstalled
                {
                    VersionId = selectedVersion.VersionId,
                    VersionName = selectedVersion.VersionName,
                    BitNum = selectedVersion.BitNum,
                    IsMono = selectedVersion.IsMono,
                    IsStable = selectedVersion.IsStable,
                    InstallPath = file
                });
            }

            JsonConverterService<List<GodotVersionInstalled>>.Serialize(versionService.InstalledVersions, $"{config.GodotInstallLocation}\\manifest.json");

            Dispatcher.Invoke(new Action(() => BuildVersionsTree()));

            Dispatcher.Invoke(new Action(() => CommonUtilsService.PopupInfoMessage("Info", "Godot version installed successfully!")));
        }

        private void OpenFolderButon_Click(object sender, RoutedEventArgs e)
        {
            var temp = (KeyValuePair<int, string>)GodotVersionsTree.SelectedItem;
            var selectedVersion = versionService.AllVersions.FirstOrDefault(x => x.VersionId == temp.Key);
            var installedVersion = versionService.InstalledVersions.FirstOrDefault(x => x.VersionId == selectedVersion.VersionId);

            string installedPath = Path.GetDirectoryName(installedVersion.InstallPath);

            Process.Start(installedPath);
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            var temp = (KeyValuePair<int, string>)GodotVersionsTree.SelectedItem;
            var selectedVersion = versionService.AllVersions.FirstOrDefault(x => x.VersionId == temp.Key);
            var installedVersion = versionService.InstalledVersions.FirstOrDefault(x => x.VersionId == selectedVersion.VersionId);

            if(File.Exists(installedVersion.InstallPath))
                File.Delete(installedVersion.InstallPath);

            if(CommonUtilsService.IsDirectoryEmpty(Path.GetDirectoryName(installedVersion.InstallPath)))
                Directory.Delete(Path.GetDirectoryName(installedVersion.InstallPath));

            versionService.InstalledVersions.Remove(installedVersion);

            JsonConverterService<List<GodotVersionInstalled>>.Serialize(versionService.InstalledVersions, $"{config.GodotInstallLocation}\\manifest.json");

            BuildVersionsTree();

            CommonUtilsService.PopupInfoMessage("Info", "Godot version uninstalled successfully!");
        }
    }
}
