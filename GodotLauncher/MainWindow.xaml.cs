using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;

namespace GodotLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string uri = FindResource("SourceUrlBase").ToString();
            var request = WebRequest.Create(uri);
            var response = request.GetResponse();
            var regex = new Regex("<a href=\"*.*\">(?<name>.*)</a>/");
            var versionRegex = new Regex(@"^\d+\.\d+\.?\d*$");

            using(var reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();
                var matches = regex.Matches(result);

                if (matches.Count == 0)
                {
                    logger.Error($"Parsing {uri} response failed.");
                    return;
                }

                string files = string.Empty;
                foreach(Match match in matches)
                {
                    if (!match.Success)
                        continue;

                    string dirName = match.Groups["name"].ToString();
                    if (versionRegex.IsMatch(dirName)) 
                        files += dirName + "\n";
                }

                MessageBox.Show(files);
            }
        }

        private void Configure_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}