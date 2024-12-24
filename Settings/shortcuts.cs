using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using IO = System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Settings
{
    public partial class shortcuts : Form
    {
        public shortcuts()
        {
          
                InitializeComponent(); // standard initialization
                LoadSettings();


        }

        private void LoadSettings()
        {
            //StartGCM//
            if (Readconfig("Shortcut0") == "1")
            {
                start_gcm_CheckBox.Checked = true;
            }
            //Switch Window
            if (Readconfig("Shortcut1") == "1")
            {
                switch_window_CheckBox.Checked = true;
            }
            //Mouse
            if (Readconfig("Shortcut2") == "1")
            {
                mouse_CheckBox.Checked = true;
            }
        }

        private void LoadShortcutView(string relativeImagePath,string text,string maintext)
        {
            // Determine the path based on the project directory
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(baseDir, relativeImagePath);

            if (!File.Exists(fullPath))
            {
                MessageBox.Show("The image was not found: " + fullPath);
                return;
            }

            Image image = Image.FromFile(fullPath);

            // Draw Image to Panel
            picture_controller_layout.Image = image;
            picture_controller_layout.BackgroundImageLayout = ImageLayout.Stretch; // or other layouts like Center, etc.

            label_shortcut_information.Text = text;
            label_shortcut_overview.Text = maintext;
        }

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
            if (!IO.File.Exists(filePath))
            {
                Console.WriteLine($"Le fichier {filePath} n'existe pas.");
                return string.Empty;
            }

            try
            {
                // Lire le contenu du fichier JSON
                string jsonContent = IO.File.ReadAllText(filePath);

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
                if (!IO.File.Exists(jsonFilePath))
                {
                    throw new FileNotFoundException("The settings.json file could not be found.");
                }

                // Lire le contenu du fichier
                string json = IO.File.ReadAllText(jsonFilePath);

                // Vérifier si le contenu du fichier JSON est vide
                if (string.IsNullOrWhiteSpace(json))
                {
                    throw new Exception("The JSON file is empty or contains only spaces.");
                }

                // Analyser le contenu JSON
                JObject jsonObj = JObject.Parse(json);

                // Vérifier si la clé "Settings" existe
                if (jsonObj["Settings"] == null)
                {
                    throw new KeyNotFoundException("The key 'Settings' was not found in the JSON file.");
                }

                // Mettre à jour la valeur de la clé
                jsonObj["Settings"][key] = newValue;

                // Écrire les modifications dans le fichier
                using (StreamWriter File = IO.File.CreateText(jsonFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(File, jsonObj);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON file: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Input/output error: {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error has occurred: {ex.Message}");
            }
        }

        private void shortcut_Load(object sender, EventArgs e)
        {
         
        }

        private void button_switch_window_Click(object sender, EventArgs e)
        {
            LoadShortcutView("Resources/switchwin_shortcut.png", "Switches through the windows, <br>very useful when your launcher window disappears <br> in the background", "SWITCH THROUGH WINDOWS (ALT-TAB)");
        }

        private void button_start_gcm_Click(object sender, EventArgs e)
        {
            LoadShortcutView("Resources/start_gcm_shortcut.png", "This allows you to start gaming mode <br> with your controller alone", "START GAMING CONSOLE MODE");
        }

        private void button_mouse_Click(object sender, EventArgs e)
        {
            LoadShortcutView("Resources/mouse_shortcut.png", "Allows you to control the mouse with the right joystick and click with LB and RB", "MOUSE");
        }

        private void start_gcm_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (start_gcm_CheckBox.Checked == true)
            {
                UpdateJsonFile("Shortcut0", "1");
            }
            else
            {
                UpdateJsonFile("Shortcut0", "0");
            }
        }

        private void switch_window_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (switch_window_CheckBox.Checked == true)
            {
                UpdateJsonFile("Shortcut1", "1");
            }
            else
            {
                UpdateJsonFile("Shortcut1", "0");
            }
        }

        private void mouse_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mouse_CheckBox.Checked == true)
            {
                UpdateJsonFile("Shortcut2", "1");
            }
            else
            {
                UpdateJsonFile("Shortcut2", "0");
            }
        }
    }
}
