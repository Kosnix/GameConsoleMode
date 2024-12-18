using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IO = System.IO;
using System.Reflection;

namespace Settings
{
    public partial class launchers : Form
    {
        private bool isUpdating = false;
        public launchers()
        {
            InitializeComponent();
            SteamAddress.Text = Readconfig("SteamPath");
            if (Readconfig("Launcher") == "steam")
            {
                ChooseSteam.Checked = true;
            }
            //Playnite//
            PlayniteAddress.Text = Readconfig("PlaynitePath");
            if (Readconfig("Launcher") == "Playnite.FullscreenApp")
            {
                ChoosePlaynite.Checked = true;
            }
            //OtherLauncher
            OtherLauncherAddress.Text = Readconfig("OtherLauncherPath");
            OtherLauncherParameterTextBox.Text = Readconfig("OtherLauncherParameter");
            if (Path.GetFileName(Readconfig("OtherLauncherPath")).Length > 4)
            {
                OtherLauncherLabel.Text = Path.GetFileName(Readconfig("OtherLauncherPath"));
                OtherLauncherLabel.Text = OtherLauncherLabel.Text.Substring(0, OtherLauncherLabel.Text.Length - 4);
            }

            if (Readconfig("Launcher") == "Other")
            {
                ChooseOtherLauncher.Checked = true;
            }

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
                    throw new FileNotFoundException("Le fichier settings.json est introuvable.");
                }

                // Lire le contenu du fichier
                string json = IO.File.ReadAllText(jsonFilePath);

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
                using (StreamWriter File = IO.File.CreateText(jsonFilePath))
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

        private void ChangeSteamAddress_Click_1(object sender, EventArgs e)
        {
            // Créer une nouvelle instance de OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Configurer les options de la boîte de dialogue (facultatif)
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Executable files (steam.exe)|steam.exe|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            // Afficher la boîte de dialogue et vérifier si l'utilisateur a sélectionné un fichier
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Récupérer le chemin complet du fichier sélectionné
                string filePath = openFileDialog.FileName;

                // Afficher le chemin du fichier dans une boîte de message (ou faites ce que vous voulez avec ce chemin)
                //initialize Forms
                UpdateJsonFile("SteamPath", filePath); // Method
            }
        }

        private void launchers_Load(object sender, EventArgs e)
        {

        }

        private void ChooseSteam_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;
            isUpdating = true;

            if (ChooseSteam.Checked)
            {
                ChoosePlaynite.Checked = false;
                ChooseOtherLauncher.Checked = false;
                UpdateJsonFile("Launcher", "steam");
            }
            else
            {
                ChooseSteam.Checked = true;
            }

            isUpdating = false;
        }

        private void ChoosePlaynite_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;
            isUpdating = true;

            if (ChoosePlaynite.Checked)
            {
                ChooseSteam.Checked = false;
                ChooseOtherLauncher.Checked = false;
                UpdateJsonFile("Launcher", "Playnite.FullscreenApp");
            }
            else
            {
                ChoosePlaynite.Checked = true;
            }

            isUpdating = false;
        }

        private void ChooseOtherLauncher_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;
            isUpdating = true;

            if (ChooseOtherLauncher.Checked)
            {
                ChooseSteam.Checked = false;
                ChoosePlaynite.Checked = false;
                UpdateJsonFile("Launcher", "Other");
            }
            else
            {
                ChooseOtherLauncher.Checked = true;
            }

            isUpdating = false;
        }

        private void ChangePlayniteAddress_Click(object sender, EventArgs e)
        {
            // Créer une nouvelle instance de OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Configurer les options de la boîte de dialogue
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Executable files|Playnite.FullscreenApp.exe";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            // Afficher la boîte de dialogue et vérifier si l'utilisateur a sélectionné un fichier
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Récupérer le chemin complet du fichier sélectionné
                string filePath = openFileDialog.FileName;

                // Vérifier si le fichier sélectionné est bien le fichier souhaité
                if (Path.GetFileName(filePath) == "Playnite.FullscreenApp.exe")
                {
                    // Fichier sélectionné correct, faire ce que vous voulez avec le chemin
                    UpdateJsonFile("PlaynitePath", filePath);
                    PlayniteAddress.Text = Readconfig("PlaynitePath");
                }
                else
                {
                    // Fichier sélectionné incorrect, afficher un message d'erreur
                    MessageBox.Show("Veuillez sélectionner uniquement le fichier 'Playnite.FullscreenApp.exe'.", "Fichier incorrect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ChooseOtherLauncher_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isUpdating) return;
            isUpdating = true;

            if (ChooseOtherLauncher.Checked)
            {
                ChooseSteam.Checked = false;
                ChoosePlaynite.Checked = false;
                UpdateJsonFile("Launcher", "Other");
            }
            else
            {
                ChooseOtherLauncher.Checked = true;
            }

            isUpdating = false;
        }

        private void ChangeOtherLauncherAddress_Click(object sender, EventArgs e)
        {
            // Créer une nouvelle instance de OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Configurer les options de la boîte de dialogue
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Executable files (*.exe)|*.exe";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            // Afficher la boîte de dialogue et vérifier si l'utilisateur a sélectionné un fichier
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Récupérer le chemin complet du fichier sélectionné
                string filePath = openFileDialog.FileName;

                // Vérifier si le fichier sélectionné est bien le fichier souhaité

                // Fichier sélectionné correct, faire ce que vous voulez avec le chemin
                UpdateJsonFile("OtherLauncherPath", filePath);
                OtherLauncherAddress.Text = Readconfig("OtherLauncherPath");
                OtherLauncherLabel.Text = Path.GetFileName(Readconfig("OtherLauncherPath"));
                OtherLauncherLabel.Text = OtherLauncherLabel.Text.Substring(0, OtherLauncherLabel.Text.Length - 4);
            }
        }

        private void ChoosePlaynite_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isUpdating) return;
            isUpdating = true;

            if (ChoosePlaynite.Checked)
            {
                ChooseSteam.Checked = false;
                ChooseOtherLauncher.Checked = false;
                UpdateJsonFile("Launcher", "Playnite.FullscreenApp");
            }
            else
            {
                ChoosePlaynite.Checked = true;
            }

            isUpdating = false;
        }

        
            private void OtherLauncherParameterTextBox_TextChanged(object sender, EventArgs e)
            {
                UpdateJsonFile("OtherLauncherParameter", OtherLauncherParameterTextBox.Text);
            }
        
    }
}
