using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.IO;
using System.Linq;
using Windows.Graphics;
using System.Management;
using WinRT.Interop;
using GAMINGCONSOLEMODE;
using Windows.UI.ApplicationSettings;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using gcmloader;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GAMINGCONSOLEMODE
{
    
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        string owner = "Kosnix";  // Repository owner
    string repo = "GameConsoleMode";  // Repository name
    string currentVersion = "2.1.0";  // Your current version
        public MainWindow()
        {
            this.InitializeComponent();
            // Set the window size
            SetWindowSize(1500, 1100);
            // 1. First Start: Create folder and default config file if needed
            AppSettings.FirstStart();
            if(IsROGAlly() == true)
            {
                ///Is Rog Ally
                contentFrame.Navigate(typeof(rogally));

                //Test if Audio function is installed 


            }
            else
            {
                #region onboarding
                try
                {

                    if (AppSettings.Load<bool>("onboarding") == true)
                    {
                        // Navigate to the 'startup' page on app launch
                        contentFrame.Navigate(typeof(Home), null, new SlideNavigationTransitionInfo()
                        {
                            Effect = SlideNavigationTransitionEffect.FromRight
                        });
                    }
                    else
                    {
                        //navigate to the onboarding page
                        // Navigate to the 'startup' page on app launch
                        contentFrame.Navigate(typeof(onboarding), null, new SlideNavigationTransitionInfo()
                        {
                            Effect = SlideNavigationTransitionEffect.FromRight
                        });
                        AppSettings.Save("onboarding", true);

                    }
                }
                catch
                {
                    //navigate to the onboarding page
                    // Navigate to the 'startup' page on app launch
                    contentFrame.Navigate(typeof(onboarding), null, new SlideNavigationTransitionInfo()
                    {
                        Effect = SlideNavigationTransitionEffect.FromRight
                    });
                    AppSettings.Save("onboarding", true);
                }
                #endregion onboarding
            }

            _ = UpdateCheck(this);
        }

        #region programm start
        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {

                if (args.IsSettingsSelected)
                {
                    // Logik for settings
                    contentFrame.Navigate(typeof(settings));

                }
                else
                {

                    string selectedTag = args.SelectedItemContainer.Tag.ToString();

                    // Determine the target page based on the tag
                    Type pageType = selectedTag switch
                    {
                        "OnboardingPage" => typeof(onboarding),
                        "GCMPage" => typeof(Home),
                        "LauncherPage" => typeof(launcher),
                        "shortcuts" => typeof(shortcuts),
                        "StartupPage" => typeof(startup),
                        "LinksPage" => typeof(Links),
                        "RogAllyPage" => typeof(rogally),
                        _ => null
                    };

                    if (pageType != null && contentFrame.CurrentSourcePageType != pageType)
                    {
                        // Use SlideNavigationTransitionInfo to specify the transition direction
                        var transitionInfo = new SlideNavigationTransitionInfo
                        {
                            Effect = SlideNavigationTransitionEffect.FromRight
                        };

                        // Navigate with transition info
                        contentFrame.Navigate(pageType, null, transitionInfo);
                    }
                }
            }
        }

        private void SetWindowSize(int width, int height)
        {
            // Get the native HWND for the current window
            var hWnd = WindowNative.GetWindowHandle(this);

            // Get the WindowId for the HWND
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

            // Get the AppWindow using the WindowId
            var appWindow = AppWindow.GetFromWindowId(windowId); // Korrekt aufgerufen mit dem Typnamen AppWindow

            if (appWindow != null)
            {
                // Set the window size
                appWindow.Resize(new SizeInt32(width, height));
            }
        }

        public static implicit operator string(MainWindow v)
        {
            throw new NotImplementedException();
        }
        #endregion programm start

        #region Update
        public async Task UpdateCheck(MainWindow mainWindow)
        {
            string latestVersion = await GetLatestReleaseVersion(owner, repo); // Await the async call

            if (!string.IsNullOrEmpty(latestVersion))
            {
                Console.WriteLine($"Latest available version: {latestVersion}");

                if (IsNewerVersion(currentVersion, latestVersion))
                {
                    Console.WriteLine("An update is available!");
                    mainWindow.UpdateBar.Visibility = Visibility.Visible;
                }
                else
                {
                    Console.WriteLine("You are up to date.");
                }
            }
            else
            {
                Console.WriteLine("Could not retrieve the version.");
            }
        }

        static async Task<string> GetLatestReleaseVersion(string owner, string repo)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "C# App"); // Required for GitHub API

            string url = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";

            try
            {
                string json = await client.GetStringAsync(url);
                using JsonDocument doc = JsonDocument.Parse(json);
                return doc.RootElement.GetProperty("tag_name").GetString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        private bool IsNewerVersion(string currentVersion, string latestVersion)
        {
            Version.TryParse(currentVersion, out Version current);
            Version.TryParse(latestVersion, out Version latest);
            return latest > current;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            _ = DownloadLatestRelease(owner, repo, UpdateProgressBar);
        }

        private void InstallUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Starte die Update.exe aus dem AppData\gcmsettings Ordner
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gcmsettings", "Update.exe"),
                    UseShellExecute = true
                };
                Process.Start(startInfo);

                // Schließe die aktuelle Anwendung
                Application.Current.Exit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'exécution de la mise à jour : {ex.Message}");
            }
        }

        private async Task DownloadLatestRelease(string owner, string repo, ProgressBar progressBar)
        {
            using HttpClient client = new HttpClient();
            string latestReleaseUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

            var response = await client.GetStringAsync(latestReleaseUrl);
            string downloadUrl = ExtractDownloadUrl(response);

            if (!string.IsNullOrEmpty(downloadUrl))
            {
                string fileName = Path.GetFileName(new Uri(downloadUrl).AbsolutePath);
                // Download in den AppData\gcmsettings Ordner
                string updateDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gcmsettings");
                if (!Directory.Exists(updateDir))
                {
                    Directory.CreateDirectory(updateDir);
                }

                string filePath = Path.Combine(updateDir, "Update.exe");

                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                using var httpStream = await client.GetStreamAsync(downloadUrl);

                var buffer = new byte[8192];
                long totalBytesRead = 0;
                int bytesRead;

                progressBar.Visibility = Visibility.Visible;
                while ((bytesRead = await httpStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                    progressBar.Value = (double)totalBytesRead / fileStream.Length * 100;
                }

                progressBar.Visibility = Visibility.Collapsed;
                InstallUpdateButton.Visibility = Visibility.Visible;
                UpdateButton.Visibility = Visibility.Collapsed;
                UpdateBarText.Text = "Install";
                Console.WriteLine($"Downloaded latest version to {filePath}");
            }
            else
            {
                Console.WriteLine("Could not find a valid download URL.");
            }
        }

        private string ExtractDownloadUrl(string jsonResponse)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;
            if (root.TryGetProperty("assets", out JsonElement assetsArray) && assetsArray.GetArrayLength() > 0)
            {
                foreach (var asset in assetsArray.EnumerateArray())
                {
                    if (asset.TryGetProperty("browser_download_url", out JsonElement downloadUrl))
                    {
                        return downloadUrl.GetString();
                    }
                }
            }
            return string.Empty;
        }
        #endregion Update

        #region filter
        #endregion filter

        #region rog ally

        private void allyaudiobuttonfix()
        {
            string audioSwitchExe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "AudioSwitch", "AudioSwitch.exe");
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string configSourcePath = Path.Combine(AppContext.BaseDirectory, "Settings.xml");
            string configTargetDir = Path.Combine(localAppData, "AudioSwitch");
            string configTargetPath = Path.Combine(configTargetDir, "Settings.xml");
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string startupLink = Path.Combine(startupFolder, "AudioSwitch.lnk");

            bool softwareExists = File.Exists(audioSwitchExe);
            bool configExists = File.Exists(configTargetPath);

            if (softwareExists && configExists)
            {
                return; // Nothing to do, silent exit
            }

            var result = MessageBox(IntPtr.Zero, "A software is required to enable your volume buttons. We will now start the installation.", "AudioSwitch Setup", 1);
            if (result != 1) return; // IDOK = 1

            try
            {
                bool alreadyInstalled = softwareExists;
                string installerPath = Path.Combine(AppContext.BaseDirectory, "asforally.exe");

                if (!alreadyInstalled)
                {
                    if (!File.Exists(installerPath))
                    {
                        MessageBox(IntPtr.Zero, "Installer file not found: " + installerPath, "Installation Status", 0);
                        return;
                    }

                    Process installer = new Process();
                    installer.StartInfo.FileName = installerPath;
                    installer.StartInfo.Arguments = "/verySilent";
                    installer.StartInfo.UseShellExecute = false;
                    installer.StartInfo.CreateNoWindow = true;

                    installer.Start();
                    installer.WaitForExit();
                }

                // After install or if already installed:

                if (File.Exists(startupLink))
                {
                    File.Delete(startupLink);
                }

                Directory.CreateDirectory(configTargetDir);

                if (File.Exists(configSourcePath))
                {
                    File.Copy(configSourcePath, configTargetPath, true);
                }
                else
                {
                    MessageBox(IntPtr.Zero, "Config XML not found: " + configSourcePath, "Config Copy", 0);
                }

                if (alreadyInstalled)
                {
                    MessageBox(IntPtr.Zero, "AudioSwitch already installed.", "Installation Status", 0);
                }
                else
                {
                    MessageBox(IntPtr.Zero, "AudioSwitch installed successfully. Config set", "Installation Status", 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox(IntPtr.Zero, "Error: " + ex.Message, "Execution Error", 0);
            }
        }


        static bool IsROGAlly()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystemProduct");
                foreach (var o in searcher.Get())
                {
                    var obj = (ManagementObject)o;
                    string name = obj["Name"]?.ToString() ?? "";
                    if (name.Contains("ROG Ally", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[WARN] Could not determine system model: " + ex.Message);
            }

            return false;
        }
        #endregion rog ally


    }
}