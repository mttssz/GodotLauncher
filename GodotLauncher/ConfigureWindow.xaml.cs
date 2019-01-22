using GodotLauncher.DataClasses;
using GodotLauncher.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GodotLauncher.ViewModels;

// TODO: add option to use proxies for downloads

namespace GodotLauncher
{
    /// <summary>
    /// Interaction logic for ConfigureWindow.xaml
    /// </summary>
    public partial class ConfigureWindow : Window
    {
        private ApplicationConfig config;
        private GodotVersionService versionService;

        private ConfigureViewModel viewModel = new ConfigureViewModel();

        public ConfigureWindow(ApplicationConfig config, GodotVersionService versionService)
        {
            InitializeComponent();

            this.config = config;
            this.versionService = versionService;

            DataContext = viewModel;

            //FillTextbox();
            //FillComboBox();
            //FillProxySettings();
        }

        private void FillTextbox()
        {
            if (config.GodotInstallLocation == String.Empty)
                GodotInstallLocationTextbox.Text = "Please specify a location";
            else
                GodotInstallLocationTextbox.Text = config.GodotInstallLocation;
        }

        private void FillComboBox()
        {
            OnGodotLaunchComboBox.SelectedValuePath = "Key";
            OnGodotLaunchComboBox.DisplayMemberPath = "Value";

            OnGodotLaunchComboBox.Items.Add(new KeyValuePair<int, string>(Constants.CLOSE_ON_LAUNCH, "Close GodotLauncher"));
            OnGodotLaunchComboBox.Items.Add(new KeyValuePair<int, string>(Constants.MINIMIZE_ON_LAUNCH, "Minimize GodotLauncher"));
            OnGodotLaunchComboBox.Items.Add(new KeyValuePair<int, string>(Constants.DO_NOTHING_ON_LAUNCH, "Do nothing"));

            OnGodotLaunchComboBox.SelectedValue = config.OnGodotLaunch;
        }

        private void FillProxySettings()
        {
            UseProxyCheckBox.IsChecked = config.UseProxy;

            ProxyPortTextBox.Text = config.ProxyPort.ToString();
            ProxyUrlTextBox.Text = config.ProxyUrl;

            ProxyPortTextBox.IsEnabled = UseProxyCheckBox.IsChecked.Value;
            ProxyUrlTextBox.IsEnabled = UseProxyCheckBox.IsChecked.Value;

            ProxyPortLabel.Foreground = UseProxyCheckBox.IsChecked.Value ? Brushes.Black : Brushes.DarkGray;
            ProxyUrlLabel.Foreground = UseProxyCheckBox.IsChecked.Value ? Brushes.Black : Brushes.DarkGray;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            config.GodotInstallLocation = GodotInstallLocationTextbox.Text;
            config.OnGodotLaunch = (int)OnGodotLaunchComboBox.SelectedValue;
            bool proxy = UseProxyCheckBox.IsChecked.Value;

            if (proxy)
            {
                if (ProxyPortTextBox.Text == String.Empty || ProxyUrlTextBox.Text == String.Empty)
                {
                    CommonUtilsService.PopupWarningMessage("Invalid values", "Please input both an URL and a port number for the proxy.");
                    return;
                }

                if (Uri.TryCreate(ProxyUrlTextBox.Text, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    config.ProxyUrl = ProxyUrlTextBox.Text;
                else
                {
                    CommonUtilsService.PopupWarningMessage("Invalid values", "Please input a valid URL for the proxy.");
                    return;
                }

                if (Int32.TryParse(ProxyPortTextBox.Text, out int portNum))
                    config.ProxyPort = portNum;
                else
                {
                    CommonUtilsService.PopupWarningMessage("Invalid values", "Please input a valid port for the proxy.");
                    return;
                }
            }

            config.UseProxy = proxy;
            JsonConverterService<ApplicationConfig>.Serialize(config, "config\\config.json");

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var ofd = new FolderBrowserDialog())
            {
                ofd.RootFolder = Environment.SpecialFolder.MyComputer;
                ofd.ShowNewFolderButton = true;

                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                string path = ofd.SelectedPath;
                string manifestFile = $"{path}\\manifest.json";

                if (!File.Exists(manifestFile))
                {
                    using (var file = File.CreateText(manifestFile))
                    {
                        file.Write("[]");
                    }
                }

                versionService.InstalledVersions = JsonConverterService<List<GodotVersionInstalled>>.Deserialize(manifestFile);

                GodotInstallLocationTextbox.Text = path;
            }
        }

        private void UseProxyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            config.UseProxy = true;

            ProxyUrlTextBox.IsEnabled = true;
            ProxyPortTextBox.IsEnabled = true;

            ProxyUrlLabel.Foreground = Brushes.Black;
            ProxyPortLabel.Foreground = Brushes.Black;
        }

        private void UseProxyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            config.UseProxy = false;

            ProxyUrlTextBox.IsEnabled = false;
            ProxyPortTextBox.IsEnabled = false;

            ProxyUrlLabel.Foreground = Brushes.DarkGray;
            ProxyPortLabel.Foreground = Brushes.DarkGray;
        }
    }
}
