using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System;
using System.IO;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System.Collections.Generic;
using NAudio.CoreAudioApi;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Net.Http;
using Flurl.Http;
using static GAMINGCONSOLEMODE.launcher;
using System.Threading.Tasks;
using System.Reflection;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GAMINGCONSOLEMODE
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
   
    public sealed partial class startup : Page
    {

        private DispatcherTimer _timer;

        public startup()
        {
            this.InitializeComponent();
            updateui();
            InitializeTimer();
        }
        
        #region code functions
        MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        private async void messagebox(string dialog)
        {
            var messagebox = new ContentDialog
            {
                Title = "Information",
                Content = dialog,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot // WICHTIG: Verknüpft den Dialog mit dem aktuellen Fenster


            };

            await messagebox.ShowAsync();
        }
        #endregion code functions
        #region filter
        private void FilterPanels(string category)
        {
            if (category == "All")
            {
                foreach (UIElement child in ContentArea.Children)
                {
                    if (child is Border border)
                    {
                        // Wenn "All" gewählt, alle Panels anzeigen
                        border.Visibility = category == "All" || border.Tag.ToString() == category
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                    }
                }

            }
            else
            {

                // Alle Kinder des ContentArea durchgehen
                foreach (UIElement child in ContentArea.Children)
                {
                    if (child is Border border && border.Tag != null)
                    {
                        // Sichtbarkeit basierend auf der Kategorie einstellen
                        border.Visibility = border.Tag.ToString() == category ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }

           
        }

      

        // Filterlogik Button-Event
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Kategorie aus dem Tag des Buttons holen
            var button = sender as Button;
            string category = button.Tag.ToString();

            // Filter anwenden
            FilterPanels(category);
        }
        #endregion filter
        #region update ui

        private void InitializeTimer()
        {
            // Create a new DispatcherTimer with a 10-second interval
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            // Subscribe to the Tick event
            _timer.Tick += Timer_Tick;

            // Start the timer
            _timer.Start();
        }
        private void Timer_Tick(object sender, object e)
        {
            // Call your update method on every tick
            updateui();
        }

        private void updateui()
        {
            #region cssloader
            try
            {
                string cssloaderExePath = @"C:\Program Files\CSSLoader Desktop\CSSLoader Desktop.exe";
                if (File.Exists(cssloaderExePath))
                {
                    text_install_state_cssloader.Text = "INSTALLED";
                    border_install_state_cssloader.Background = new SolidColorBrush(Colors.Green);
                    use_cssloader.IsEnabled = true;

                    //toggle ui
                    bool cssloadertogglestatus = AppSettings.Load<bool>("usecssloader");
                    if (cssloadertogglestatus == true)
                    {
                        use_cssloader.IsOn = true;
                    }
                    else
                    {
                        use_cssloader.IsOn = false;
                    }
                }
                else
                {
                    text_install_state_cssloader.Text = "NOT INSTALLED";
                    border_install_state_cssloader.Background = new SolidColorBrush(Colors.Brown);
                    use_cssloader.IsEnabled = false;
                    use_cssloader.IsOn = false;
                }
            }
            catch
            {

            }
            #endregion cssloader
            #region joyxoff
            string joyxoffExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Joyxoff", "Joyxoff.exe");

            try
            {
                if (File.Exists(joyxoffExePath))
                {
                    text_install_state_joyxoff.Text = "INSTALLED";
                    border_install_state_joyxoff.Background = new SolidColorBrush(Colors.Green);
                    use_joyxoff.IsEnabled = true;
                    //toggle ui
                    bool joyxofftogglestatus = AppSettings.Load<bool>("usejoyxoff");
                    if (joyxofftogglestatus == true)
                    {
                        use_joyxoff.IsOn = true;
                    }
                    else
                    {
                        use_joyxoff.IsOn = false;
                    }

                }
                else
                {
                    text_install_state_joyxoff.Text = "NOT INSTALLED";
                    border_install_state_joyxoff.Background = new SolidColorBrush(Colors.Brown);
                    use_joyxoff.IsEnabled = false;
                    use_joyxoff.IsOn = false;

                }
            }catch 
            {
            
            }



            #endregion joyxoff
            #region displayfusion
            // Registry path for DisplayFusion profiles
            string registryPath = @"Software\Binary Fortress Software\DisplayFusion\MonitorConfig";
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath))
                {
                    if (key != null)
                    {
                        // Iterate over all profiles in the registry
                        foreach (string profileName in key.GetSubKeyNames())
                        {
                            using (RegistryKey profileKey = key.OpenSubKey(profileName))
                            {
                                if (profileKey != null)
                                {
                                    // Read data from the registry
                                    string name = profileKey.GetValue("Name")?.ToString();
                                    string audioPlaybackMM = profileKey.GetValue("AudioPlaybackMM")?.ToString();
                                    string monitorJson = profileKey.GetValue("Monitor0_Json")?.ToString();


                                    //Displayfusion installed
                                    text_install_state_displayfusion.Text = "INSTALLED";
                                    border_install_state_displayfusion.Background = new SolidColorBrush(Colors.Green);
                                    use_displayfusion.IsEnabled = true;
                                    displayfusion_end.IsEnabled = true;
                                    use_displayfusion.IsEnabled = true;

                                    //toggle ui
                                    bool displayfusiontogglestatus = AppSettings.Load<bool>("usedisplayfusion");
                                    if (displayfusiontogglestatus == true)
                                    {
                                        use_displayfusion.IsOn = true;
                                    }
                                    else
                                    {
                                        use_displayfusion.IsOn = false;
                                    }

                                    // Parse JSON data
                                    if (!string.IsNullOrEmpty(monitorJson))
                                    {
                                        var monitorData = JObject.Parse(monitorJson);
                                        string makeModel = monitorData["UniqueID"]?["MakeModel"]?.ToString();
                                        string dpiScalingPercent = monitorData["DpiScalingPercent"]?.ToString();

                                    }

                                    //Load Profile Name Data
                                    string displayfusionstartprofile = AppSettings.Load<string>("usedisplayfusion_start");
                                    string displayfusionendprofile = AppSettings.Load<string>("usedisplayfusion_end");
                                    displayfusion_start.Text = displayfusionstartprofile;
                                    displayfusion_end.Text = displayfusionendprofile;

                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("The registry path was not found. or program is not installed");
                        text_install_state_displayfusion.Text = "NOT INSTALLED OR NO DISPLAY CONFIG";
                        border_install_state_displayfusion.Background = new SolidColorBrush(Colors.Brown);
                        displayfusion_start.IsEnabled = false;
                        displayfusion_end.IsEnabled = false;
                        use_displayfusion.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while reading the registry: {ex.Message}");

                //Displayfusion not installed or error
            }
            #endregion displayfusion
            #region gcmwallpaper
            try
            {
                bool gcmwallpaper = AppSettings.Load<bool>("gcmwallpaper");
                if (gcmwallpaper == true)
                {
                    use_wallpaper.IsOn = true;
                    text_install_state_wallpaper.Text = "ACTIVATED";
                    border_install_state_wallpaper.Background = new SolidColorBrush(Colors.Green);
                    //text
                    string wallpaperpath = AppSettings.Load<string>("gcmwallpaperpath");
                    wallpaper_path.Text = wallpaperpath;
                }
                else
                {
                    use_wallpaper.IsOn = false;
                    text_install_state_wallpaper.Text = "DISABLED";
                    border_install_state_wallpaper.Background = new SolidColorBrush(Colors.Brown);
                    wallpaper_path.Text = "";
                }
            }
            catch
            {
                Console.WriteLine("wallpaper gui error");
            }
            #endregion gcmwallpaper
            #region discord
            try
            {
                bool usediscord = AppSettings.Load<bool>("usediscord");
                if(usediscord == true)
                {
                    use_discord.IsOn = true;
                    discord_end.IsEnabled = true;
                    discord_start.IsEnabled = true;
                    PopulateAudioDevices();

                    text_install_state_discord.Text = "ACTIVATED";
                    border_install_state_discord.Background = new SolidColorBrush(Colors.Green);
                    infobar_discord.IsOpen = false;
                }
                else
                {
                    use_discord.IsOn = false;
                    discord_end.IsEnabled = false;
                    discord_start.IsEnabled = false;
                    

                    text_install_state_discord.Text = "DISABLED";
                    border_install_state_discord.Background = new SolidColorBrush(Colors.Brown);
                    infobar_discord.IsOpen = false;
                }
            }
            catch
            {

            }

            #endregion discord
            #region StartupVideo
            try {
                bool usestartupvideo = AppSettings.Load<bool>("usestartupvideo");
                if (usestartupvideo == true) {
                    text_install_state_Startup_Video.Text = "ACTIVATED";
                    border_install_state_Startup_Video.Background = new SolidColorBrush(Colors.Green);
                    use_startup_video.IsOn = true;
                }
                else
                {
                    text_install_state_Startup_Video.Text = "DISABLED";
                    border_install_state_Startup_Video.Background = new SolidColorBrush(Colors.Brown);
                    use_startup_video.IsOn = false;
                }
                string startupvideo_path = AppSettings.Load<string>("startupvideo_path");
                if (startupvideo_path == "")
                {
                    startupvideo_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\GCM_Startup_Video.webm");
                    AppSettings.Save("startupvideo_path", startupvideo_path);
                }
                    textbox_startupvideo_path.Text = startupvideo_path;

            if(AppSettings.Load<bool>("usesteamstartupvideo") == true)
                {
                    UseSteamStartupVideo.IsChecked = true;
                    Injectstartupvideo_button.IsEnabled = true;
                }
                else
                {
                    UseSteamStartupVideo.IsChecked = false;
                    Injectstartupvideo_button.IsEnabled = false;
                }
            }
            catch {}

            #endregion StartupVideo
        }
        #endregion update ui
        #region functions
        #region csloader
        private void button_uninstall_cssloader_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var psi = new ProcessStartInfo("ms-settings:appsfeatures")
                {
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening Windows settings: " + ex.Message);
            }
        }

        private async void button_install_cssloader_Click(object sender, RoutedEventArgs e)
        {

            // 1. Retrieve the latest release information from GitHub API
            string latestReleaseApiUrl = "https://api.github.com/repos/DeckThemes/CSSLoader-Desktop/releases/latest";
            using (HttpClient httpClient = new HttpClient())
            {
                // GitHub API requires a valid User-Agent header
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                string releaseJson;
                try
                {
                    releaseJson = await httpClient.GetStringAsync(latestReleaseApiUrl);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error retrieving the latest release info: " + ex.Message);
                    return;
                }

                // 2. Parse the JSON to extract the tag name and select the MSI asset
                JsonDocument jsonDoc = JsonDocument.Parse(releaseJson);
                JsonElement root = jsonDoc.RootElement;
                string tagName = root.GetProperty("tag_name").GetString();
                Console.WriteLine("Latest release version: " + tagName);

                // Get the assets array from the JSON
                JsonElement assets = root.GetProperty("assets");
                JsonElement? msiAsset = null;
                foreach (JsonElement asset in assets.EnumerateArray())
                {
                    string assetName = asset.GetProperty("name").GetString();
                    // Select asset if it ends with .msi (case-insensitive)
                    if (assetName.EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                    {
                        msiAsset = asset;
                        break;
                    }
                }

                if (msiAsset == null)
                {
                    Console.WriteLine("No MSI asset found in the latest release.");
                    return;
                }

                string selectedAssetName = msiAsset.Value.GetProperty("name").GetString();
                string downloadUrl = msiAsset.Value.GetProperty("browser_download_url").GetString();

                Console.WriteLine("Selected asset: " + selectedAssetName);
                Console.WriteLine("Download URL: " + downloadUrl);

                // 3. Determine the destination folder (create a custom subfolder in the Downloads directory)
                string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "DownloadedInstaller");
                if (!Directory.Exists(downloadsFolder))
                    Directory.CreateDirectory(downloadsFolder);

                string destinationPath = Path.Combine(downloadsFolder, selectedAssetName);

                // 4. Download the MSI file using Flurl.Http (the async download is awaited)
                try
                {
                    Console.WriteLine("Starting download of the MSI asset...");
                    await downloadUrl.DownloadFileAsync(downloadsFolder, selectedAssetName);
                    Console.WriteLine("Download complete. File saved at:");
                    Console.WriteLine(destinationPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Download error: " + ex.Message);
                    return;
                }

                // 5. Open the folder in File Explorer so the user can directly access the downloaded MSI file
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"\"{downloadsFolder}\"",
                        UseShellExecute = true
                    });
                    Console.WriteLine("Folder opened in File Explorer.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error opening the folder: " + ex.Message);
                }
            }

        }

        private void use_cssloader_Toggled(object sender, RoutedEventArgs e)
        {
           

            if (use_cssloader.IsOn == true)
            {
                AppSettings.Save("usecssloader", true);
            }
            else
            {
                AppSettings.Save("usecssloader", false);
            }
        }
        #endregion csloader
        #region joyxoff
        private void button_uninstall_joyxoff_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var psi = new ProcessStartInfo("ms-settings:appsfeatures")
                {
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening Windows settings: " + ex.Message);
            }
        }
        private async void button_install_joyxoff_Click(object sender, RoutedEventArgs e)
        {
            // 1. Dynamically determine the version from the download page
            string baseDownloadUrl = "https://joyxoff.com/download.php?culture=en";
            string pageContent = "";
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                    pageContent = wc.DownloadString(baseDownloadUrl);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving the page: " + ex.Message);
                return;
            }

            // Regex searches for a link that contains the version pattern, e.g. ?culture=en&version=3.63.10.7
            string pattern = @"download\.php\?culture=en&version=([\d\.]+)";
            var match = Regex.Match(pageContent, pattern);
            string version = match.Success ? match.Groups[1].Value : "3.63.10.7";
            Console.WriteLine("Found version: " + version);

            // 2. Build the download URL
            string downloadUrl = $"https://joyxoff.com/download.php?culture=en&version={version}";
            Console.WriteLine("Download URL: " + downloadUrl);

            // 3. Destination folder: create a custom subfolder in the Downloads directory
            string downloadFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads",
                "joyxoffInstaller");
            if (!Directory.Exists(downloadFolder))
                Directory.CreateDirectory(downloadFolder);

            string fileName = "joyxoff.rar";
            string destinationPath = Path.Combine(downloadFolder, fileName);

            // 4. Download the file using Flurl.Http (the async download is executed synchronously)
            try
            {
                Console.WriteLine("Starting download with Flurl.Http ...");
                downloadUrl.DownloadFileAsync(downloadFolder, fileName).GetAwaiter().GetResult();
                Console.WriteLine("Download complete. File saved at:");
                Console.WriteLine(destinationPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Download error: " + ex.Message);
                return;
            }

            // 5. Open the folder in File Explorer so the user can see the downloaded RAR file
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{downloadFolder}\"",
                    UseShellExecute = true
                });
                Console.WriteLine("Folder opened in File Explorer.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening the folder: " + ex.Message);
            }
        }

        private void use_joyxoff_Toggled(object sender, RoutedEventArgs e)
        {
            if(use_joyxoff.IsOn == true)
            {
                AppSettings.Save("usejoyxoff", true);
            }
            else
            {
                AppSettings.Save("usejoyxoff", false);
            }
            
        }
        #endregion joyxoff
        #region displayfusion
        private void use_displayfusion_Toggled(object sender, RoutedEventArgs e)
        {
            if (use_displayfusion.IsOn == true)
            {
                AppSettings.Save("usedisplayfusion", true);
            }
            else
            {
                AppSettings.Save("usedisplayfusion", false);
            }
        }
        private void button_uninstall_displayfusion_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var psi = new ProcessStartInfo("ms-settings:appsfeatures")
                {
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening Windows settings: " + ex.Message);
            }
        }
        private void button_install_displayfusion_Click_2(object sender, RoutedEventArgs e)
        {
            // URL to open
            string url = "https://www.displayfusion.com/download/";

            try
            {
                // Open the URL in the default web browser
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // Ensures it opens in the default browser
                });

                Console.WriteLine("Webpage launched successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error launching webpage: {ex.Message}");
            }
        }
        private void displayfusion_start_TextChanged(object sender, TextChangedEventArgs e)
        {
            string startprofile = displayfusion_start.Text;
            AppSettings.Save("usedisplayfusion_start", startprofile);
        }
        private void displayfusion_end_TextChanged(object sender, TextChangedEventArgs e)
        {
            string endprofile = displayfusion_end.Text;
            AppSettings.Save("usedisplayfusion_end", endprofile);
        }
        #endregion displayfusion
        #region wallpaper GCM
        private void wallpaper_path_TextChanged(object sender, TextChangedEventArgs e)
        {
            string wallpaperpath = wallpaper_path.Text;
            AppSettings.Save("gcmwallpaperpath", wallpaperpath);
        }
        private void use_wallpaper_Toggled(object sender, RoutedEventArgs e)
        {
            if (use_wallpaper.IsOn == true)
            {
                AppSettings.Save("gcmwallpaper", true);
                text_install_state_wallpaper.Text = "ACTIVATED";
                border_install_state_wallpaper.Background = new SolidColorBrush(Colors.Green);
             
            }
            else
            {
                AppSettings.Save("gcmwallpaper", false);
                text_install_state_wallpaper.Text = "DISABLED";
                border_install_state_wallpaper.Background = new SolidColorBrush(Colors.Brown);
                wallpaper_path.Text = "";
            }
        }
        #endregion wallpaper GCM
        #region discord
        private void use_discord_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                PopulateAudioDevices();

                if (use_discord.IsOn == true)
                {
                    AppSettings.Save("usediscord", true);
                    discord_end.IsEnabled = true;
                    discord_start.IsEnabled = true;

                    text_install_state_discord.Text = "ACTIVATED";
                    border_install_state_discord.Background = new SolidColorBrush(Colors.Green);

                }
                else
                {
                    AppSettings.Save("usediscord", false);
                    discord_end.IsEnabled = false;
                    discord_start.IsEnabled = false;

                    text_install_state_discord.Text = "DISABLED";
                    border_install_state_discord.Background = new SolidColorBrush(Colors.Brown);
                }
            }
            catch
            {
                Console.WriteLine("problem with discord integration");
            }
        }

        private Dictionary<string, string> deviceMap = new Dictionary<string, string>();
        private void PopulateAudioDevices()
        {
            try
            {
                var enumerator = new MMDeviceEnumerator();

                // Clear existing items and reset the device map
                discord_end.Items.Clear();
                discord_start.Items.Clear();
                deviceMap.Clear();

                // Get playback devices
                foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active))
                {
                    // Geräte-Namen zur ComboBox hinzufügen
                    discord_end.Items.Add(device.FriendlyName);
                    discord_start.Items.Add(device.FriendlyName);

                    // Zuordnung von FriendlyName zu ID speichern
                    deviceMap[device.FriendlyName] = device.FriendlyName;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while populating audio devices: {ex.Message}");
            }
        }

        private void discord_end_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox comboBox = sender as ComboBox;

                if (comboBox != null && comboBox.SelectedItem != null)
                {
                    // Hole den ausgewählten Gerätenamen
                    string selectedDeviceName = comboBox.SelectedItem.ToString();
                    string cleanedDeviceName = selectedDeviceName.Split('(')[0].Trim();

                    // Speichere die Geräte-ID
                    AppSettings.Save("discordend", cleanedDeviceName);
                    

                    // Bestätigung ausgeben
                    Console.WriteLine($"Saved Device ID: {cleanedDeviceName}");

                        infobar_discord.IsOpen = true;
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving selected device: {ex.Message}");
            }
        }
        private void discord_start_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox comboBox = sender as ComboBox;

                if (comboBox != null && comboBox.SelectedItem != null)
                {
                    // Hole den ausgewählten Gerätenamen
                    string selectedDeviceName = comboBox.SelectedItem.ToString();
                    string cleanedDeviceName = selectedDeviceName.Split('(')[0].Trim();

                    // Speichere die Geräte-ID
                    AppSettings.Save("discordstart", cleanedDeviceName);

                        // Bestätigung ausgeben
                        Console.WriteLine($"Saved Device ID: {cleanedDeviceName}");
                        infobar_discord.IsOpen = true;
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving selected device: {ex.Message}");
            }
        }
        #endregion discord
        #region StartupVideo
        private void textbox_startupvideo_path_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                AppSettings.Save("startupvideo_path", textbox_startupvideo_path.Text);
            }
            catch { }
        }

        private void use_startup_video_Toggled(object sender, RoutedEventArgs e)
        {
            if (use_startup_video.IsOn == true)
            {
                AppSettings.Save("usestartupvideo", true);
                text_install_state_Startup_Video.Text = "ACTIVÉ";
                border_install_state_Startup_Video.Background = new SolidColorBrush(Colors.Green);
            }
            else
            {
                AppSettings.Save("usestartupvideo", false);
                text_install_state_Startup_Video.Text = "DÉSACTIVÉ";
                border_install_state_Startup_Video.Background = new SolidColorBrush(Colors.Brown);
            }
        }

        private void pichstartupvideopath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pichstartupvideopath.IsEnabled = false;
                string? file = FilePicker.ShowDialog(
                    "C:\\",  // Starting folder
                    new string[] { "*" },  // Accepted extensions
                    "*",  // Full filter with correct syntax
                    "Select a video file"  // Dialog box title
                );

                if (!string.IsNullOrEmpty(file))
                {
                    AppSettings.Save("startupvideo_path", file);
                    textbox_startupvideo_path.Text = file;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error when selecting the file: {ex.Message}");
            }
            pichstartupvideopath.IsEnabled = true;
        }

        private void UseSteamStartupVideoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save the checkbox state in the settings
                AppSettings.Save("usesteamstartupvideo", true);

                // Enable video injection
                Injectstartupvideo_button.IsEnabled = true;
                textbox_select_startupvideo_path.Visibility = Visibility.Collapsed; // Hide the text field
            }
            catch { }
        }

        private void UseSteamStartupVideoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save the checkbox state in the settings
                AppSettings.Save("usesteamstartupvideo", false);

                // Disable video injection
                Injectstartupvideo_button.IsEnabled = false;
                textbox_select_startupvideo_path.Visibility = Visibility.Visible; // Show the text field
            }
            catch { }
        }

        private void Injectstartupvideo_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Injectstartupvideo_Text.Text = "";
                Injectstartupvideo_Text.Visibility = Visibility.Visible;
                Injectstartupvideo_ProgressBar.Visibility = Visibility.Visible;
                Injectstartupvideo_ProgressBar.Value = 0;
                Debug.WriteLine("Start of Injectstartupvideo_button_Click function");

                Injectstartupvideo_button.IsEnabled = false; // Disable the button

                string videoPath = AppSettings.Load<string>("startupvideo_path");
                Debug.WriteLine($"Video path retrieved: {videoPath}");

                if (string.IsNullOrEmpty(videoPath) || !File.Exists(videoPath))
                {
                    Injectstartupvideo_Text.Text = "No valid video file selected.";
                    Injectstartupvideo_ProgressBar.Visibility = Visibility.Collapsed;
                    throw new Exception("No valid video file selected.");
                }

                // Check if the source file is already WebM
                if (Path.GetExtension(videoPath).ToLower() != ".webm")
                {
                    Injectstartupvideo_Text.Text = "The selected file is not in WebM format.";
                    Injectstartupvideo_ProgressBar.Visibility = Visibility.Collapsed;
                    Injectstartupvideo_ProgressBar.Value = 0;
                    throw new Exception("The selected file is not in WebM format.");
                }

                string steamPath = AppSettings.Load<string>("steamlauncherpath");
                if (steamPath.EndsWith("steam.exe", StringComparison.OrdinalIgnoreCase))
                {
                    steamPath = Path.GetDirectoryName(steamPath);
                }

                string outputPath = Path.Combine(steamPath, "steamui", "movies", "GCM_vid.webm");

                // Check and create the folder if needed
                string moviesFolder = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(moviesFolder))
                {
                    Directory.CreateDirectory(moviesFolder);
                }

                // Copy the WebM file
                Injectstartupvideo_ProgressBar.Value = 50;
                File.Copy(videoPath, outputPath, true);
                Debug.WriteLine("File copy complete");

                Injectstartupvideo_ProgressBar.Visibility = Visibility.Visible;
                Injectstartupvideo_ProgressBar.Value = 100;
                Injectstartupvideo_Text.Text = "Video copied successfully.";
                Injectstartupvideo_button.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                Injectstartupvideo_ProgressBar.Visibility = Visibility.Collapsed;
                Injectstartupvideo_Text.Text = "Error: " + ex.Message;
                Injectstartupvideo_button.IsEnabled = true;
            }
        }

        private void Resetstartupvideopath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string startupvideo_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\GCM_Startup_Video.webm");
                AppSettings.Save("startupvideo_path", startupvideo_path);
                textbox_startupvideo_path.Text = startupvideo_path;
            }
            catch { }
        }

        #endregion StartupVideo

        #endregion functions

    }
}
