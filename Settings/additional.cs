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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Settings
{
    public partial class additional : Form
    {
        public additional()
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
        private void HideMouse_CheckedChanged(object sender, EventArgs e)
        {
            if (HideMouse.Checked)
            {
                UpdateJsonFile("HideMouse", "true");
            }
            else
            {
                UpdateJsonFile("HideMouse", "false");
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
            if (Readconfig("HideMouse") == "true")
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
    }
}
