using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Collections.Generic;

using GodotLauncher.Classes;

namespace GodotLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly GodotVersionStorage versionStorage;

        public MainWindow(GodotVersionStorage versionStorage)
        {
            InitializeComponent();
            this.versionStorage = versionStorage;
        }

        private void Start_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string valami = null;

            valami.Trim();
        }

        private void Configure_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}