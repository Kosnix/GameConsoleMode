using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Security.Principal;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using LibVLCSharp.Shared;
using System.Drawing;
using System.Collections.Generic;
using LibVLCSharp.WinForms;
using System.Threading.Tasks;
using System.Threading;

namespace GameConsoleMode
{
    static class Program
    {
        private static StreamWriter logWriter;
        private static bool introPlayed = false;


        [STAThread]
        static void Main()
        {
            if (IsAlreadyRunning())
            {
                Console.WriteLine("Another instance of the application is already running.");
                return;
            }
            SetupLogging();
            AdminVerify();
            if (IsAdministrator())
            {
                try
                {
                    if (!VerifyFolder())
                    {
                        MessageBox.Show("Files are missing, please reinstall the program.");
                        BackToWindows();
                        CleanupLogging();
                        Environment.Exit(0);
                    }
                    SettingsVerify();
                    SetVolume();
                    ChangeScreen();
                    HideMouse();
                    StartLauncher();
                    ConsoleModeToShell();
                    HideMouse();
                    if (!introPlayed)
                    {
                        PlayIntro();
                        introPlayed = true; // Set the flag to true after playing the intro
                    }
                    WaitForLauncherToClose();

                    RestoreScreen();
                    BackToWindows();
                }
                finally
                {
                    CleanupLogging();
                }
                Environment.Exit(0);
            }
        }

        private static bool IsAlreadyRunning()
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(currentProcessName);
            return processes.Length > 1;
        }

        static bool VerifyFolder()
        {
            string[] requiredFiles = new string[]
            {
        "Newtonsoft.Json.dll",
        "nomousy.exe",
        "MultiMonitorTool.exe",
        "screenStates.json",
        "LibVLCSharp.dll",
        "LibVLCSharp.Forms.dll",
        "LibVLCSharp.WinForms.dll",
        "Microsoft.Win32.Registry.dll",
        "NAudio.Asio.dll",
        "NAudio.Core.dll",
        "NAudio.dll",
        "NAudio.Midi.dll",
        "NAudio.WinForms.dll",
        "NAudio.WinMM.dll",
        "NAudio.Wasapi.dll",
        "System.Buffers.dll",
        "System.Memory.dll",
        "System.Numerics.Vectors.dll",
        "System.Runtime.CompilerServices.Unsafe.dll",
        "System.Security.AccessControl.dll",
        "System.Security.Principal.Windows.dll",
        "Xamarin.Forms.Core.dll",
        "Xamarin.Forms.Platform.dll",
        "Xamarin.Forms.Xaml.dll",
        "Settings.exe"
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
            if (File.Exists(filePath))
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

        static void FirstStart()
        {
            // Warning message about modifying the Windows registry
            string message = "This application modifies the Windows registry and may temporarily block your PC if used improperly. " +
                             "I disclaim any responsibility for improper use. If you encounter any issues, please visit the project on GitHub: " +
                             "https://github.com/Kosnix/GameConsoleMode";
            string caption = "First Start";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Information;
            MessageBox.Show(message, caption, buttons, icon);

            // Thank you message and initial configuration instructions
            message = "Thank you for downloading my app. This is the first start of the application, please configure it. The settings window will appear.";
            MessageBox.Show(message, caption, buttons, icon);

            // Notification for the next startup
            message = "Next time, the application will start directly.";
            MessageBox.Show(message, caption, buttons, icon);

            // Launch the settings file and terminate the program
            Process.Start(new ProcessStartInfo(Path.Combine(exeFolder(), "Settings.exe")));
            Console.WriteLine("Settings launched");
            CleanupLogging();
            Environment.Exit(0);
        }


        static void SettingsVerify()
        {
            if (!VerifyFile("settings.json"))
            {
                FirstStart();
            }
            
            //verif launcher//
            string launcher = ReadConfig("Launcher");
            if (launcher == "steam" || launcher == "Playnite.FullscreenApp" || launcher == "Other")
            {
                Console.WriteLine("The selected launcher is valid");



                if (launcher == "steam")
                {
                    string steamPath = ReadConfig("SteamPath");
                    if (!string.IsNullOrEmpty(steamPath) && File.Exists(steamPath))
                    {
                        Console.WriteLine("The Steam path is valid.");
                    }
                    else
                    {
                        MessageBox.Show("The Steam path is invalid or non-existent. Use the Settings.exe file to correct this.");
                        CleanupLogging();
                        Environment.Exit(0);
                    }
                }

                if (launcher == "Playnite.FullscreenApp")
                {
                    string PlaynitePath = ReadConfig("PlaynitePath");
                    if (!string.IsNullOrEmpty(PlaynitePath) && File.Exists(PlaynitePath))
                    {
                        Console.WriteLine("The playnite path is valid.");
                    }
                    else
                    {
                        Console.WriteLine("The playnite path is invalid or non-existent. Use the Settings.exe file to correct this.");
                        CleanupLogging();
                        Environment.Exit(0);
                    }
                }

                if (launcher == "Other")
                {
                    string OtherLauncherPath = ReadConfig("OtherLauncherPath");
                    if (!string.IsNullOrEmpty(OtherLauncherPath) && File.Exists(OtherLauncherPath))
                    {
                        Console.WriteLine("The launcher path is valid.");
                    }
                    else
                    {
                        MessageBox.Show("The launcher path is invalid or non-existent. Use the Settings.exe file to correct this.");
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                MessageBox.Show("The selected launcher is invalid or non-existent. Use the Settings.exe file to fix this");
                CleanupLogging();
                Environment.Exit(0);
            }

            //Verif audio//
            string audioBool = ReadConfig("AudioBool");
            if (audioBool == "1")
            {
                int audioVolume = int.Parse(ReadConfig("AudioVolume"));
                if (audioVolume < 0 || audioVolume > 100)
                {
                    MessageBox.Show("The audio volume is outside the allowed range (0-100). Use the Settings.exe file to correct this.");
                    CleanupLogging();
                    Environment.Exit(0);
                }
            }
        }

        static void SetupLogging()
        {
            try
            {
                string logFilePath = Path.Combine(exeFolder(), "log.txt");
                File.Delete(logFilePath);
                logWriter = new StreamWriter(logFilePath, true) { AutoFlush = true };

                // Redirect standard output and error output
                Console.SetOut(logWriter);
                Console.SetError(logWriter);

                Console.WriteLine("Application started...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error setting up log: " + ex.Message);
            }
        }

        static void CleanupLogging()
        {
            if (logWriter != null)
            {
                Console.WriteLine("Application stopped...");
                logWriter.Close();
            }

            // Restore standard output and error output
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
        }

        static string exeFolder()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string folderPath = Path.GetDirectoryName(exePath);
            return folderPath;
        }

        static void AdminVerify()
        {
            if (!IsAdministrator())
            {
                Console.WriteLine("Restarting as admin");
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
                    Environment.Exit(0); // Ensure the program exits immediately
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error restarting application as administrator: " + ex.Message);
                    Environment.FailFast("Failed to restart as administrator.", ex);
                }
            }
        }

        static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void StartLauncher()
        {
            switch (ReadConfig("Launcher"))
            {
                case "steam":
                    StartSteam();
                    break;

                case "Playnite.FullscreenApp":
                    StartPlaynite();
                    break;

                case "Other":
                    StartOtherLauncher();
                    break;

                default:
                    MessageBox.Show("The chosen launcher is not recognized");
                    BackToWindows();
                    break;
            }
        }

        static void StartSteam()
        {
            if (string.IsNullOrWhiteSpace(ReadConfig("SteamPath")) || !File.Exists(ReadConfig("SteamPath")))
            {
                Console.WriteLine("Error: SteamPath is empty, invalid, or does not exist.");
                BackToWindows();
                return;
            }

            KillProcess("steam.exe");

            try
            {
                string Path = ReadConfig("SteamPath");
                string arguments = "-gamepadui -noverifyfiles -nobootstrapupdate -skipinitialbootstrap -overridepackageurl";
                Process.Start(new ProcessStartInfo(Path, arguments));
                Console.WriteLine("Steam launched");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error launching Steam: " + ex.Message);
                BackToWindows();
                Console.WriteLine("explorer restored");
            }
        }

        static void StartPlaynite()
        {

            if (string.IsNullOrWhiteSpace(ReadConfig("PlaynitePath")) || !File.Exists(ReadConfig("PlaynitePath")))
            {
                Console.WriteLine("Error: PlaynitePath is empty, invalid, or does not exist.");
                BackToWindows();
                return;
            }
            KillProcess("Playnite.FullscreenApp.exe");

            try
            {
                string Path = ReadConfig("PlaynitePath");
                string arguments = " --hidesplashscreen";
                Process.Start(new ProcessStartInfo(Path, arguments));
                Console.WriteLine("Playnite launched");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error launching Playnite: " + ex.Message);
                BackToWindows();
                Console.WriteLine("explorer restored");
            }
        }

        static void StartOtherLauncher()
        {
            string launcherPath = ReadConfig("OtherLauncherPath");

            if (string.IsNullOrWhiteSpace(launcherPath) || !File.Exists(launcherPath))
            {
                Console.WriteLine("Error: OtherLauncherPath is empty, invalid, or does not exist.");
                BackToWindows();
                return;
            }

            KillProcess(Path.GetFileName(launcherPath));

            try
            {
                string arguments = ReadConfig("OtherLauncherParameter");
                Process.Start(new ProcessStartInfo(launcherPath, arguments));
                Console.WriteLine("OtherLauncher launched");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error launching OtherLauncher: " + ex.Message);
                BackToWindows();
                Console.WriteLine("Explorer restored");
            }
        }

        static void WaitForLauncherToClose()
        {
            string Launcher = ReadConfig("Launcher");
            if (Launcher == "Other")
            {
                Launcher = Path.GetFileName(ReadConfig("OtherLauncherPath"));
                Launcher = Launcher.Substring(0, Launcher.Length - 4);
                Console.WriteLine($"OtherLauncher name: {Launcher}");
            }
            Process[] processes;
            string ProcessName = Launcher;
            do
            {
                Thread.Sleep(1000);
                processes = Process.GetProcessesByName(ProcessName);
            } while (processes.Length > 0);
            Console.WriteLine($"{ProcessName} closed");
        }

        static void BackToWindows()
        {
            try
            {
                const string keyName = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";
                const string valueName = "Shell";
                const string newValue = @"explorer.exe";

                // Open registry key for writing
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, true))
                {
                    if (key != null)
                    {
                        KillProcess("explorer.exe");
                        // Modify value in registry key
                        key.SetValue(valueName, newValue, RegistryValueKind.String);
                        Console.WriteLine($"Value '{valueName}' has been changed to '{newValue}'.");
                        Process.Start("explorer.exe");
                        Console.WriteLine("explorer.exe restarted.");
                        ShowMouse();
                    }
                    else
                    {
                        Console.WriteLine($"Unable to open registry key '{keyName}'.");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error: Access Denied. Run the application as an administrator.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void ConsoleModeToShell()
        {
            try
            {
                const string keyName = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";
                const string valueName = "Shell";
                // Get executable address to set it as shell
                string newValue = Assembly.GetExecutingAssembly().Location;
                Console.WriteLine("Executable path: " + newValue);

                // Open registry key for writing
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, true))
                {
                    if (key != null)
                    {
                        // Modify value in registry key
                        key.SetValue(valueName, newValue, RegistryValueKind.String);
                        Console.WriteLine($"Value '{valueName}' has been changed to '{newValue}'.");
                        KillProcess("explorer.exe");
                    }
                    else
                    {
                        Console.WriteLine($"Unable to open registry key '{keyName}'.");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error: Access Denied. Run the application as an administrator.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void KillProcess(string ProcessName)
        {
            ProcessName = ProcessName.Substring(0, ProcessName.Length - 4);
            bool explorersStillRunning = true;

            while (explorersStillRunning)
            {
                // Get all processes named "explorer"
                var explorerProcesses = Process.GetProcessesByName(ProcessName);

                // If no "explorer" process found, exit loop
                if (!explorerProcesses.Any())
                {
                    Console.WriteLine("All explorer.exe processes have been successfully killed.");
                    explorersStillRunning = false;
                }
                else
                {
                    // Kill each "explorer" process
                    foreach (var process in explorerProcesses)
                    {
                        try
                        {
                            process.Kill();
                            process.WaitForExit(); // Optional, to ensure process is terminated
                            Console.WriteLine("Explorer.exe process killed successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error attempting to kill explorer.exe: {ex.Message}");
                        }
                    }
                }
            }
        }

        static void StartThisFolderExe(string exeName, string argument)
        {
            string exePath = Path.Combine(exeFolder(), exeName);

            // Create a new instance of Process
            Process process = new Process();

            // Specify executable path and arguments
            process.StartInfo.FileName = exePath;
            process.StartInfo.Arguments = argument;

            try
            {
                // Start the process
                process.Start();
                Console.WriteLine($"The {exeName} process with {argument} was started successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"The {exeName} process encountered an error: " + e.Message);
            }
        }

        static void HideMouse()
        {
            if (ReadConfig("HideMouse") == "true")
                StartThisFolderExe("nomousy.exe", "/hide");
        }

        static void ShowMouse()
        {
            StartThisFolderExe("nomousy.exe", "");
        }

        static string ReadConfig(string key)
        {
            string filePath = Path.Combine(exeFolder(), "settings.json");

            // Check if file exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"The {filePath} file does not exist.");
                return "";
            }

            try
            {
                // Read JSON file content
                string jsonContent = File.ReadAllText(filePath);

                // Parse JSON
                JObject jsonObject = JObject.Parse(jsonContent);

                // Access item specified by key
                JToken item = jsonObject.SelectToken($"$.Settings.{key}");
                // Check if item exists
                if (item != null)
                {
                    string value = item.ToString();
                    // Console.WriteLine($"The '{key}' key is configured to '{value}'");
                    return value;
                }
                else
                {
                    Console.WriteLine($"The '{key}' key was not found in the configuration.");
                    return "0";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading JSON file: {ex.Message}");
                return "0";
            }
        }

        static string ReadScreenConfig(string key)
        {
            string filePath = Path.Combine(exeFolder(), "screenStates.json");

            // Check if file exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"The {filePath} file does not exist.");
                return "";
            }

            try
            {
                // Read JSON file content
                string jsonContent = File.ReadAllText(filePath);

                // Parse JSON
                JObject jsonObject = JObject.Parse(jsonContent);

                // Access item specified by key
                JToken item = jsonObject.SelectToken($"$.screenStates.{key}");
                // Check if item exists
                if (item != null)
                {
                    string value = item.ToString();
                    Console.WriteLine($"The '{key}' key is configured to '{value}'");
                    return value;
                }
                else
                {
                    Console.WriteLine($"The '{key}' key was not found in the configuration.");
                    return "0";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading JSON file: {ex.Message}");
                return "0";
            }
        }

        static void UpdateJsonFile(string key, string newValue)
        {
            try
            {
                string jsonFilePath = Path.Combine(exeFolder(), "settings.json");

                if (!File.Exists(jsonFilePath))
                {
                    throw new FileNotFoundException("Le fichier settings.json est introuvable.");
                }

                string json = File.ReadAllText(jsonFilePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    throw new Exception("Le fichier JSON est vide ou contient uniquement des espaces.");
                }

                JObject jsonObj = JObject.Parse(json);

                if (jsonObj["Settings"] == null)
                {
                    Console.WriteLine("La clé 'Settings' est introuvable dans le fichier JSON.");
                    return;
                }

                if (newValue == null)
                {
                    jsonObj["Settings"][key]?.Parent.Remove();
                }
                else
                {
                    jsonObj["Settings"][key] = newValue;
                }

                using (StreamWriter file = File.CreateText(jsonFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, jsonObj);
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
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur inattendue s'est produite : {ex.Message}");
            }
        }

        public static void ChangeScreen()
        {
            if (ReadConfig("ScreenBool") == "1")
            {
                // Chemin vers l'exécutable DisplaySwitch.exe
                string displaySwitchPath = @"C:\Windows\System32\DisplaySwitch.exe";

                // Argument pour passer en mode "étendre" (extend)
                string arguments = "3";

                // Création du processus
                Process process = new Process();
                process.StartInfo.FileName = displaySwitchPath;
                process.StartInfo.Arguments = arguments;

                // Démarrer le processus
                process.Start();
                process.WaitForExit();

                Console.WriteLine("Screen switched to extended mode.");

                if (!int.TryParse(ReadConfig("SelectedScreen"), out int screenIndex))
                {
                    Console.WriteLine("Invalid screen index in configuration.");
                    return;
                }

                string multiMonitorToolPath = Path.Combine(exeFolder(), "MultiMonitorTool.exe");

                if (!File.Exists(multiMonitorToolPath))
                {
                    Console.WriteLine("MultiMonitorTool not found.");
                    return;
                }

                try
                {
                    // Lire l'état actuel des écrans uniquement si SelectedScreenIndex est vide
                    string selectedScreenIndex = ReadConfig("SelectedScreen");
                    if (string.IsNullOrEmpty(selectedScreenIndex))
                    {
                        var screenStates = new Dictionary<int, bool>();
                        for (int i = 0; i < 10; i++)
                        {
                            bool isEnabled = IsScreenEnabled(multiMonitorToolPath, i + 1);
                            screenStates[i] = isEnabled;
                        }

                        // Enregistrer les états des écrans dans un fichier JSON
                        File.WriteAllText("screenStates.json", JsonConvert.SerializeObject(screenStates));

                        // Enregistrer l'index de l'écran sélectionné dans un fichier JSON
                        UpdateJsonFile("SelectedScreenIndex", screenIndex.ToString());
                    }

                    // Activer l'écran sélectionné avec résolution maximale
                    RunCommand(multiMonitorToolPath, $"/Enable {screenIndex + 1} /SetResolution Max");

                    // Désactiver tous les autres écrans
                    for (int i = 0; i < 10; i++)
                    {
                        if (i != screenIndex)
                        {
                            RunCommand(multiMonitorToolPath, $"/Disable {i + 1}");
                        }
                    }

                    Console.WriteLine($"Screen {screenIndex} activated with maximum resolution.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        private static bool IsScreenEnabled(string toolPath, int screenIndex)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = toolPath,
                    Arguments = $"/GetStatus {screenIndex}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd().ToLower(); // Convertir en minuscules
                process.WaitForExit();

                return output.Contains("on"); // Rechercher en minuscules
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking screen status: {ex.Message}");
                return false;
            }
            finally
            {
                process.Dispose(); // Assurez-vous que le processus est bien nettoyé
            }
        }

        public static void RestoreScreen()
        {
            if (ReadConfig("ScreenBool") == "1") { 
                string screenIndexJson = ReadConfig("SelectedScreenIndex");

            if (!string.IsNullOrEmpty(screenIndexJson) && int.TryParse(screenIndexJson, out int screenIndex))
            {
                string multiMonitorToolPath = Path.Combine(exeFolder(), "MultiMonitorTool.exe");

                if (!File.Exists(multiMonitorToolPath))
                {
                    Console.WriteLine("MultiMonitorTool not found.");
                    return;
                }

                try
                {
                    var screenStates = JsonConvert.DeserializeObject<Dictionary<int, bool>>(File.ReadAllText("screenStates.json"));

                    foreach (var screenState in screenStates)
                    {
                        string command = screenState.Value ? $"/Enable {screenState.Key + 1}" : $"/Disable {screenState.Key + 1}";
                        RunCommand(multiMonitorToolPath, command);
                    }

                    Console.WriteLine("Screens restored to previous states.");

                    // Supprimer la valeur SelectedScreenIndex du fichier JSON
                    UpdateJsonFile("SelectedScreenIndex", null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("No valid screen settings found in the configuration.");
            }
           }
        }

        private static void RunCommand(string toolPath, string arguments)
        {
            Console.WriteLine($"Executing command: {toolPath} {arguments}");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = toolPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                Console.WriteLine($"Command output: {output}");
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Command error: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing command: {ex.Message}");
            }
            finally
            {
                process.Dispose();
            }
        }

        public static void SetVolume()
        {
            string volume = ReadConfig("AudioVolume");
            Console.WriteLine($"Read configuration: AudioVolume = {volume}");

            if (ReadConfig("AudioBool") == "1")
            {
                float level = int.Parse(volume) / 100.0f;
                Console.WriteLine($"Parsed volume level: {level}");

                if (level < 0.0f || level > 1.0f)
                {
                    Console.WriteLine($"Error: Volume level out of range: {level}");
                    throw new ArgumentOutOfRangeException("level", "Le niveau doit être compris entre 0.0 et 1.0");
                }

                try
                {
                    // Récupérer le gestionnaire de volume
                    var deviceEnumerator = new MMDeviceEnumerator();
                    var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    var volumeControl = device.AudioEndpointVolume;

                    // Démuter si nécessaire
                    if (volumeControl.Mute)
                    {
                        volumeControl.Mute = false;
                        Console.WriteLine("Audio was muted, unmuting now.");
                    }

                    // Régler le volume
                    volumeControl.MasterVolumeLevelScalar = level;
                    Console.WriteLine($"Volume set to {level}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Failed to set volume. {ex.Message}");
                    throw;
                }
            }
            else
            {
                Console.WriteLine("Volume change not required");
            }
        }

        [STAThread]
        public static void PlayIntro()
        {
            try { 
            if (!introPlayed)
            {
                if (ReadConfig("IntroBool") == "1")
                {
                    // Vérifie si le chemin de la vidéo est valide
                    string introPath = ReadConfig("IntroPath");
                    if (!File.Exists(introPath))
                    {
                        Console.WriteLine("The introductory video is not available. The reading is skipped.");
                        return;
                    }

                    // Vérifie si le fichier est une vidéo en vérifiant son extension
                    string extension = Path.GetExtension(introPath).ToLower();
                    string[] videoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm" };

                    if (!videoExtensions.Contains(extension))
                    {
                        Console.WriteLine("The specified file is not a video. The reading is skipped.");
                        return;
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Core.Initialize();
                    using (var libvlc = new LibVLC())
                    using (var mediaPlayer = new MediaPlayer(libvlc))
                    {
                        Form introForm = new Form();
                        introForm.FormBorderStyle = FormBorderStyle.None;
                        introForm.WindowState = FormWindowState.Maximized;
                        introForm.TopMost = true;

                        var videoView = new VideoView { MediaPlayer = mediaPlayer, Dock = DockStyle.Fill };
                        introForm.Controls.Add(videoView);
                        mediaPlayer.EndReached += (sender, e) => { introForm.Close(); };

                        mediaPlayer.Media = new Media(libvlc, introPath, FromType.FromPath);
                        if(ReadConfig("IntroMuteBool") == "1")
                            { mediaPlayer.Volume = 0;}
                       else
                            { mediaPlayer.Volume = 100;}
                        
                        mediaPlayer.Play();
                        Console.WriteLine("Playing the intro video.");

                        // Marquer que l'introduction a été lue une fois
                        introForm.ShowDialog();
                        introPlayed = true;
                    }
                }
            }
            }
            finally
            {

            }
        }

    }
}