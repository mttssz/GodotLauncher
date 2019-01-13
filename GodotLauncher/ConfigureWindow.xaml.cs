using GodotLauncher.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GodotLauncher
{
    /// <summary>
    /// Interaction logic for ConfigureWindow.xaml
    /// </summary>
    /// 
    public partial class ConfigureWindow : Window
    {
        #region Close button hide stuff
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]

        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
        #endregion

        public ApplicationConfig appConfig;
        public bool ReloadNeeded { get; set; }

        public ConfigureWindow(ApplicationConfig appConfig)
        {
            InitializeComponent();

            this.appConfig = appConfig;

            FillInstallLocation();
            FillUpdateCombobox();
            FillCheckBoxes();
        }

        private void FillCheckBoxes()
        {
            MonoCB.IsChecked = appConfig.ShowMonoVersions;
            X32CB.IsChecked = appConfig.Show32BitVersions;
            X64CB.IsChecked = appConfig.Show64BitVersions;
            UnstableCB.IsChecked = appConfig.ShowUnstableVersions;
        }

        private void FillInstallLocation()
        {
            GodotInstallLocationTextbox.Text = appConfig.GodotInstallPath == String.Empty ? "No install path specified" : appConfig.GodotInstallPath;
        }

        private void FillUpdateCombobox()
        {
            UpdateIntervalCombobox.SelectedValuePath = "Key";
            UpdateIntervalCombobox.DisplayMemberPath = "Value";

            UpdateIntervalCombobox.Items.Add(new KeyValuePair<int, string>(0, "Never"));
            UpdateIntervalCombobox.Items.Add(new KeyValuePair<int, string>(30, "After 30 minutes"));
            UpdateIntervalCombobox.Items.Add(new KeyValuePair<int, string>(60, "After an hour"));
            UpdateIntervalCombobox.Items.Add(new KeyValuePair<int, string>(180, "After 3 hours"));
            UpdateIntervalCombobox.Items.Add(new KeyValuePair<int, string>(360, "After 6 hours"));
            UpdateIntervalCombobox.Items.Add(new KeyValuePair<int, string>(720, "After 12 hours"));
            UpdateIntervalCombobox.Items.Add(new KeyValuePair<int, string>(1440, "After a day"));
            UpdateIntervalCombobox.Items.Add(new KeyValuePair<int, string>(10080, "After a week"));

            UpdateIntervalCombobox.SelectedValue = appConfig.UpdateCheckInterval;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            string path = GodotInstallLocationTextbox.Text;

            if (!CommonUtils.IsValidPath(path))
            {
                CommonUtils.PopupWarningMessage("Invalid install location", "Please specify a valid install location");
                return;
            }

            appConfig.GodotInstallPath = path;

            appConfig.UpdateCheckInterval = (int)UpdateIntervalCombobox.SelectedValue;

            appConfig.ShowMonoVersions = MonoCB.IsChecked.Value;
            appConfig.Show32BitVersions = X32CB.IsChecked.Value;
            appConfig.Show64BitVersions = X64CB.IsChecked.Value;
            appConfig.ShowUnstableVersions = UnstableCB.IsChecked.Value;

            JsonConverter<ApplicationConfig>.Serialize(appConfig, Constants.CONFIG_FILE);

            this.Close();
        }

        private void ChooseInstallLocationButton_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.ShowNewFolderButton = true;
                fbd.RootFolder = Environment.SpecialFolder.MyComputer;

                var result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    GodotInstallLocationTextbox.Text = fbd.SelectedPath;
                    string manifestPath = $"{fbd.SelectedPath}\\{Constants.MANIFEST_FILE}";

                    if (!File.Exists(manifestPath))
                    {
                        using (var file = File.CreateText(manifestPath))
                        {
                            file.Write("{}");
                        }
                    }
                }
            }
        }

        private void CheckForUpdatesButton_Click(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();

            string versionsFileUrl = FindResource("VersionsFileUrl").ToString();
            string tempFilePath = $"{Constants.TEMP_DIRECTORY}\\versions_{rnd.Next(1000, 9999)}.json";
            bool updateHappened = false;

            if (!File.Exists(Constants.VERSIONS_FILE))
            {
                DownloadManager.DownloadFileSync(versionsFileUrl, Constants.VERSIONS_FILE);

                updateHappened = true;
            }
            else
            {
                DownloadManager.DownloadFileSync(versionsFileUrl, tempFilePath);

                if (!CommonUtils.AreFilesIdentical(Constants.VERSIONS_FILE, tempFilePath))
                {
                    File.Copy(tempFilePath, Constants.VERSIONS_FILE, true);

                    updateHappened = true;
                }

                File.Delete(tempFilePath);
            }

            CommonUtils.PopupInfoMessage("Update", updateHappened ? "The update was completed successfully." : "No update was neccessary, you are up-to-date.");

            appConfig.LastUpdateChecked = DateTime.Now;
        }

        private void ComboBox_Click(object sender, RoutedEventArgs e)
        {
            this.ReloadNeeded = true;
        }
    }
}
