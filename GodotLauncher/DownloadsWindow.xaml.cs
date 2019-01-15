using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

using GodotLauncher.Services;
using GodotLauncher.DataClasses;

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
        }

        void BuildVersionsTree()
        {
            GodotVersionsTree.Items.Clear();
        }
    }
}
