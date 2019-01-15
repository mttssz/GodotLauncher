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

namespace GodotLauncher
{
    /// <summary>
    /// Interaction logic for ConfigureWindow.xaml
    /// </summary>
    public partial class ConfigureWindow : Window
    {
        private ApplicationConfig config;
        private GodotVersionService versionService;

        public ConfigureWindow(ApplicationConfig config, GodotVersionService versionService)
        {
            InitializeComponent();

            this.config = config;
            this.versionService = versionService;

            FillTextbox();
        }

        private void FillTextbox()
        {
            if (config.GodotInstallLocation == String.Empty)
                GodotInstallLocationTextbox.Text = "Please specify a location";
            else
                GodotInstallLocationTextbox.Text = config.GodotInstallLocation;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            config.GodotInstallLocation = GodotInstallLocationTextbox.Text;

            JsonConverter<ApplicationConfig>.Serialize(config, "config\\config.json");

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

                versionService.InstalledVersions = JsonConverter<List<GodotVersionInstalled>>.Deserialize(manifestFile);

                GodotInstallLocationTextbox.Text = path;
            }
        }
    }
}
