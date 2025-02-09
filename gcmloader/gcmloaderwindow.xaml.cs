using GAMINGCONSOLEMODE;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using NAudio.CoreAudioApi;
using Windows.Graphics.Display;
using Microsoft.UI;
using Windows.Media.Protection.PlayReady;
using System.Data;
using System.Collections.Generic;
using System.Text.Json;

namespace gcmloader
{
    public sealed partial class MainWindow : Window
    {
        #region needed
        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;
        private const long WS_POPUP = 0x80000000L;
        private const long WS_OVERLAPPEDWINDOW = 0x00CF0000L;
        private const uint WS_EX_NOACTIVATE = 0x08000000;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_SHOWWINDOW = 0x0040;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        private const int HORZRES = 8; // Horizontal resolution
        private const int VERTRES = 10; // Vertical resolution
        private static StreamWriter logWriter;
        private static readonly string SettingsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings");
        private static readonly string SettingsFilePath = Path.Combine(SettingsFolder, "settings.json");
        public MainWindow()
        {
          
            this.InitializeComponent();
            this.Activated += MainWindow_Activated;
            // Start 
            Start();
            // ASYNC PROZES
            StartAsynctasks();
        }



        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            // Get the window handle
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Remove window borders and set to popup style
            IntPtr style = GetWindowLongPtr(hwnd, GWL_STYLE);
            style = (IntPtr)((long)style & ~WS_OVERLAPPEDWINDOW | WS_POPUP);
            SetWindowLongPtr(hwnd, GWL_STYLE, style);

            // Get screen dimensions
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();

            // Set the window size to fullscreen
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, screenWidth, screenHeight, SWP_NOZORDER | SWP_SHOWWINDOW);

            // Set the wallpaper as the background
            SetBackgroundImage(screenWidth, screenHeight);
        }
        private int GetScreenWidth()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            int width = GetDeviceCaps(hdc, HORZRES);
            ReleaseDC(IntPtr.Zero, hdc);
            return width;
        }
        private int GetScreenHeight()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            int height = GetDeviceCaps(hdc, VERTRES);
            ReleaseDC(IntPtr.Zero, hdc);
            return height;
        }

        private DiscordSocketClient _client;

        #endregion needed
        #region methodes
        #region methodes for code
        private Task messagebox(string dialog)
        {
            var messagebox = new ContentDialog
            {
                Title = "Information",
                Content = dialog,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            return messagebox.ShowAsync().AsTask(); // Rückgabe des Tasks
        }
        static string exeFolder()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string folderPath = Path.GetDirectoryName(exePath);
            return folderPath;
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
        static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        #endregion methodes for code
        #region functions
        public void WaitForLauncherToClose()
        {
           

            string Launcher = AppSettings.Load<string>("launcher");
            if (Launcher == "Other")
            {
                Launcher = Path.GetFileName(AppSettings.Load<string>("customlauncherpath"));
                Launcher = Launcher.Substring(0, Launcher.Length - 4);
                Console.WriteLine($"OtherLauncher name: {Launcher}");
            }
            Process[] processes;
            string ProcessName = Launcher;
            do
            {
                Thread.Sleep(3000);
                processes = Process.GetProcessesByName(ProcessName);
            } while (processes.Length > 0);

            // back to windows 
            displayfusion("end");
            BackToWindows();
            CleanupLogging();
            //close
           
        }
        private static bool IsAlreadyRunning()
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(currentProcessName);
            return processes.Length > 1;
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
        private void SetBackgroundImage(int width, int height)
        {
            try
            {
                bool gcmwallpaper = AppSettings.Load<bool>("gcmwallpaper");
                if (gcmwallpaper == true)
                {
                    // Create an Image control
                    Microsoft.UI.Xaml.Controls.Image backgroundImage = new Microsoft.UI.Xaml.Controls.Image();

                    string getwallpaper = Settwallpaper();
                    // Absolute path of the background image
                    string imagePath = getwallpaper;

                    // Ensure the path is valid
                    if (System.IO.File.Exists(imagePath))
                    {
                      
                        // Set the image source
                        backgroundImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                        backgroundImage.Stretch = Stretch.UniformToFill;

                        // Set the size of the image to match the window size
                        backgroundImage.Width = width;
                        backgroundImage.Height = height;

                        // Add the background image to the main content
                        if (this.Content is Grid mainGrid)
                        {
                            mainGrid.Children.Insert(0, backgroundImage); // Add background to the first layer
                        }
                        else
                        {
                            // Use a new Grid as a container
                            Grid grid = new Grid();
                            grid.Children.Add(backgroundImage); // Add background image
                            if (this.Content != null)
                                grid.Children.Add((UIElement)this.Content); // Add the existing content
                            this.Content = grid;
                        }
                    }
                    else
                    {
                        // Log an error if the path is invalid
                        System.Diagnostics.Debug.WriteLine($"Image path not found: {imagePath}");
                    }

                }
                else
                {
                    return;
                }
            }
            catch
            {
                Console.WriteLine("wallpaper gui error");
              
            }

          
        }
        private string Settwallpaper()
        {
            try
            {
                bool gcmwallpaper = AppSettings.Load<bool>("gcmwallpaper");
                if (gcmwallpaper == true)
                {

                    string wallpaperpath = AppSettings.Load<string>("gcmwallpaperpath");
                    return wallpaperpath;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                Console.WriteLine("wallpaper gui error");
                return null;
            }
        }
        static void IsJoyxoffInstalledAndStart()
        {
            try
            {
                bool joyxofftogglestatus = AppSettings.Load<bool>("usejoyxoff");
                if (joyxofftogglestatus == true)
                {
                    string joyxoffExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Joyxoff", "Joyxoff.exe");
                    try
                    {
                        if (File.Exists(joyxoffExePath))
                        {
                            Process.Start(joyxoffExePath);
                        }
                    }
                    catch
                    {

                    }
                }
                else
                {

                }
            }
            catch
            {

            }

        }
        public void displayfusion(string art)
        {

            try
            {
                // Function
                // Full path to DisplayFusionCommand.exe
                string displayFusionCommandPath = @"C:\Program Files\DisplayFusion\DisplayFusionCommand.exe";

                // Check if DisplayFusionCommand.exe exists
                if (!File.Exists(displayFusionCommandPath))
                {
                    Console.WriteLine("DisplayFusionCommand.exe not found at the expected location.");
                    return;
                }
                // Check if action is "start"
                if (art == "start")
                {
                    bool usedisplayfusion = AppSettings.Load<bool>("usedisplayfusion");

                    if (usedisplayfusion == true)
                    {
                        // Get start profile
                        string startprofil = AppSettings.Load<string>("usedisplayfusion_start");

                        if (!string.IsNullOrEmpty(startprofil))
                        {
                            // Command to load the profile using DisplayFusion Command Line
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = displayFusionCommandPath,
                                Arguments = $"-monitorloadprofile \"{startprofil}\"",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true
                            });

                            Console.WriteLine($"Loaded DisplayFusion profile: {startprofil}");
                            // sleep 
                        }
                        else
                        {
                            Console.WriteLine("No start profile configured. Skipping...");
                        }
                    }
                    else
                    {
                        Console.WriteLine("DisplayFusion integration is disabled.");
                    }
                }
                else
                {
                    // Action is "end"
                    bool usedisplayfusion = AppSettings.Load<bool>("usedisplayfusion");

                    if (usedisplayfusion == true)
                    {
                        // Get end profile
                        string endprofil = AppSettings.Load<string>("usedisplayfusion_end");

                        if (!string.IsNullOrEmpty(endprofil))
                        {
                            // Command to load the profile using DisplayFusion Command Line
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = displayFusionCommandPath,
                                Arguments = $"-monitorloadprofile \"{endprofil}\"",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true
                            });

                            Console.WriteLine($"Loaded DisplayFusion profile: {endprofil}");

                        }
                        else
                        {
                            Console.WriteLine("No end profile configured. Skipping...");
                        }
                    }
                    else
                    {
                        Console.WriteLine("DisplayFusion integration is disabled.");
                    }
                }
            }catch
            {
                Console.WriteLine("DisplayFusion problem-");
            }
        }
        static void cssloader()
        {
            try
            {
                bool cssloadertogglestatus = AppSettings.Load<bool>("usecssloader");
                if (cssloadertogglestatus == true)
                {
                    // Check if CSSLOADER Desktop is installed
                    string cssloaderExePath = @"C:\Program Files\CSSLoader Desktop\CSSLoader Desktop.exe";

                    if (File.Exists(cssloaderExePath))
                    {
                        try
                        {
                            // Get the dynamic user profile path
                            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                            string startupCssLoaderPath = Path.Combine(userProfilePath, @"AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\CssLoader-Standalone-Headless.exe");

                            // Check if the CSS Loader Standalone Headless file exists
                            if (File.Exists(startupCssLoaderPath))
                            {
                                // Start the process
                                Process.Start(startupCssLoaderPath);
                            }
                            else
                            {
                                Console.WriteLine("CSS Loader Standalone Headless is not installed in the Startup folder.");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handle any errors that occur during the process
                            Console.WriteLine($"An error occurred while starting the CSS Loader: {ex.Message}");
                        }
                    }
                    else
                    {
                        // CSS Loader Desktop is not installed
                        Console.WriteLine("CSS Loader Desktop is not installed.");
                        return;
                    }
                }
                else
                {

                }
            }
            catch
            {

            }
        }
        static bool VerifySettings()
        {
            try
            {
                // Hole den Pfad des Verzeichnisses, in dem die ausführbare Datei liegt
                string exeFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                // Erstelle den vollständigen Pfad zum "settings"-Ordner
                string settingsFolderPath = Path.Combine(exeFolderPath, "settings");

                // Erstelle den vollständigen Pfad zur Datei "settings.json"
                string settingsFilePath = Path.Combine(settingsFolderPath, "settings.json");

                // Überprüfe, ob der Ordner "settings" existiert
                if (Directory.Exists(settingsFolderPath))
                {
                    // Überprüfe, ob die Datei "settings.json" im Ordner existiert
                    if (File.Exists(settingsFilePath))
                    {
                        Console.WriteLine($"The file 'settings.json' exists in the folder '{settingsFolderPath}'.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"The file 'settings.json' is missing in the folder '{settingsFolderPath}'.");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine($"The folder 'settings' does not exist in '{exeFolderPath}'.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while verifying the settings: {ex.Message}");
                return false;
            }
        }
        private void FirstStart()
        {
            // Warning message about modifying the Windows registry
            string message = "This application modifies the Windows registry and may temporarily block your PC if used improperly. " +
                             "I disclaim any responsibility for improper use. If you encounter any issues, please visit the project on GitHub: " +
                             "https://github.com/Kosnix/GameConsoleMode";
            string caption = "First Start";

            Console.WriteLine(message);

            // Thank you message and initial configuration instructions
            message = "Thank you for downloading my app. This is the first start of the application, please configure it. The settings window will appear.";
            Console.WriteLine(message);
            // Notification for the next startup
            message = "Next time, the application will start directly.";
            Console.WriteLine(message);

            // Launch the settings file and terminate the program
            Process.Start(new ProcessStartInfo(Path.Combine(exeFolder(), "GAMINGCONSOLEMODE.exe")));
            Console.WriteLine("Settings launched");
            CleanupLogging();
            Environment.Exit(0);
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
                        Console.WriteLine("Registry key opened successfully.");

                        KillProcess("explorer.exe");

                        // Modify value in registry key
                        key.SetValue(valueName, newValue, RegistryValueKind.String);
                        Console.WriteLine($"Value '{valueName}' has been changed to '{newValue}'.");

                        // Verify the change
                        string currentValue = key.GetValue(valueName)?.ToString();
                        if (currentValue == newValue)
                        {
                            Console.WriteLine($"Successfully set '{valueName}' to '{newValue}'.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to set '{valueName}'. Current value: {currentValue}");
                        }

                        // Restart explorer.exe
                        Process.Start("explorer.exe");
                        Console.WriteLine("explorer.exe restarted.");
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
        static void StartLauncher()
        {
            string launcher = AppSettings.Load<string>("launcher");
            switch (launcher)
            {
                case "steam":
                    StartSteam();
                    break;

                case "playnite":
                    StartPlaynite();
                    break;

                case "custom":
                    StartOtherLauncher();
                    break;

                default:
                    Console.WriteLine("Invalid launcher. Defaulting to Custom.");
                    launcher = "steam";
                    AppSettings.Save("launcher", launcher);
                    BackToWindows();

                    break;
            }

        }
        static void ConsoleModeToShell()
        {
            try
            {
                const string keyName = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";
                const string valueName = "Shell";

                // Get the path of the current directory and append the target executable name
                string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string targetExecutable = Path.Combine(currentDirectory, "gcmloader.exe");

                Console.WriteLine($"Current Directory: {currentDirectory}");
                Console.WriteLine($"Target Executable Path: {targetExecutable}");

                if (!File.Exists(targetExecutable))
                {
                    Console.WriteLine($"Error: The file '{targetExecutable}' does not exist.");
                    return;
                }

                // Open registry key for writing
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, writable: true))
                {
                    if (key != null)
                    {
                        // Modify value in registry key
                        key.SetValue(valueName, targetExecutable, RegistryValueKind.String);

                        // Verify the change
                        string currentValue = key.GetValue(valueName)?.ToString();
                        if (currentValue == targetExecutable)
                        {
                            Console.WriteLine($"Successfully set '{valueName}' to '{targetExecutable}'.");
                            KillProcess("explorer.exe");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to set '{valueName}'. Current value: {currentValue}");
                        }
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
        private void SettingsVerify()
        {

            if (!VerifySettings())
            {
                Console.WriteLine("The settings folder or file is missing. Initializing first start process...");
                FirstStart();
            }

            //verif launcher//
            string launcher = AppSettings.Load<string>("launcher");
            if (launcher == "steam" || launcher == "Playnite.FullscreenApp" || launcher == "Other")
            {
                Console.WriteLine("The selected launcher is valid");



                if (launcher == "steam")
                {
                    string steamPath = AppSettings.Load<string>("steamlauncherpath");
                    if (!string.IsNullOrEmpty(steamPath) && File.Exists(steamPath))
                    {
                        Console.WriteLine("The Steam path is valid.");
                    }
                    else
                    {
                     //MessageBox.Show("The Steam path is invalid or non-existent. Use the Settings.exe file to correct this.");
                        CleanupLogging();
                        Environment.Exit(0);
                    }
                }

                if (launcher == "Playnite.FullscreenApp")
                {
                    string PlaynitePath = AppSettings.Load<string>("playnitelauncherpath");
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
                    string OtherLauncherPath = AppSettings.Load<string>("customlauncherpath");
                    if (!string.IsNullOrEmpty(OtherLauncherPath) && File.Exists(OtherLauncherPath))
                    {
                        Console.WriteLine("The launcher path is valid.");
                    }
                    else
                    {
                       // MessageBox.Show("The launcher path is invalid or non-existent. Use the Settings.exe file to correct this.");
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                //MessageBox.Show("The selected launcher is invalid or non-existent. Use the Settings.exe file to fix this");
                CleanupLogging();
                Environment.Exit(0);
            }
        }
        public async Task StartAsynctasks()
        {
            try
            {
                //discord
                bool usediscord = AppSettings.Load<bool>("usediscord");
                if (usediscord)
                {
                    await MonitorDiscordProcessAndWindow();
                }
            }
            catch (Exception ex)
            {

            }
        }
        #region discord 
        static void Changeaudio(string playback)
        {
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audiochanger.ps1");
            string nircmdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nircmd.exe");
            // Check if the script file exists
            if (!File.Exists(scriptPath))
            {
                Console.WriteLine($"Error: PowerShell script not found in the directory: {scriptPath}");
                return;
            }

            try
            {
                // Build PowerShell arguments
                string arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\" -nircmdPath \"{nircmdPath}\" -deviceName \"{playback}\"";

                // Set up the process to run PowerShell
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Start the process
                using (Process process = Process.Start(psi))
                {
                    // Capture the output
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    // Display the output or errors
                    if (!string.IsNullOrEmpty(output))
                    {
                        Console.WriteLine($"Output: {output}");
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine($"Error: {error}");
                    }

                    Console.WriteLine("PowerShell script executed successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while executing PowerShell script: {ex.Message}");
            }
        }
        #region discord automatic need
        // Importing user32.dll functions to interact with window handles
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd); // Checks if a window is minimized

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);
        #endregion discord automatic need
        private static async Task MonitorDiscordProcessAndWindow()
        {
            // Initial
            // Load the saved device ID from the configuration
            string startdiscord = AppSettings.Load<string>("discordstart");
            // Load the saved device ID from the configuration
            string enddiscord = AppSettings.Load<string>("discordend");
            bool handlestate = false;

            while (true)
            {
                try
                {
                    // Check if Discord process is running
                    Process[] discordProcesses = Process.GetProcessesByName("Discord");
                    if (discordProcesses.Length > 0)
                    {
                        // Get the main window handle of the Discord process
                        IntPtr discordHandle = discordProcesses[0].MainWindowHandle;

                        if (discordHandle == IntPtr.Zero)
                        {
                            // Discord is running, but no window is detected

                            if (string.IsNullOrEmpty(enddiscord))
                            {
                                //No audio device ID found in start the configuration
                               
                                return;
                            }
                            else
                            {
                                if (handlestate == true)
                                {
                                    Changeaudio(enddiscord);
                                    handlestate = false;
                                }
                                else
                                {
                                    //nothing
                                }

                            }
                        }
                        else if (IsIconic(discordHandle))
                        {
                            // Discord is running, but the window is minimized;

                            if (string.IsNullOrEmpty(enddiscord))
                            {
                                //"No audio device ID found in start the configuration
                                
                                return;
                            }
                            else
                            {
                                if (handlestate == true)
                                {
                                    Changeaudio(enddiscord);
                                    handlestate = false;
                                }
                                else
                                {
                                   //nothing
                                }

                            }

                        }
                        else
                        {
                            // Discord is running, and the window is open
                            if (string.IsNullOrEmpty(startdiscord))
                            {

                               //No audio device ID found in start the configuration
                                
                                return;
                            }
                            else
                            {
                                
                                if (handlestate == false)
                                {
                                    Changeaudio(startdiscord);
                                    handlestate = true;
                                }
                                else
                                {
                                    //nothing
                                }
                            }


                        }
                    }
                    else
                    {

                    }

                    // Wait for 5 seconds before checking again
                    await Task.Delay(3000);
                }
                catch (Exception ex)
                {
                   
                    await Task.Delay(3000); // Avoid rapid retries on error
                }
            }

        }
        // Method to append logs to the file on the Desktop
        #endregion discord
        #endregion functions
        #region launcher
        static void StartSteam()
        {
            if (string.IsNullOrWhiteSpace(AppSettings.Load<string>("steamlauncherpath")) || !File.Exists(AppSettings.Load<string>("steamlauncherpath")))
            {
                Console.WriteLine("Error: SteamPath is empty, invalid, or does not exist.");
                BackToWindows();
                return;
            }

            KillProcess("steam.exe");
            Console.WriteLine("try start Steam");
            try
            {
                string Path = AppSettings.Load<string>("steamlauncherpath");
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

            if (string.IsNullOrWhiteSpace(AppSettings.Load<string>("playnitelauncherpath")) || !File.Exists(AppSettings.Load<string>("playnitelauncherpath")))
            {
                Console.WriteLine("Error: PlaynitePath is empty, invalid, or does not exist.");
                BackToWindows();
                return;
            }
            KillProcess("Playnite.FullscreenApp.exe");

            try
            {
                string Path = AppSettings.Load<string>("playnitelauncherpath");
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
            string launcherPath = AppSettings.Load<string>("customlauncherpath");

            if (string.IsNullOrWhiteSpace(launcherPath) || !File.Exists(launcherPath))
            {
                Console.WriteLine("Error: OtherLauncherPath is empty, invalid, or does not exist.");
                BackToWindows();
                return;
            }

            KillProcess(Path.GetFileName(launcherPath));

            try
            {
                Process.Start(new ProcessStartInfo(launcherPath));
                Console.WriteLine("OtherLauncher launched");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error launching OtherLauncher: " + ex.Message);
                BackToWindows();
                Console.WriteLine("Explorer restored");
            }
        }
        #endregion launcher
        #region start
        private async Task Start()
        {
            if (IsAlreadyRunning())
            {
                Console.WriteLine("Another instance of the application is already running.");
                Environment.Exit(0);
            }
            else // First Instance
            {
                SetupLogging();
                AdminVerify();
                if (IsAdministrator())
                {
                        SettingsVerify();
                    displayfusion("start");
                        IsJoyxoffInstalledAndStart(); //only check if is installed, than start
                        cssloader(); //only check if is installed, than start
                        StartLauncher();
                        ConsoleModeToShell();
                    await Task.Run(() =>
                    {
                        WaitForLauncherToClose();
                       
                    });

                    this.Close();
                }
            }
        }
        #endregion start
        #endregion methodes
    }

}