using Guna.UI2.WinForms;
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameConsoleMode
{
    public partial class explorer : Form
    {
        #region function and variable

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

        public explorer()
        {
            InitializeComponent();
            this.Text = "Explorer";
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = System.Drawing.Color.Black; // Set a background col
            timer_open_apps.Start();


        }

        private void explorer_Load(object sender, EventArgs e)
        {
           string customwallpaper =  Readconfig("customwallpaper");
            if (customwallpaper == "1")
            {
                //on
                string customwallpaperpath = Readconfig("customwallpaperpath");
                if (!string.IsNullOrEmpty(customwallpaperpath) && System.IO.File.Exists(customwallpaperpath))
                {
                    // Set the form's background image
                    this.BackgroundImage = Image.FromFile(customwallpaperpath);
                    this.BackgroundImageLayout = ImageLayout.Stretch; // Adjust image layout as needed
                    Console.WriteLine($"Form background set to: {customwallpaperpath}");
                }
                else
                {
                    Console.WriteLine("Custom wallpaper path is invalid or file does not exist. Skipping...");
                }

            }
            else if (customwallpaper == "0")
            {
                //off
                // Remove the background image
                this.BackgroundImage = null;

                // Set the background color to black
                this.BackColor = Color.Black;
            }
            else
            {
                // Remove the background image
                this.BackgroundImage = null;
             
                // Set the background color to black
                this.BackColor = Color.Black;

            }
        }

        private void timer_open_apps_Tick(object sender, EventArgs e)
        {
            string stopgcm = Readconfig("stopgcmexplorer");
            if (stopgcm == "1")
            {
                //run
            }
            else if (stopgcm == "0")
            {
                //stop
                this.Close();
            }
            else
            {
                //stop
                this.Close();
            }

            flowLayoutPanel_open_apps.Controls.Clear();

            foreach (Process process in Process.GetProcesses())
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    var appPanel = new Guna2GradientPanel
                    {
                        Size = new Size(246, 275),
                        BorderColor = Color.Black,
                        BorderThickness = 0,
                        BorderRadius = 10,
                        FillColor = Color.FromArgb(27, 40, 56),
                        FillColor2 = Color.FromArgb(60, 60, 60),
                        BackColor = Color.Transparent,
                        
                    };

                    var titleLabel = new Label
                    {
                        Text = process.MainWindowTitle,
                        Dock = DockStyle.Top,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        Height = 40

                    };
                    appPanel.Controls.Add(titleLabel);

                    var closeButton = new Guna2Button
                    {
                        Text = "Close App",
                        Dock = DockStyle.Bottom,
                        Height = 30,
                        FillColor = Color.Brown,
                        ForeColor = Color.White,
                        BorderRadius = 10
                        
                    };
                    closeButton.Click += (s, args) =>
                    {
                        try
                        {
                            process.Kill();
                            process.WaitForExit();
                            flowLayoutPanel_open_apps.Controls.Remove(appPanel);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to close process: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };
                    appPanel.Controls.Add(closeButton);

                    var focusButton = new Guna2Button
                    {
                        Text = "Focus App",
                        Dock = DockStyle.Bottom,
                        Height = 30,
                        FillColor = Color.Green,
                        ForeColor = Color.White,
                        BorderRadius = 10
                    };
                    focusButton.Click += (s, args) =>
                    {
                        try
                        {
                            IntPtr handle = process.MainWindowHandle;
                            if (handle != IntPtr.Zero)
                            {
                                ShowWindow(handle, SW_RESTORE);
                                SetForegroundWindow(handle);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to focus process: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };
                    appPanel.Controls.Add(focusButton);

                    flowLayoutPanel_open_apps.Controls.Add(appPanel);
                }
            }
        }
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;
    }

}
