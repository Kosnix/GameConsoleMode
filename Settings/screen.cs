using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Threading;
using System.Diagnostics;

namespace Settings
{
    public partial class screen : Form
    {
        public screen()
        {
            InitializeComponent();
        }
        #region need variable and methodes

      

        static string exeFolder()
        {
            string cheminExecutable = Assembly.GetExecutingAssembly().Location;
            string dossierExecutable = Path.GetDirectoryName(cheminExecutable);
            return dossierExecutable;
        }
        static string Readconfig(string key)
        {
            string filePath = Path.Combine(exeFolder(), "settings.json");

            // Vérifier si le fichier existe
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine($"Le fichier {filePath} n'existe pas.");
                return string.Empty;
            }

            try
            {
                // Lire le contenu du fichier JSON
                string jsonContent = System.IO.File.ReadAllText(filePath);

                // Analyser le JSON
                JObject jsonObject = JObject.Parse(jsonContent);

                // Accéder à l'item spécifié par la clé
                JToken item = jsonObject.SelectToken($"$.Settings.{key}");
                // Vérifier si l'item existe
                if (item != null)
                {
                    string value = item.ToString();
                    Console.WriteLine($"La clé '{key}' est configurée à '{value}'");
                    return value;
                }
                else
                {
                    Console.WriteLine($"La clé '{key}' n'a pas été trouvée dans la configuration.");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la lecture du fichier JSON : {ex.Message}");
                return string.Empty;
            }
        }
        public void UpdateJsonFile(string key, string newValue)
        {
            try
            {
                // Chemin du fichier JSON
                string jsonFilePath = Path.Combine(exeFolder(), "settings.json");

                // Vérifier si le fichier existe
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    throw new FileNotFoundException("Le fichier settings.json est introuvable.");
                }

                // Lire le contenu du fichier
                string json = System.IO.File.ReadAllText(jsonFilePath);

                // Vérifier si le contenu du fichier JSON est vide
                if (string.IsNullOrWhiteSpace(json))
                {
                    throw new Exception("Le fichier JSON est vide ou contient uniquement des espaces.");
                }

                // Analyser le contenu JSON
                JObject jsonObj = JObject.Parse(json);

                // Vérifier si la clé "Settings" existe
                if (jsonObj["Settings"] == null)
                {
                    throw new KeyNotFoundException("La clé 'Settings' est introuvable dans le fichier JSON.");
                }

                // Mettre à jour la valeur de la clé
                jsonObj["Settings"][key] = newValue;

                // Écrire les modifications dans le fichier
                using (StreamWriter File = System.IO.File.CreateText(jsonFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(File, jsonObj);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erreur lors de l'analyse du fichier JSON : {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Erreur d'entrée/sortie : {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur inattendue s'est produite : {ex.Message}");
            }
        }
        #endregion need variable and methodes
        private void SetScreenCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SetScreenCheckBox.Checked == true)
            {


                UpdateJsonFile("ScreenBool", "1");
            }
            else
            {
                UpdateJsonFile("ScreenBool", "0");
            }
        }

        private void screen_Load(object sender, EventArgs e)
        {
            #region Displayfusion
            // DisplayFusion is installed; continue processing
            Console.WriteLine("Loading DisplayFusion profiles...");

            // DisplayFusion panel visibility
            // Assuming this is part of a GUI application
            displayfusion_Panel.Visible = true;

            // Registry path for DisplayFusion profiles
            string registryPath = @"Software\Binary Fortress Software\DisplayFusion\MonitorConfig";

            var profiles = new List<MonitorProfile>();

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
                                    
                                    aktivate_df_function.Enabled = true;
                                    guna2Chip_displayfusioninstall_status.Text = "INSTALLED";
                                    guna2Chip_displayfusioninstall_status.FillColor = Color.Green;
                                    guna2Chip_displayfusioninstall_status.BorderColor = Color.Green;

                                    if (Readconfig("displayfusion") == "1")
                                    {
                                        panel_df_mainpanel.Enabled = true;
                                        aktivate_df_function.Checked = true;
                                    }
                                    else if (Readconfig("displayfusion") == "0")
                                    {
                                        panel_df_mainpanel.Enabled = false;
                                        aktivate_df_function.Checked = false;
                                    }
                                    else
                                    {
                                        panel_df_mainpanel.Enabled = false;
                                        aktivate_df_function.Checked = false;
                                    }

                                    // Parse JSON data
                                    if (!string.IsNullOrEmpty(monitorJson))
                                    {
                                        var monitorData = JObject.Parse(monitorJson);
                                        string makeModel = monitorData["UniqueID"]?["MakeModel"]?.ToString();
                                        string dpiScalingPercent = monitorData["DpiScalingPercent"]?.ToString();

                                        // Add profile name to ComboBox
                                      
                                        guna2ComboBox_displayfusionprofil.Items.Add(name);

                                        // Save profile data to the list
                                        profiles.Add(new MonitorProfile
                                        {
                                            Name = name,
                                            AudioPlaybackMM = audioPlaybackMM,
                                            MakeModel = makeModel,
                                            DpiScalingPercent = dpiScalingPercent
                                        });
                                    }
                                }
                            }
                        }
                        guna2ComboBox_displayfusionprofil.SelectedIndex = 0;

                        // Output profile data to the console
                        foreach (var profile in profiles)
                        {
                            Console.WriteLine($"Profile Name: {profile.Name}");
                            Console.WriteLine($"Audio Playback MM: {profile.AudioPlaybackMM}");
                            Console.WriteLine($"Make Model: {profile.MakeModel}");
                            Console.WriteLine($"DPI Scaling Percent: {profile.DpiScalingPercent}");
                            Console.WriteLine(new string('-', 30));
                        }
                    }
                    else
                    {
                        Console.WriteLine("The registry path was not found. or program is not installed");
                        panel_df_mainpanel.Enabled = false;
                        aktivate_df_function.Enabled = false;
                        guna2Chip_displayfusioninstall_status.Text = "NOT INSTALLED";
                        guna2Chip_displayfusioninstall_status.FillColor = Color.Brown;
                        guna2Chip_displayfusioninstall_status.BorderColor = Color.Brown;
                    }
                }

                

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while reading the registry: {ex.Message}");

                //Displayfusion not installed or error

            }
            #endregion displayfusion
            #region customwallpaper
           string customwallpaperpath =  Readconfig("customwallpaperpath");
            guna2TextBox_gcmwallpaper.Text = customwallpaperpath;
            string customwallpaper = Readconfig("customwallpaper");
            if (customwallpaper == "1")
            {
                checkbox_customwallpaper.Checked = true;
            }
            else if(customwallpaper == "0")
            {
                checkbox_customwallpaper.Checked = false;
            }
            else
            {
                checkbox_customwallpaper.Checked = false;
            }
            #endregion customwallpaper
            LoadScreens();
            
        }

            private void ScreenList_SelectedIndexChanged(object sender, EventArgs e)
            {
                UpdateJsonFile("SelectedScreen", ScreenList.SelectedIndex.ToString());
            }

            private void LoadScreens()
            {
                // Populate the ComboBox with available screens
                foreach (var screen in Screen.AllScreens)
                {
                    ScreenList.Items.Add(screen.DeviceName);
                }

                string display = Readconfig("ScreenBool");
                if (display == "1")
                {
                    SetScreenCheckBox.Checked = true;
                }
                else
                {
                    SetScreenCheckBox.Checked = false;
                }
            }

        private void guna2ComboBox_displayfusionprofil_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Path to the registry where profiles are stored
            string registryPath = @"Software\Binary Fortress Software\DisplayFusion\MonitorConfig";

            // Get the currently selected profile name from the ComboBox
            string selectedProfileName = guna2ComboBox_displayfusionprofil.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedProfileName))
            {
                Console.WriteLine("No profile selected.");
                return;
            }

            try
            {
                // Open the registry key
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath))
                {
                    if (key != null)
                    {
                        // Iterate through all subkeys (profiles)
                        foreach (string profileName in key.GetSubKeyNames())
                        {
                            using (RegistryKey profileKey = key.OpenSubKey(profileName))
                            {
                                if (profileKey != null)
                                {
                                    // Read the profile name from the registry
                                    string name = profileKey.GetValue("Name")?.ToString();

                                    // Check if the name matches the selected profile name
                                    if (name == selectedProfileName)
                                    {
                                        string audioPlaybackMM = profileKey.GetValue("AudioPlaybackMM")?.ToString();
                                        string monitorJson = profileKey.GetValue("Monitor0_Json")?.ToString();

                                        // Parse the JSON data
                                        if (!string.IsNullOrEmpty(monitorJson))
                                        {
                                            var monitorData = JObject.Parse(monitorJson);
                                            string makeModel = monitorData["UniqueID"]?["MakeModel"]?.ToString();
                                            string dpiScalingPercent = monitorData["DpiScalingPercent"]?.ToString();

                                            // Output the profile data to the console
                                            Console.WriteLine($"Profile Name: {name}");
                                            Console.WriteLine($"Audio Playback MM: {audioPlaybackMM}");
                                            Console.WriteLine($"Make Model: {makeModel}");
                                            Console.WriteLine($"DPI Scaling Percent: {dpiScalingPercent}");
                                            Console.WriteLine(new string('-', 30));

                                            // Additional processing can be added here if needed
                                            df_monitorname.Text = makeModel;
                                            df_Audio_model.Text = makeModel;
                                            df_Audio_scaling.Text = dpiScalingPercent;
                                            df_Audio.Text = audioPlaybackMM;


                                        }

                                        // Break the loop as the profile has been found
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("The registry path was not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle errors during registry access
                Console.WriteLine($"Error while reading the registry: {ex.Message}");
            }

           




        }

        private void add_config_df_Click(object sender, EventArgs e)
        {
            if (df_gcmstart_checkbox.Checked == true && df_gcmend_checkbox.Checked == false )
            {
                //by start
                UpdateJsonFile("dfgcmstart", guna2ComboBox_displayfusionprofil.Text);
                Task.Run(() =>
                {
                    add_config_df.Text = "Saved profile for START....";
                    Task.Delay(3000).Wait(); // Pause for 3 seconds
                    add_config_df.Text = "Save config";
                });

            }
            else if (df_gcmend_checkbox.Checked == true && df_gcmstart_checkbox.Checked == false ) //end checked
            {
                // by end
                UpdateJsonFile("dfgcmend", guna2ComboBox_displayfusionprofil.Text);
                Task.Run(() =>
                {
                    add_config_df.Text = "Saved profile for END....";
                    Task.Delay(3000).Wait(); // Pause for 3 seconds
                    add_config_df.Text = "Save config";
                });

            }
            else
            {
                MessageBox.Show("Please select only one checkbox and profil.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void aktivate_df_function_CheckedChanged(object sender, EventArgs e)
        {
            if (aktivate_df_function.Checked == true)
            {
                UpdateJsonFile("displayfusion", "1");
                panel_df_mainpanel.Enabled = true;
            }
            else
            {
                UpdateJsonFile("displayfusion", "0");
                panel_df_mainpanel.Enabled = false;
            }
          
          
        }

        private void deinstall_displayfusion_Click(object sender, EventArgs e)
        {
            // open "Install Apps" in Windows-Settings
            Process.Start(new ProcessStartInfo("ms-settings:appsfeatures"));
        }

        private void download_displayfusion_Click(object sender, EventArgs e)
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

        private void checkbox_customwallpaper_CheckedChanged(object sender, EventArgs e)
        {
            if (checkbox_customwallpaper.Checked == true)
            {
                UpdateJsonFile("customwallpaper", "1");
            }
            else
            {
                UpdateJsonFile("customwallpaper", "0");
            }
        }

        private void guna2TextBox_gcmwallpaper_TextChanged(object sender, EventArgs e)
        {
            UpdateJsonFile("customwallpaperpath", guna2TextBox_gcmwallpaper.Text);
        }
    }


}

