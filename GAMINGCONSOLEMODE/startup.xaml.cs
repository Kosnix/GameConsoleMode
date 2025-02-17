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


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GAMINGCONSOLEMODE
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
   
    public sealed partial class startup : Page
    {

       

        public startup()
        {
            this.InitializeComponent();
            updateui();
            
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
        }
        #endregion update ui
        #region functions
        #region csloader
        private void button_uninstall_cssloader_Click(object sender, RoutedEventArgs e)
        {
            // open "Install Apps" in Windows-Settings
            Process.Start(new ProcessStartInfo("ms-settings:appsfeatures"));
        }

        private async void button_install_cssloader_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // Check if Joyxoff is already installed
                string cssloaderExePath = @"C:\Program Files\CSSLoader Desktop\CSSLoader Desktop.exe";
                if (File.Exists(cssloaderExePath))
                {
                    messagebox("Css Loader is installed and will start now");
                    Process.Start(cssloaderExePath);
                    return;
                }

                // Get current directory
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Path to the MSI file
                string msiPath = Path.Combine(currentDirectory, "cssloader.msi");

                // Check if the MSI file exists
                if (!File.Exists(msiPath))
                {
                    messagebox("the file {msiPath} was not found.");
                    return;
                }

                // Start process to install the MSI file with the passive parameter
                Process process = new Process();
                process.StartInfo.FileName = "msiexec";
                process.StartInfo.Arguments = $"/i \"{msiPath}\" /passive";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                // Start process
                process.Start();
                process.WaitForExit();

                // Check if the installation was successful
                if (process.ExitCode == 0)
                {
                    messagebox("Installation completed successfully.");
                    // Check if Joyxoff is already installed
                    cssloaderExePath = @"C:\Program Files\CSSLoader Desktop\CSSLoader Desktop.exe";
                    if (File.Exists(cssloaderExePath))
                    {
                        text_install_state_cssloader.Text = "INSTALLED";
                        border_install_state_cssloader.Background = new SolidColorBrush(Colors.Green);
                        Process.Start(cssloaderExePath);
                        use_cssloader.IsEnabled = true;
                        use_cssloader.IsOn = true;
                        return;
                    }
                    else
                    {
                        text_install_state_cssloader.Text = "NOT INSTALLED";
                        border_install_state_cssloader.Background = new SolidColorBrush(Colors.Brown);
                        use_cssloader.IsEnabled = false;
                        use_cssloader.IsOn = false;

                    }
                }
                else
                {
                    messagebox($"Installation failed. Error code: {process.ExitCode}\", \"Error");
                    text_install_state_cssloader.Text = "NOT INSTALLED";
                    border_install_state_cssloader.Background = new SolidColorBrush(Colors.Brown);
                    use_cssloader.IsEnabled = false;
                    use_cssloader.IsOn = false;
                }
            }
            catch (Exception ex)
            {
                messagebox($"An error occurred: {ex.Message}");
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
            // open "Install Apps" in Windows-Settings
            Process.Start(new ProcessStartInfo("ms-settings:appsfeatures"));
        }

        private void button_install_joyxoff_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if Joyxoff is already installed
                string joyxoffExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Joyxoff", "Joyxoff.exe");
                if (File.Exists(joyxoffExePath))
                {
                    messagebox("Joyxoff is already installed and will now start");
                    Process.Start(joyxoffExePath);
                    return;
                }

                // Get current directory
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Path to the MSI file
                string msiPath = Path.Combine(currentDirectory, "Joyxoff.msi");

                // Check if the MSI file exists
                if (!File.Exists(msiPath))
                {
                    messagebox($"The file {msiPath} was not found.");
                    return;
                }

                // Start process to install the MSI file with the passive parameter
                Process process = new Process();
                process.StartInfo.FileName = "msiexec";
                process.StartInfo.Arguments = $"/i \"{msiPath}\" /passive";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                // Start process
                process.Start();
                process.WaitForExit();

                // Check if the installation was successful
                if (process.ExitCode == 0)
                {
                    messagebox("Installation completed successfully.");
                    // Check if Joyxoff is already installed
                    joyxoffExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Joyxoff", "Joyxoff.exe");
                    if (File.Exists(joyxoffExePath))
                    {
                        text_install_state_joyxoff.Text = "INSTALLED";
                        border_install_state_joyxoff.Background = new SolidColorBrush(Colors.Green);
                        Process.Start(joyxoffExePath);
                        use_joyxoff.IsEnabled = true;
                        use_joyxoff.IsOn = true;
                        return;
                    }
                    else
                    {
                        text_install_state_joyxoff.Text = "NOT INSTALLED";
                        border_install_state_joyxoff.Background = new SolidColorBrush(Colors.Brown);
                        use_joyxoff.IsEnabled = false;
                        use_joyxoff.IsOn = false;

                    }
                }
                else
                {
                    messagebox($"Installation failed. Error code: {process.ExitCode}");
                    text_install_state_joyxoff.Text = "NOT INSTALLED";
                    border_install_state_joyxoff.Background = new SolidColorBrush(Colors.Brown);
                    use_joyxoff.IsEnabled = false;
                    use_joyxoff.IsOn = false;
                }
            }
            catch (Exception ex)
            {
                messagebox($"An error occurred: {ex.Message}");
                use_joyxoff.IsEnabled = false;
                use_joyxoff.IsOn = false;
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
            // open "Install Apps" in Windows-Settings
            Process.Start(new ProcessStartInfo("ms-settings:appsfeatures"));
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
        #endregion functions




    }
}
