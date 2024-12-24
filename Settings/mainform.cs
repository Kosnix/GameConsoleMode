using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using IO = System.IO;
using Wsh = IWshRuntimeLibrary;


namespace Settings
{
    public partial class Settings : Form
    {
        private bool isUpdating = false;
        // initialize new Forms
        private launchers _launchers;
        private shortcuts _shortcuts;
        private screen _screen;
        private additional _additional;
        private startup _startup;

        private object InitializeObject(string type)
        {
            switch (type)
            {
                case "launchers":
                    if (_launchers == null)
                        _launchers = new launchers();
                    return _launchers;

                case "shortcut":
                    if (_shortcuts == null)
                        _shortcuts = new shortcuts();
                    return _shortcuts;

                case "screen":
                    if (_screen == null)
                        _screen = new screen();
                    return _screen;

                case "additional":
                    if (_additional == null)
                        _additional = new additional();
                    return _additional;

                case "startup":
                    if (_startup == null)
                        _startup = new startup();
                    return _startup;

                default:
                    throw new ArgumentException("Unbekannter Typ: " + type);
            }
        }




        public Settings()
        {
            AdminVerif();
            InitializeComponent();
            VerifSettings();
            
        }

        static void Quit()
        {
            Application.Exit();
            Environment.Exit(0);
        }

        static bool VerifyFolder()
        {
            string[] requiredFiles = new string[]
            {
        "Guna.UI2.dll"
            };

            foreach (string fileName in requiredFiles)
            {
                if (!VerifyFile(fileName))
                {
                    return false;
                }
            }

            return true;
        }


        static bool VerifyFile(string FileName)
        {
            string filePath = Path.Combine(exeFolder(), FileName);

            // Check if file exists
            if (IO.File.Exists(filePath))
            {
                Console.WriteLine($"{FileName} exists.");
                return true;
            }
            else
            {
                if (FileName == "settings.json")
                {
                    return false;
                }
                else
                {
                    string message = $"{FileName} is missing.\nPlease reinstall the program.";
                    string title = "File Missing";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine(message);
                    return false;
                }

            }
        }

        static void AdminVerif()
        {
            if (!IsAdministrator())
            {
                Console.WriteLine("Restart as admin");
                RestartAsAdministrator();
                return;
            }
        }
        static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        static void RestartAsAdministrator()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                Verb = "runas"
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error restarting application as administrator : " + ex.Message);
            }

            Environment.Exit(0);
        }
        private void VerifSettings()
        {
            string exeFolderPath = exeFolder();
            string settingsFilePath = Path.Combine(exeFolderPath, "settings.json");

            // Vérifier si le fichier settings.json existe
            if (!IO.File.Exists(settingsFilePath))
            {
                // Créer le fichier avec le contenu spécifié
                string defaultSettings = @"{
    ""Settings"": {
        ""HideMouse"": ""0"",
        ""Launcher"": ""steam"",
        ""SteamPath"": ""C:\\Program Files (x86)\\Steam\\steam.exe"",
        ""PlaynitePath"": """",
        ""OtherLauncherPath"": """",
        ""OtherLauncherParameter"": """",
        ""AudioBool"": ""0"",
        ""AudioVolume"": ""100"",
        ""ScreenBool"": ""0"",
        ""SelectedScreen"": """",
        ""IntroBool"": ""0"",
        ""IntroMuteBool"": ""0"",
        ""Shortcut0"": ""1"",
        ""Shortcut1"": ""1"",
        ""Shortcut2"": ""1"",
        ""Shortcut3"": ""1"",
        ""Shortcut4"": ""1"",
        ""Shortcut5"": ""1"",
        ""Shortcut6"": ""1"",
        ""Shortcut7"": ""1"",
        ""Shortcut8"": ""1"",
        ""Shortcut9"": ""1""      
    }
}";


                IO.File.WriteAllText(settingsFilePath, defaultSettings);
                Console.WriteLine("The settings.json file was created successfully.");
            }
            else
            {
                Console.WriteLine("The settings.json file already exists.");
            }
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void otherButton_Click(object sender, EventArgs e)
        {
        }


        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }

        static string Readconfig(string key)
        {
            string filePath = Path.Combine(exeFolder(), "settings.json");

            // Vérifier si le fichier existe
            if (!IO.File.Exists(filePath))
            {
                Console.WriteLine($"The file {filePath} does not exist.");
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
                    Console.WriteLine($"Key '{key}' is set to '{value}'");
                    return value;
                }
                else
                {
                    Console.WriteLine($"The key '{key}' was not found in the configuration.");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading JSON file: {ex.Message}");
                return string.Empty;
            }
        }

        static void StartThisFolderExe(string exeName, string argument)
        {
            string cheminExecutable = Path.Combine(exeFolder(), exeName);

            // Créer une nouvelle instance de Process
            Process process = new Process();

            // Spécifier le chemin de l'exécutable et les arguments
            process.StartInfo.FileName = cheminExecutable;
            process.StartInfo.Arguments = argument;

            try
            {
                // Démarrer le processus
                process.Start();
                Console.WriteLine($"The process {exeName} with {argument} was started successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"The process {exeName} encountered an error:" + e.Message);
            }
        }

        static string exeFolder()
        {
            string cheminExecutable = Assembly.GetExecutingAssembly().Location;
            string dossierExecutable = Path.GetDirectoryName(cheminExecutable);
            return dossierExecutable;
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


        private void Settings_Load(object sender, EventArgs e)
        {

        }

        private void IntroAddress_TextChanged(object sender, EventArgs e)
        {

        }


        private void ShowFormInPanel(Form formToShow, Panel targetPanel)
        {
            // Clear any existing controls from the panel
            targetPanel.Controls.Clear();

            // Prepare the form to act as a child of the panel
            formToShow.TopLevel = false; // Prevent the form from being a top-level window
            formToShow.FormBorderStyle = FormBorderStyle.None; // Remove form borders
            formToShow.Dock = DockStyle.Fill; // Make the form fill the panel

            // Add the form to the panel's controls and display it
            targetPanel.Controls.Add(formToShow);
            formToShow.Show();
        }



        private void Settings_Load_1(object sender, EventArgs e)
        {

        }

        private void optionsPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void UACflowLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Audio_Label_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        #region menu panel
        // menu launcher
        #endregion menu panel

        private void start_gamemode_Click(object sender, EventArgs e)
        {

            try
            {
                string Path = string.Concat(exeFolder(), "\\GameConsoleMode.exe");
                Process.Start(new ProcessStartInfo(Path));
                Console.WriteLine("GameConsoleMode launched");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error launching GameConsoleMode: " + ex.Message);
            }
        }
      
        private void button_shortcuts_Click(object sender, EventArgs e)
        {
            var shortcutInstance = (shortcuts)InitializeObject("shortcut");
            ShowFormInPanel(shortcutInstance, panel_main);

        }

        private void button_launcher_Click(object sender, EventArgs e)
        {
            var launchersInstance = (launchers)InitializeObject("launchers");
            ShowFormInPanel(launchersInstance, panel_main);
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void button_minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void guna2Button4_Click(object sender, EventArgs e) // SCREEN
        {
            var launchersInstance = (screen)InitializeObject("screen");
            ShowFormInPanel(launchersInstance, panel_main);
        }

        private void button_startup_Click(object sender, EventArgs e)
        {
            var launchersInstance = (startup)InitializeObject("startup");
            ShowFormInPanel(launchersInstance, panel_main);
        }

        private void button_additional_Click(object sender, EventArgs e)
        {
            var launchersInstance = (additional)InitializeObject("additional");
            ShowFormInPanel(launchersInstance, panel_main);
        }
    }
}
