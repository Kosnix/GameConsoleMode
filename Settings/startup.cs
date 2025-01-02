using IWshRuntimeLibrary;
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
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Windows.Forms;
using File = System.IO.File;

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
           
            // Check if Joyxoff is already installed
            string joyxoffExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Joyxoff", "Joyxoff.exe");
            if (File.Exists(joyxoffExePath))
            {
                guna2Chip_joyxoffinstall_status.Text = "INSTALLED";
                guna2Chip_joyxoffinstall_status.FillColor = Color.Green;
                guna2Chip_joyxoffinstall_status.BorderColor = Color.Green;
            }
            else
            {
                guna2Chip_joyxoffinstall_status.Text = "NOT INSTALLED";
                guna2Chip_joyxoffinstall_status.FillColor = Color.Brown;
                guna2Chip_joyxoffinstall_status.BorderColor = Color.Brown;
            }

            string cssloaderExePath = @"C:\Program Files\CSSLoader Desktop\CSSLoader Desktop.exe";
            if (File.Exists(cssloaderExePath))
            {
                guna2Chip_cssloader_install_status.Text = "INSTALLED";
                guna2Chip_cssloader_install_status.FillColor = Color.Green;
                guna2Chip_cssloader_install_status.BorderColor = Color.Green;
            }
            else
            {
                guna2Chip_cssloader_install_status.Text = "NOT INSTALLED";
                guna2Chip_cssloader_install_status.FillColor = Color.Brown;
                guna2Chip_cssloader_install_status.BorderColor = Color.Brown;
            }


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

        // Hilfsmethode: Prüft, ob der Dienst existiert
        private bool ServiceExists(string serviceName)
        {
            try
            {
                ServiceController sc = new ServiceController(serviceName);
                ServiceControllerStatus status = sc.Status; // Löst eine Ausnahme aus, wenn der Dienst nicht existiert
                return true;
            }
            catch
            {
                return false;
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
        #region mouse control
        //methodes

        private void ad_program_start_add_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if Joyxoff is already installed
                string joyxoffExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Joyxoff", "Joyxoff.exe");
                if (File.Exists(joyxoffExePath))
                {
                    MessageBox.Show("Joyxoff is already installed and will now start.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show($"The file {msiPath} was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("Installation completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Check if Joyxoff is already installed
                    joyxoffExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Joyxoff", "Joyxoff.exe");
                    if (File.Exists(joyxoffExePath))
                    {
                        guna2Chip_joyxoffinstall_status.Text = "INSTALLED";
                        guna2Chip_joyxoffinstall_status.FillColor = Color.Green;
                        guna2Chip_joyxoffinstall_status.BorderColor = Color.Green;
                        Process.Start(joyxoffExePath);
                        return;
                    }
                    else
                    {
                        guna2Chip_joyxoffinstall_status.Text = "NOT INSTALLED";
                        guna2Chip_joyxoffinstall_status.FillColor = Color.Brown;
                        guna2Chip_joyxoffinstall_status.BorderColor = Color.Brown;

                    }
                }
                else
                {
                    MessageBox.Show($"Installation failed. Error code: {process.ExitCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    guna2Chip_joyxoffinstall_status.Text = "NOT INSTALLED";
                    guna2Chip_joyxoffinstall_status.FillColor = Color.Brown;
                    guna2Chip_joyxoffinstall_status.BorderColor = Color.Brown;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void guna2Button_joyxoff_uninstall_Click(object sender, EventArgs e)
        {
            // open "Install Apps" in Windows-Settings
            Process.Start(new ProcessStartInfo("ms-settings:appsfeatures"));
        }

        #endregion mouse control

        #region cssloader
        private void CSS_loader_install_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if Joyxoff is already installed
                string cssloaderExePath = @"C:\Program Files\CSSLoader Desktop\CSSLoader Desktop.exe";
                if (File.Exists(cssloaderExePath))
                {
                    MessageBox.Show("CSS LOADER  is already installed and will now start.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show($"The file {msiPath} was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("Installation completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Check if Joyxoff is already installed
                    cssloaderExePath = @"C:\Program Files\CSSLoader Desktop\CSSLoader Desktop.exe";
                    if (File.Exists(cssloaderExePath))
                    {
                        guna2Chip_cssloader_install_status.Text = "INSTALLED";
                        guna2Chip_cssloader_install_status.FillColor = Color.Green;
                        guna2Chip_cssloader_install_status.BorderColor = Color.Green;
                        Process.Start(cssloaderExePath);
                        return;
                    }
                    else
                    {
                        guna2Chip_cssloader_install_status.Text = "NOT INSTALLED";
                        guna2Chip_cssloader_install_status.FillColor = Color.Brown;
                        guna2Chip_cssloader_install_status.BorderColor = Color.Brown;

                    }
                }
                else
                {
                    MessageBox.Show($"Installation failed. Error code: {process.ExitCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    guna2Chip_cssloader_install_status.Text = "NOT INSTALLED";
                    guna2Chip_cssloader_install_status.FillColor = Color.Brown;
                    guna2Chip_cssloader_install_status.BorderColor = Color.Brown;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
      

        private void guna2Button_cssloader_uninstall_Click(object sender, EventArgs e)
        {
            // open "Install Apps" in Windows-Settings
            Process.Start(new ProcessStartInfo("ms-settings:appsfeatures"));
        }
        #endregion cssloader

       
    }
}
    
    
    
  

