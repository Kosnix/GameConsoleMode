using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GAMINGCONSOLEMODE
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class settings : Page
    {
        public settings()
        {
            this.InitializeComponent();

            try
            {
                string latestVersion = GetLatestVersion();
                versiontext.Text = ("Latest version: " + latestVersion);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }


        }

        public static string GetLatestVersion()
        {
            // GitHub API endpoint for the latest release
            string apiUrl = "https://api.github.com/repos/Kosnix/GameConsoleMode/releases/latest";

            using (WebClient client = new WebClient())
            {
                // GitHub API requires a User-Agent header.
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                // Synchronously download the JSON response from the API.
                string json = client.DownloadString(apiUrl);

                // Parse the JSON document to extract the "tag_name" property.
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement root = doc.RootElement;
                    string tagName = root.GetProperty("tag_name").GetString();
                    return tagName;
                }
            }
        }

        private void changelogbutton_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/Kosnix/GameConsoleMode/releases";
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
                Console.WriteLine("The URL has been opened in your default browser.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening the URL: " + ex.Message);
            }
        }
    }
}