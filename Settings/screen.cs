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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            string display =  Readconfig("ScreenBool");
            if(display == "1")
            {
                SetScreenCheckBox.Checked = true;
            }
            else
            {
                SetScreenCheckBox.Checked = false;
            }
        }
    }


}
