using IWshRuntimeLibrary;
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
    public partial class startup : Form
    {
        public startup()
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


        private void startup_Load(object sender, EventArgs e)
        {
            //Intro video//
            if (Readconfig("IntroBool") == "1")
            {
                IntroCheckBox.Checked = true;
            }
            if (Readconfig("IntroMuteBool") == "1")
            {
                MuteIntroCheckBox.Checked = true;
            }
            IntroAddress.Text = Readconfig("IntroPath");
            //GamePad//
            if (CheckShortcutPresence())
            {
                usecontrollershorts.Checked = true;
            }

        }
        private bool CheckShortcutPresence()
        {
            string shortcutName = "GameConsoleModeGamepad.lnk";

            if (IsShortcutPresent(shortcutName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool IsShortcutPresent(string shortcutName)
        {
            // 1. Récupérer le chemin du dossier "Startup" de l'utilisateur
            string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            // 2. Construire le chemin complet du raccourci
            string shortcutPath = Path.Combine(startupFolderPath, shortcutName);

            // 3. Vérifier si le fichier existe
            return System.IO.File.Exists(shortcutPath);
        }

      

        private void IntroCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (IntroCheckBox.Checked == true)
            {
                UpdateJsonFile("IntroBool", "1");
            }
            else
            {
                UpdateJsonFile("IntroBool", "0");
            }
        }

        private void MuteIntroCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            {
                if (MuteIntroCheckBox.Checked == true)
                {
                    UpdateJsonFile("IntroMuteBool", "1");
                }
                else
                {
                    UpdateJsonFile("IntroMuteBool", "0");
                }
            }
        }

        private void ChangeIntroAddress_Click(object sender, EventArgs e)
        {
            // Créer une nouvelle instance de OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Configurer les options de la boîte de dialogue
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Fichiers vidéo (*.mp4;*.avi;*.mov;*.mkv)|*.mp4;*.avi;*.mov;*.mkv|Tous les fichiers (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            // Afficher la boîte de dialogue et vérifier si l'utilisateur a sélectionné un fichier
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Récupérer le chemin complet du fichier sélectionné
                string filePath = openFileDialog.FileName;

                // Vérifier si le fichier sélectionné est bien le fichier souhaité

                // Fichier sélectionné correct, faire ce que vous voulez avec le chemin
                UpdateJsonFile("IntroPath", filePath);
                IntroAddress.Text = Readconfig("IntroPath");
            }
        }

        private void usecontrollershorts_CheckedChanged(object sender, EventArgs e)
        {
            // 1. Récupérer le chemin du dossier "Startup" de l'utilisateur
            string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            // 2. Définir le chemin complet du raccourci à créer
            string shortcutPath = Path.Combine(startupFolderPath, "GameConsoleModeGamepad.lnk");

            // 3. Spécifier le chemin de l'application ou du fichier cible
            string targetPath = Path.Combine(exeFolder(), "GameConsoleModeGamepad.exe");

            // Vérification si l'utilisateur a coché ou décoché la case
            if (usecontrollershorts.Checked)
            {
                // Créer le raccourci
                try
                {
                    // Si le raccourci existe déjà, le recréer pour éviter tout problème
                    if (System.IO.File.Exists(shortcutPath))
                    {
                        System.IO.File.Delete(shortcutPath);
                    }

                    // Créer une instance de WshShell
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

                    // Configurer le raccourci
                    shortcut.TargetPath = targetPath; // Chemin de l'application ou fichier
                    shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath); // Répertoire de travail
                    shortcut.Description = "Lance GameConsoleModeGamepad au démarrage"; // Description
                    shortcut.IconLocation = targetPath; // Icône associée (facultatif)

                    // Sauvegarder le raccourci
                    shortcut.Save();

                    Console.WriteLine($"Raccourci créé dans le dossier de démarrage : {shortcutPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la création du raccourci : {ex.Message}");
                }
            }
            else
            {
                // Supprimer le raccourci si la case est décochée
                try
                {
                    if (System.IO.File.Exists(shortcutPath))
                    {
                        System.IO.File.Delete(shortcutPath);
                        Console.WriteLine($"Raccourci supprimé du dossier de démarrage : {shortcutPath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la suppression du raccourci : {ex.Message}");
                }
            }
        }
    }
}
