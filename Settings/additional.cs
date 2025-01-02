using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Squirrel;
using NAudio.CoreAudioApi;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi;

namespace Settings
{
    public partial class additional : Form
    {
        public additional()
        {
            InitializeComponent();
            LoadStartList();
            controller = new CoreAudioController();
        }

        #region autoupdate

        #endregion autoupdate

        #region need variable and methodes
        private CoreAudioController controller;



        static string exeFolder()
        {
            string cheminExecutable = Assembly.GetExecutingAssembly().Location;
            string dossierExecutable = Path.GetDirectoryName(cheminExecutable);
            return dossierExecutable;
        }


        #region function start_and_end

        private void LoadStartList()
        {
            //start
            try
            {
                List<string> startList = GetList("start");

                // Vorhandene Einträge in der ListView löschen
                gcmstart_list.Items.Clear();
             
                // ListView mit den Einträgen füllen
                foreach (var item in startList)
                {
                    gcmstart_list.Items.Add(item);
                }

                Console.WriteLine("Start-Liste erfolgreich geladen.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Start-Liste: {ex.Message}");
            }
            //end
            try
            {
                List<string> endList = GetList("end");

                // Vorhandene Einträge in der ListView löschen
                gcmend_list.Items.Clear();

                // ListView mit den Einträgen füllen
                foreach (var item in endList)
                {
                    gcmend_list.Items.Add(item);
                }

                Console.WriteLine("Start-Liste erfolgreich geladen.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Start-Liste: {ex.Message}");
            }
        }


        public static List<string> GetList(string key)
        {
            string filePath = ConfigFilePath();
            InitializeConfigFile();

            try
            {
                string jsonContent = File.ReadAllText(filePath);
                JObject jsonObj = JObject.Parse(jsonContent);

                if (jsonObj[key] is JArray array)
                {
                    return array.ToObject<List<string>>();
                }
                else
                {
                    Console.WriteLine($"Die Schlüssel '{key}' ist keine Liste.");
                    return new List<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Lesen der Liste: {ex.Message}");
                return new List<string>();
            }
        }

        static string ConfigFilePath()
        {
            string exeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(exeFolder, "start_and_end_config.json");
        }

        // Initialisiert die JSON-Datei, falls sie nicht existiert
        static void InitializeConfigFile()
        {
            string filePath = ConfigFilePath();

            if (!File.Exists(filePath))
            {
                var defaultConfig = new JObject
                {
                    ["start"] = new JArray(),
                    ["end"] = new JArray()
                };

                File.WriteAllText(filePath, defaultConfig.ToString(Formatting.Indented));
            }
        }

        // Fügt einen Eintrag (Start oder End) hinzu
        public static void AddEntry(string key, string value)
        {
            string filePath = ConfigFilePath();
            InitializeConfigFile();

            try
            {
                string jsonContent = File.ReadAllText(filePath);
                JObject jsonObj = JObject.Parse(jsonContent);

                if (jsonObj[key] is JArray array)
                {
                    if (!array.Contains(value))
                    {
                        array.Add(value);
                        Console.WriteLine($"Eintrag '{value}' wurde zur Liste '{key}' hinzugefügt.");
                    }
                    else
                    {
                        Console.WriteLine($"Eintrag '{value}' existiert bereits in der Liste '{key}'.");
                    }
                }
                else
                {
                    Console.WriteLine($"Die Schlüssel '{key}' ist keine Liste.");
                }

                File.WriteAllText(filePath, jsonObj.ToString(Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Hinzufügen: {ex.Message}");
            }
        }

        // Entfernt einen Eintrag aus der Liste
        public static void RemoveEntry(string key, string value)
        {
            string filePath = ConfigFilePath(); // Pfad zur JSON-Datei
            InitializeConfigFile(); // Stellt sicher, dass die Datei existiert

            try
            {
                // JSON-Datei einlesen
                string jsonContent = File.ReadAllText(filePath);
                JObject jsonObj = JObject.Parse(jsonContent);

                // Prüfen, ob der Schlüssel existiert und eine Liste ist
                if (jsonObj[key] is JArray array)
                {
                    // Kopiere die Liste und entferne manuell alle Vorkommen
                    var itemsToRemove = array.Where(item => item.ToString() == value).ToList();

                    foreach (var item in itemsToRemove)
                    {
                        array.Remove(item);
                    }

                    if (itemsToRemove.Count > 0)
                    {
                        Console.WriteLine($"Eintrag '{value}' wurde {itemsToRemove.Count} Mal aus der Liste '{key}' entfernt.");
                    }
                    else
                    {
                        Console.WriteLine($"Eintrag '{value}' wurde nicht in der Liste '{key}' gefunden.");
                    }
                }
                else
                {
                    Console.WriteLine($"Die Schlüssel '{key}' ist keine Liste oder existiert nicht.");
                }

                // Änderungen zurückschreiben
                File.WriteAllText(filePath, jsonObj.ToString(Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Entfernen des Eintrags: {ex.Message}");
            }
        }

        #endregion funktion start_and_end

        //Audio
        private MMDeviceEnumerator deviceEnumerator;  // Deklaration auf Klassenebene
        private MMDevice selectedDevice;

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
        private void HideMouse_CheckedChanged(object sender, EventArgs e)
        {
            if (HideMouse.Checked)
            {
                UpdateJsonFile("HideMouse", "1");
            }
            else
            {
                UpdateJsonFile("HideMouse", "0");
            }
        }

        public void UAC_Color()
        {
            object consentPromptBehaviorAdmin = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", null);
            object PromptOnSecureDesktop = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", null);
            if (consentPromptBehaviorAdmin != null && (int)consentPromptBehaviorAdmin == 5 && PromptOnSecureDesktop != null && (int)PromptOnSecureDesktop == 1)
            {
                EnableUAC.BackColor = Color.LightCoral;
            }
            else
            {
                EnableUAC.BackColor = SystemColors.InactiveCaption;
            }
        }

        private void additional_Load(object sender, EventArgs e)
        {
            //hidemouse//
            if (Readconfig("HideMouse") == "1")
            {
                HideMouse.Checked = true;
            }
            //Audio//
            if (Readconfig("AudioBool") == "1")
            {
                AudioVolumeCheckBox.Checked = true;
            }
            SelectedAudioVolumeLabel.Text = String.Concat(Readconfig("AudioVolume"), "%");
            VolumeTrackBar.Value = int.Parse(Readconfig("AudioVolume"));



            //Audio
            // Initialisiere die Geräte-Auflistung
            PopulatePlaybackDevices();
            LoadScripts();
        }

        private void AudioVolumeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AudioVolumeCheckBox.Checked == true)
            {
                UpdateJsonFile("AudioBool", "1");
            }
            else
            {
                UpdateJsonFile("AudioBool", "0");
            }
        }

        private void VolumeTrackBar_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateJsonFile("AudioVolume", VolumeTrackBar.Value.ToString());
            SelectedAudioVolumeLabel.Text = String.Concat(VolumeTrackBar.Value.ToString(), "%");
        }

        private void DisableUAC_Click(object sender, EventArgs e)
        {
            try
            {
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 0);

                MessageBox.Show("UAC has been successfully disabled.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while disabling UAC: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnableUAC_Click(object sender, EventArgs e)
        {
            try
            {
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 5);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 1);

                MessageBox.Show("UAC has been successfully enabled.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exception("Unauthorized access: you need to run this program as an administrator.");
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to restore default UAC settings: " + ex.Message);
            }
        }

        private void ad_program_start_add_Click(object sender, EventArgs e)
        {

            // OpenFileDialog initialisieren
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Dialogeinstellungen
                openFileDialog.Filter = "Programme (*.exe)|*.exe|Alle Dateien (*.*)|*.*";
                openFileDialog.Title = "Wähle eine Datei aus";

                // Dialog anzeigen und prüfen, ob der Benutzer eine Datei ausgewählt hat
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Ausgewählten Dateipfad in die Textbox schreiben
                    AddEntry("start", openFileDialog.FileName);
                    LoadStartList();
                }
            }

            
        }

        private void ad_program_start_clear_Click(object sender, EventArgs e)
        {
            

            // OpenFileDialog initialisieren
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Dialogeinstellungen
                openFileDialog.Filter = "Programme (*.exe)|*.exe|Alle Dateien (*.*)|*.*";
                openFileDialog.Title = "Wähle eine Datei aus";

                // Dialog anzeigen und prüfen, ob der Benutzer eine Datei ausgewählt hat
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Ausgewählten Dateipfad in die Textbox schreiben
                    RemoveEntry("start", openFileDialog.FileName);
                    LoadStartList();
                }
            }
        }

        private void ad_program_end_add_Click(object sender, EventArgs e)
        {
            // OpenFileDialog initialisieren
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Dialogeinstellungen
                openFileDialog.Filter = "Programme (*.exe)|*.exe|Alle Dateien (*.*)|*.*";
                openFileDialog.Title = "Wähle eine Datei aus";

                // Dialog anzeigen und prüfen, ob der Benutzer eine Datei ausgewählt hat
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Ausgewählten Dateipfad in die Textbox schreiben
                    AddEntry("end", openFileDialog.FileName);
                    LoadStartList();
                }
            }
        }

        private void ad_program_end_clear_Click(object sender, EventArgs e)
        {

            // OpenFileDialog initialisieren
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Dialogeinstellungen
                openFileDialog.Filter = "Programme (*.exe)|*.exe|Alle Dateien (*.*)|*.*";
                openFileDialog.Title = "Wähle eine Datei aus";

                // Dialog anzeigen und prüfen, ob der Benutzer eine Datei ausgewählt hat
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Ausgewählten Dateipfad in die Textbox schreiben
                    RemoveEntry("end", openFileDialog.FileName);
                    LoadStartList();
                }
            }
        }

        #region Custom Scripts

        //Function
        public void RemoveSelectedScript()
        {
            // Check if an item is selected in the start ListView
            if (listView_added_scripts_start.SelectedItems.Count > 0)
            {
                var selectedItem = listView_added_scripts_start.SelectedItems[0];
                string filePath = selectedItem.Tag as string;

                if (filePath != null && File.Exists(filePath))
                {
                    File.Delete(filePath);
                    MessageBox.Show($"File '{Path.GetFileName(filePath)}' has been deleted from start_scripts.", "File Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    listView_added_scripts_start.Items.Remove(selectedItem);
                }
            }
            // Check if an item is selected in the end ListView
            else if (listView_added_scripts_end.SelectedItems.Count > 0)
            {
                var selectedItem = listView_added_scripts_end.SelectedItems[0];
                string filePath = selectedItem.Tag as string;

                if (filePath != null && File.Exists(filePath))
                {
                    File.Delete(filePath);
                    MessageBox.Show($"File '{Path.GetFileName(filePath)}' has been deleted from end_scripts.", "File Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    listView_added_scripts_end.Items.Remove(selectedItem);
                }
            }
            else
            {
                MessageBox.Show("No script selected. Please select a script to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public void LoadScripts()
        {
            // Get the directory where the program is executed
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Define the folder names
            string[] scriptFolders = { "start_scripts", "end_scripts" };

            // Clear the ListViews before adding new items
            listView_added_scripts_start.Items.Clear();
            listView_added_scripts_end.Items.Clear();

            foreach (var folder in scriptFolders)
            {
                // Build the full path to the folder
                string folderPath = Path.Combine(currentDirectory, folder);

                // Skip if the folder does not exist
                if (!Directory.Exists(folderPath))
                {
                    continue;
                }

                // Get all .ps1 files in the folder
                var ps1Files = Directory.GetFiles(folderPath, "*.ps1");

                foreach (var file in ps1Files)
                {
                    // Extract the file name
                    string fileName = Path.GetFileName(file);

                    // Add the file to the appropriate ListView
                    if (folder == "start_scripts")
                    {
                        ListViewItem item = new ListViewItem(fileName)
                        {
                            Tag = file // Store the file path in the Tag for later use
                        };
                        listView_added_scripts_start.Items.Add(item);
                    }
                    else if (folder == "end_scripts")
                    {
                        ListViewItem item = new ListViewItem(fileName)
                        {
                            Tag = file // Store the file path in the Tag for later use
                        };
                        listView_added_scripts_end.Items.Add(item);
                    }
                }
            }
        }

        public void CreateFolderAndFile(RichTextBox richTextBox,string art)
        {
            if (art == "start")
            {
                try
                {
                    // where the programm starts
                    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    // new directory
                    string folderName = Path.Combine(currentDirectory, "start_scripts");
                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }

                    // path for PS1-Datei
                    string ps1FilePath = Path.Combine(folderName, script_name.Text + ".ps1");

                    string scriptContent = richTextBox.Text;

                    // write .ps1-Datei
                    File.WriteAllText(ps1FilePath, scriptContent);

                    MessageBox.Show($"Script was saved successfully: {ps1FilePath}", "successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.Start(folderName);
                    LoadScripts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when creating the script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (art == "end")
            {
                //end
                try
                {
                    // where the programm starts
                    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    // new directory
                    string folderName = Path.Combine(currentDirectory, "end_scripts");
                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }

                    // path for PS1-Datei
                    string ps1FilePath = Path.Combine(folderName, script_name.Text + ".ps1");

                    string scriptContent = richTextBox.Text;

                    // write .ps1-Datei
                    File.WriteAllText(ps1FilePath, scriptContent);

                    MessageBox.Show($"Script was saved successfully: {ps1FilePath}", "successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.Start(folderName);
                    LoadScripts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when creating the script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }else if (art == "both") //start and end
            {
                try
                {
                    // where the programm starts
                    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    // new directory
                    string folderName = Path.Combine(currentDirectory, "start_scripts");
                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }

                    // path for PS1-Datei
                    string ps1FilePath = Path.Combine(folderName, script_name.Text + ".ps1");

                    string scriptContent = richTextBox.Text;

                    // write .ps1-Datei
                    File.WriteAllText(ps1FilePath, scriptContent);

                    MessageBox.Show($"Script was saved successfully: {ps1FilePath}", "successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.Start(folderName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when creating the script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                //end
                try
                {
                    // where the programm starts
                    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    // new directory
                    string folderName = Path.Combine(currentDirectory, "end_scripts");
                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }

                    // path for PS1-Datei
                    string ps1FilePath = Path.Combine(folderName, script_name.Text + ".ps1");

                    string scriptContent = richTextBox.Text;

                    // write .ps1-Datei
                    File.WriteAllText(ps1FilePath, scriptContent);

                    MessageBox.Show($"Script was saved successfully: {ps1FilePath}", "successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.Start(folderName);
                    LoadScripts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when creating the script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void add_script_Click(object sender, EventArgs e)
        {
            if (checkbox_scripts_start.Checked == true && checkbox_scripts_end.Checked == false)//start
            {
                CreateFolderAndFile(textbox_script, "start");

            }
            else if(checkbox_scripts_end.Checked == true && checkbox_scripts_start.Checked == false) //end
            {
                CreateFolderAndFile(textbox_script, "end");
            }
            else if (checkbox_scripts_end.Checked == true && checkbox_scripts_start.Checked == true) // start and end
            {
                CreateFolderAndFile(textbox_script, "both"); // start and end
            }
            else if (checkbox_scripts_end.Checked == false && checkbox_scripts_start.Checked == false)
            {
                MessageBox.Show($"Missing checkbox input (START/END/BOTH)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void remove_scripts_Click(object sender, EventArgs e)
        {
            RemoveSelectedScript();
        }
       
        //Checkbox
        private void checkbox_scripts_start_CheckedChanged(object sender, EventArgs e)
        {
          
        }

        private void checkbox_scripts_end_CheckedChanged(object sender, EventArgs e)
        {
        
        }
   

        private void checkbox_scripts_start_CheckStateChanged(object sender, EventArgs e)
        {

        }

        private void checkbox_scripts_end_CheckStateChanged(object sender, EventArgs e)
        {

        }

        private void checkbox_scripts_start_Click(object sender, EventArgs e)
        {

        }

        private void checkbox_scripts_end_Click(object sender, EventArgs e)
        {

        }


        private void script_name_Click(object sender, EventArgs e)
        {
            if (script_name.Text == "Insert Script Name")
            {
                script_name.Text = "";
            }
            
           

        }

        private void textbox_script_TextChanged(object sender, EventArgs e)
        {
            if (textbox_script.Text == "")
            {
                add_script.Enabled = false;
            }
            else
            {
                add_script.Enabled = true;
            }
        }
       

        private void listView_added_scripts_start_SelectedIndexChanged(object sender, EventArgs e)
        {
           if( listView_added_scripts_start.SelectedItems.Count > 0)
            {
                remove_scripts.Enabled = true;
            }
            else
            {
                remove_scripts.Enabled = false;
            }
        }

        private void listView_added_scripts_end_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView_added_scripts_end.SelectedItems.Count > 0)
            {
                remove_scripts.Enabled = true;
            }
            else
            {
                remove_scripts.Enabled = false;
            }
        }
        #endregion Custom Script

        #region advanced audiosettings
        //function
        private async void PopulatePlaybackDevices()
        {
            try
            {
                // Get all active playback devices
                var devices = await controller.GetPlaybackDevicesAsync();

                // Clear the ComboBox before populating
                guna2ComboBox_playbackdevice.Items.Clear();

                // Add each device's friendly name to the ComboBox
                foreach (var device in devices)
                {
                    guna2ComboBox_playbackdevice.Items.Add(device.FullName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while populating playback devices: {ex.Message}");
            }
        }
        private async Task SetPlaybackDeviceFromJsonAsync()
        {
            try
            {
                // Read the device ID from the JSON configuration
                string deviceId = Readconfig("audioplaybackdevice");

                if (string.IsNullOrEmpty(deviceId))
                {
                    MessageBox.Show("No playback device ID found in configuration.");
                    return;
                }

                // Convert the string ID to a GUID
                var deviceGuid = new Guid(deviceId);

                // Retrieve the device using its ID
                var device = await controller.GetDeviceAsync(deviceGuid);

                if (device == null)
                {
                    MessageBox.Show("Playback device not found.");
                    return;
                }

                // Set the device as the default playback device
                await device.SetAsDefaultAsync();
            }
            catch (Exception ex)
            {

            }
        }







        private void guna2ComboBox_playbackdevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Get the list of playback devices
                var devices = controller.GetPlaybackDevices();

                // Ensure devices is a collection and check its count
                int deviceCount = devices.Count(); // Use LINQ's Count() if it's an IEnumerable

                // Validate the selected index
                int selectedIndex = guna2ComboBox_playbackdevice.SelectedIndex;
                if (selectedIndex < 0 || selectedIndex >= deviceCount)
                {
                    MessageBox.Show("Invalid selection. Please select a valid playback device.");
                    return;
                }

                // Get the selected device
                var selectedDevice = devices.ElementAt(selectedIndex); // Use LINQ's ElementAt

                // Save the device ID to the JSON file
                UpdateJsonFile("audioplaybackdevice", selectedDevice.Id.ToString());

                MessageBox.Show($"Playback device '{selectedDevice.FullName}' saved to configuration.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            SetPlaybackDeviceFromJsonAsync();
        }

        #endregion advanced audiosettings
    }

 
}

