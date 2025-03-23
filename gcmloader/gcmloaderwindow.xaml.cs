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
using Windows.Media.Core;
using Windows.Media.Playback;
using Microsoft.UI.Windowing;
using System.Xml.Linq;
using System.Text;
using Windows.System;
using SharpDX.XInput;
using System.Timers;
using Button = Microsoft.UI.Xaml.Controls.Button;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using WinRT.Interop;
using System.Windows.Input;
using Microsoft.UI.Xaml.Input;
using Image = Microsoft.UI.Xaml.Controls.Image;

using HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;
using Microsoft.VisualBasic;
using Orientation = Microsoft.UI.Xaml.Controls.Orientation;
using Microsoft.UI.Xaml.Media.Animation;
using System.Linq.Expressions;


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

        #region TaskManager
        private TranslateTransform _listTranslate = new TranslateTransform() { X = 0, Y = 0 };

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        private static extern bool AllowSetForegroundWindow(int dwProcessId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        #endregion TaskManager
        // Dans ta classe MainWindow
        private List<(uint Pid, string ProductName)> _previousWindows = new List<(uint, string)>();



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
            this.Activated += (s, e) => this.Content.Focus(FocusState.Programmatic);
            this.Content.KeyDown += MainWindow_KeyDown;
            Start();
            //ASYNC PROZES
            ShowTaskManager(); //after 10 seconds
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


        private bool IsWindowInForeground()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            return GetForegroundWindow() == hWnd;
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
            Process[] processes;

            if (Launcher == "custom")
            {
                Launcher = Path.GetFileName(AppSettings.Load<string>("customlauncherpath"));
                Launcher = Launcher.Substring(0, Launcher.Length - 4);

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
            else if (Launcher == "playnite")
            {
                string ProcessName = "Playnite.FullscreenApp";
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
            else if (Launcher == "steam")
            {

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
            else
            {
                // back to windows 
                displayfusion("end");
                BackToWindows();
                CleanupLogging();
                //close
            }

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
            else
            {
                uac("off");
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
            }
            catch
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
                // Get the path of the AppData folder
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                // Create the full path to the "gcmsettings" folder in AppData
                string settingsFolderPath = Path.Combine(appDataPath, "gcmsettings");

                // Create the full path to the "settings.json" file within the folder
                string settingsFilePath = Path.Combine(settingsFolderPath, "settings.json");

                // Check if the "gcmsettings" folder exists
                if (Directory.Exists(settingsFolderPath))
                {
                    // Check if the "settings.json" file exists in the folder
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
                    Console.WriteLine($"The folder 'gcmsettings' does not exist in AppData.");
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
        static void uac(string art)
        {

            if (art == "on")
            {
                try
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 5);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 1);

                    //  MessageBox.Show("UAC has been successfully enabled.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            else
            {
                try
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 0);

                    //  MessageBox.Show("UAC has been successfully disabled.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    //  MessageBox.Show("An error occurred while disabling UAC: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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



                if (!File.Exists(targetExecutable))
                {
                    //Logger.Logger.Log($"Error: The file '{targetExecutable}' does not exist.");
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
            if (launcher == "steam" || launcher == "playnite" || launcher == "custom")
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

                if (launcher == "playnite")
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

                if (launcher == "custom")
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
                string arguments;
                //  if (AppSettings.Load<bool>("usestartupvideo")){
                // arguments = "-gamepadui -noverifyfiles -nobootstrapupdate -skipinitialbootstrap -overridepackageurl";
                //  }
                //  else
                //  {
                arguments = "-gamepadui -noverifyfiles -nobootstrapupdate -skipinitialbootstrap -overridepackageurl -noinstro";
                //  }

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

                //Logger.Logger.Log($"Error: PlaynitePath is empty, invalid, or does not exist.");
                BackToWindows();
                return;
            }
            KillProcess("Playnite.FullscreenApp.exe");
            try
            {
                string arguments = " --hidesplashscreen";
                string Path = AppSettings.Load<string>("playnitelauncherpath");
                Process.Start(new ProcessStartInfo(Path, arguments));
                // Logger.Logger.Log("Playnite launched");
            }
            catch (Exception ex)
            {
                //Logger.Logger.Log("Error launching Playnite");
                BackToWindows();
                //Logger.Logger.Log("explorer restored");
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
                // Logger.Logger.Log($"start SETUPLOGGIN:");
                SetupLogging();
                // Logger.Logger.Log($"start ADMINVERIFY:");
                AdminVerify();
                if (IsAdministrator())
                {
                    SettingsVerify();
                    StartupVideo.Play();
                    displayfusion("start");
                    IsJoyxoffInstalledAndStart(); //only check if is installed, than start
                    cssloader(); //only check if is installed, than start
                    StartLauncher();
                    SetupGamepad();
                    // TaskManager //
                    TaskManagerPanel.RenderTransform = _listTranslate;
                    CenterSelectedApp();
                    LoadTaskManagerList();
                    InitializeTaskManagerRefresh();
                    ///////////////
                    ConsoleModeToShell();
                    LoadTaskManagerList();
                    await Task.Run(() =>
                    {
                        WaitForLauncherToClose();

                    });
                    try
                    {
                        StartupVideo.RenameSteamStartupVideo_End();
                    }
                    catch { }

                    uac("on");
                    this.Close();
                }
            }
        }
        #endregion start

        #region TaskManager

        public bool TaskManagerVisibility;

        // Internal class to represent an application row
        private class ProgramRow
        {
            public StackPanel ColumnPanel;  // Vertical column for each application
            public TextBlock NameText;      // Application name
            public Button FocusButton;      // Focus button
            public Button KillButton;       // Kill button

            public IntPtr Hwnd;             // Window handle
            public Process Proc;            // Corresponding process
        }

        // Instead of _selectedRow => _selectedAction
        // Instead of _selectedCol => _selectedApp
        private int _selectedAction = 0;  // 0 = Name, 1 = Focus, 2 = Kill
        private int _selectedApp = 0;     // index of the currently selected application

        // Timer to refresh the list every second
        private DispatcherTimer _refreshTimer;

        // List of rows (each row = one app/process)
        private List<ProgramRow> _rows = new List<ProgramRow>();

        // For detecting minimized / non-minimized
        private AppWindow _appWindow;

        // Used to check if the window is currently in the foreground
        private bool _isForeground = false;

        private void InitializeTaskManagerRefresh()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = AppWindow.GetFromWindowId(windowId);

            if (_appWindow != null)
            {
                _appWindow.Changed += OnAppWindowChanged;
            }

            _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _refreshTimer.Tick += (s, e) =>
            {
                if (IsWindowInForeground())
                {
                    LoadTaskManagerList();
                }
            };
            _refreshTimer.Start();
        }

        private void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
        {
            try
            {
                if (sender.Presenter is OverlappedPresenter p)
                {
                    IntPtr foregroundWindow = GetForegroundWindow();
                    IntPtr appWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(sender);

                    _isForeground = (foregroundWindow == appWindowHandle);
                }
            }
            catch { }
        }

        private void ShowTaskManager()
        {
            var hideTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            hideTimer.Tick += (s, e) =>
            {
                hideTimer.Stop();
                TaskManagerPanel.Visibility = Visibility.Visible;
            };
            hideTimer.Start();

        }

        private void LoadTaskManagerList()
        {
            if (TaskManagerPanel == null) return;

            // 1) Construit la "nouvelle liste" de (pid, productName)
            var currentWindows = new List<(uint Pid, string ProductName)>();

            // EnumWindows pour remplir currentWindows
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    // Récup pid
                    GetWindowThreadProcessId(hWnd, out uint pid);

                    Process p;
                    try
                    {
                        p = Process.GetProcessById((int)pid);
                    }
                    catch
                    {
                        return true;
                    }

                    // Skip self
                    if (pid == (uint)Process.GetCurrentProcess().Id)
                        return true;

                    // Récup nom
                    string productName;
                    try
                    {
                        productName = p.MainModule?.FileVersionInfo?.ProductName;
                    }
                    catch
                    {
                        productName = null;
                    }
                    if (string.IsNullOrWhiteSpace(productName))
                        productName = p.ProcessName;

                    // Filtrage
                    if (productName?.Contains("Windows", StringComparison.OrdinalIgnoreCase) == true
                        || p.ProcessName.Equals("explorer", StringComparison.OrdinalIgnoreCase)
                        || productName?.Contains("SwUSB", StringComparison.OrdinalIgnoreCase) == true
                        || productName?.Contains("csrss", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        return true;
                    }

                    // Ajoute à la liste
                    currentWindows.Add((pid, productName));
                }
                return true;
            }, IntPtr.Zero);

            // 2) Compare la nouvelle liste avec _previousWindows
            if (!AreWindowListsDifferent(_previousWindows, currentWindows))
            {
                // => Pas de changement, on sort sans rafraîchir l’UI
                return;
            }

            // S’il y a un changement, on met à jour _previousWindows
            _previousWindows = currentWindows;

            // 3) On vide l’UI, on vide _rows
            TaskManagerPanel.Children.Clear();
            _rows.Clear();

            // 4) Pour chaque item, on appelle CreateProgramCol, etc.
            foreach (var (pid, productName) in currentWindows)
            {
                // Retrouve le Process
                Process p;
                try
                {
                    p = Process.GetProcessById((int)pid);
                }
                catch
                {
                    continue;
                }

                IntPtr hWnd = p.MainWindowHandle;
                // => si tu veux la MainWindow ou tu peux re-EnumWindows pour each pid, 
                //    mais on suppose que tu veux la "primary window"

                var row = CreateProgramCol(productName, p, hWnd);
                TaskManagerPanel.Children.Add(row.ColumnPanel);
                _rows.Add(row);
            }

            // 5) Appelle UpdateRowSelection
            UpdateRowSelection(false);
        }

        private bool AreWindowListsDifferent(
    List<(uint Pid, string ProductName)> oldList,
    List<(uint Pid, string ProductName)> newList)
        {
            if (oldList.Count != newList.Count)
                return true;

            // Vérifie si chaque élément est le même (ordre, etc.)
            // Ici on suppose que l’ordre n’est pas forcément identique, donc on peut
            // faire un tri ou un ensemble pour comparer plus simplement

            var oldSet = oldList.ToHashSet();
            var newSet = newList.ToHashSet();

            return !oldSet.SetEquals(newSet);
        }


        private ProgramRow CreateProgramCol(string programName, Process proc, IntPtr hwnd)
        {
            var baseText = new SolidColorBrush(Colors.White);

            // Main panel for each app
            var rowPanel = new StackPanel
            {
                Orientation = Microsoft.UI.Xaml.Controls.Orientation.Vertical,
                Margin = new Thickness(10),
                Width = 250,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            // Buttons panel
            var buttonPanel = new StackPanel
            {
                Orientation = Microsoft.UI.Xaml.Controls.Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                Visibility = Visibility.Collapsed
            };

            var focusButton = new Button
            {
                Content = "Show",
                FontSize = 18,
                Margin = new Thickness(5),
                Foreground = baseText
            };
            var killButton = new Button
            {
                Content = "Close",
                FontSize = 18,
                Margin = new Thickness(5),
                Foreground = baseText
            };

            focusButton.Click += (s, e) => SetForegroundWindow(hwnd);
            killButton.Click += (s, e) => { try { proc.Kill(); } catch { } LoadTaskManagerList(); };

            buttonPanel.Children.Add(focusButton);
            buttonPanel.Children.Add(killButton);

            // Name
            var nameText = new TextBlock
            {
                Text = programName,
                FontSize = 15,
                Foreground = baseText,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Icon
                var iconSource = GetLargestIconFromExe(proc);
                var programIcon = new Image
                {
                    Source = iconSource,
                    Width = 96,
                    Height = 96,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                
            if (programName == "Discord")
            {
                try {
                    var discordImage = new BitmapImage(new Uri("ms-appx:///Assets/discord.png"));
                    programIcon.Source = discordImage;

                }
                catch { iconSource = GetLargestIconFromExe(proc); }
                
            }
            else
            {
                iconSource = GetLargestIconFromExe(proc);
            }


            rowPanel.Children.Add(buttonPanel);
            rowPanel.Children.Add(programIcon);
            rowPanel.Children.Add(nameText);

            return new ProgramRow
            {
                ColumnPanel = rowPanel,
                NameText = nameText,
                FocusButton = focusButton,
                KillButton = killButton,
                Hwnd = hwnd,
                Proc = proc
            };
        }

        private ProgramRow _previousSelectedRow = null;

        private void UpdateRowSelection(bool anim)
        {
            for (int i = 0; i < _rows.Count; i++)
            {
                var row = _rows[i];
                bool isAppSelected = (_selectedApp == i);

                // -- Mise en évidence du fond si sélectionné
                row.ColumnPanel.Background = isAppSelected
                    ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 50, 50, 50))
                    : null;

                // -- Boutons
                row.FocusButton.Visibility = isAppSelected ? Visibility.Visible : Visibility.Collapsed;
                row.KillButton.Visibility = isAppSelected ? Visibility.Visible : Visibility.Collapsed;
                row.ColumnPanel.Children[0].Visibility = isAppSelected ? Visibility.Visible : Visibility.Collapsed;

                // -- Couleur des boutons en fonction de _selectedAction
                if (isAppSelected)
                {
                    if (_selectedAction == 1)
                        row.FocusButton.Background = new SolidColorBrush(Colors.CornflowerBlue);
                    else
                        row.FocusButton.Background = new SolidColorBrush(Colors.DimGray);

                    if (_selectedAction == 2)
                        row.KillButton.Background = new SolidColorBrush(Colors.CornflowerBlue);
                    else
                        row.KillButton.Background = new SolidColorBrush(Colors.DimGray);
                }
                else
                {
                    row.FocusButton.Background = new SolidColorBrush(Colors.DimGray);
                    row.KillButton.Background = new SolidColorBrush(Colors.DimGray);
                }

                // -- Animation
                if (isAppSelected)
                {
                    // Si c’est vraiment un nouveau changement de sélection
                    if (anim && _previousSelectedRow != row)
                    {
                        // Zoom sur le nouvel élément
                        AnimateSelection(row, true);

                        // Dé-zoomer l’ancien s’il existe et est différent
                        if (_previousSelectedRow != null && _previousSelectedRow != row)
                        {
                            AnimateSelection(_previousSelectedRow, false);
                        }

                        // Mettre à jour la référence
                        _previousSelectedRow = row;
                    }
                }
                else
                {
                    // Si cet élément n’est plus sélectionné ET c’était le précédent
                    // on le repasse à 1.0 (mais uniquement si on veut animer)
                    if (anim && _previousSelectedRow == row)
                    {
                        AnimateSelection(row, false);
                    }
                }
            }
        }

        private void MoveAction(int delta)
        {
            PlaySound(0);
            // 0=Name, 1=Focus, 2=Kill
            _selectedAction = (_selectedAction + delta + 3) % 3;
            UpdateRowSelection(false);
        }

        private void MoveApp(int delta)
        {
            PlaySound(0);
            if (_rows.Count == 0) return;

            _selectedApp = (_selectedApp + delta + _rows.Count) % _rows.Count;
            _selectedAction = 0;
            UpdateRowSelection(true);
            CenterSelectedApp();
        }

        private void PlaySound(short sound)
        {
            try
            {
                string path = sound switch
                {
                    0 => "Assets\\navigation.wav",
                    1 => "Assets\\activation.wav",
                    _ => null
                };

                if (!string.IsNullOrEmpty(path))
                {
                    SoundPlayer player = new SoundPlayer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));
                    player.Play();
                }
            }
            catch { }
        }


        private void CenterSelectedApp()
        {
            double windowWidth = GetScreenWidth(); // ou la largeur du conteneur parent
            double itemWidth = 270;                // la largeur supposée par item

            // Milieu de l'écran
            double screenCenter = windowWidth / 2.0;

            // Centre horizontal de l'item sélectionné
            // => index * itemWidth + (itemWidth/2)
            double selectedCenter = _selectedApp * itemWidth + (itemWidth / 2);

            // offset = centre de l'écran - centre de l'élément
            double offset = screenCenter - selectedCenter;

            _listTranslate.X = offset;
        }

        private void ExecuteSelectedAction()
        {
            PlaySound(1);
            if (_rows.Count == 0) return;
            var row = _rows[_selectedApp];

            if (_selectedAction == 1)
            {
                // Focus
                ShowWindow(row.Hwnd, SW_RESTORE);
                SetForegroundWindow(row.Hwnd);
            }
            else if (_selectedAction == 2)
            {
                // Kill
                try { row.Proc.Kill(); } catch { }
                LoadTaskManagerList();
            }
        }

        // Extract the largest icon from an exe/dll
        private BitmapImage GetLargestIconFromExe(Process proc)
        {
            try
            {
                if (proc?.MainModule == null) return null;
                string exePath = proc.MainModule.FileName;
                if (!File.Exists(exePath)) return null;

                var icon = IconExtractor.ExtractLargestIconFromExe(exePath);
                if (icon == null) return null;

                using (var ms = new MemoryStream())
                {
                    icon.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;
                    BitmapImage bmpImg = new BitmapImage();
                    bmpImg.SetSource(ms.AsRandomAccessStream());
                    return bmpImg;
                }
            }
            catch
            {
                // If an error occurs, just return null (avoid crashing)
                return null;
            }
        }

        public static class IconExtractor
        {
            // Data structures for icon resources
            [StructLayout(LayoutKind.Sequential, Pack = 2)]
            private struct ICONDIR
            {
                public short Reserved;
                public short Type;
                public short Count;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 2)]
            private struct ICONDIRENTRY
            {
                public byte Width;
                public byte Height;
                public byte ColorCount;
                public byte Reserved;
                public short Planes;
                public short BitCount;
                public int BytesInRes;
                public int ImageOffset;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 2)]
            private struct GRPICONDIR
            {
                public short Reserved;
                public short Type;
                public short Count;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 2)]
            private struct GRPICONDIRENTRY
            {
                public byte Width;
                public byte Height;
                public byte ColorCount;
                public byte Reserved;
                public short Planes;
                public short BitCount;
                public int BytesInRes;
                public short ID;
            }

            private const int RT_GROUP_ICON = 14;
            private const int RT_ICON = 3;
            private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, int lpType);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

            [DllImport("kernel32.dll", SetLastError = false)]
            private static extern IntPtr LockResource(IntPtr hResData);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern int SizeofResource(IntPtr hModule, IntPtr hResInfo);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            private static extern bool EnumResourceNames(IntPtr hModule, int lpszType, EnumResNameProc lpEnumFunc, IntPtr lParam);

            private delegate bool EnumResNameProc(IntPtr hModule, int lpszType, IntPtr lpszName, IntPtr lParam);

            /// <summary>
            /// Extracts the largest icon from a specified exe/dll file.
            /// </summary>
            public static Icon ExtractLargestIconFromExe(string filePath)
            {
                Icon bestIconSoFar = null;
                int bestScoreSoFar = -1;

                IntPtr hModule = LoadLibraryEx(filePath, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                if (hModule == IntPtr.Zero) return null;

                try
                {
                    // Enumerate icon groups
                    EnumResourceNames(hModule, RT_GROUP_ICON, (h, t, name, lParam) =>
                    {
                        IntPtr grpRes = FindResource(h, name, RT_GROUP_ICON);
                        if (grpRes == IntPtr.Zero) return true;

                        IntPtr grpResLock = LoadResource(h, grpRes);
                        if (grpResLock == IntPtr.Zero) return true;

                        IntPtr grpPtr = LockResource(grpResLock);
                        if (grpPtr == IntPtr.Zero) return true;

                        var grpHeader = Marshal.PtrToStructure<GRPICONDIR>(grpPtr);
                        int count = grpHeader.Count;

                        long entryOffset = Marshal.SizeOf<GRPICONDIR>();
                        GRPICONDIRENTRY[] entries = new GRPICONDIRENTRY[count];

                        for (int i = 0; i < count; i++)
                        {
                            IntPtr entryPtr = IntPtr.Add(grpPtr, (int)entryOffset);
                            entries[i] = Marshal.PtrToStructure<GRPICONDIRENTRY>(entryPtr);
                            entryOffset += Marshal.SizeOf<GRPICONDIRENTRY>();
                        }

                        // Pick the best entry in this group
                        GRPICONDIRENTRY bestEntryInGroup = default;
                        int bestEntryScore = -1;

                        foreach (var e in entries)
                        {
                            int realWidth = (e.Width == 0 ? 256 : e.Width);
                            int realHeight = (e.Height == 0 ? 256 : e.Height);
                            int area = realWidth * realHeight;
                            int bitCount = e.BitCount;

                            int currentScore = area * 100 + bitCount;
                            if (currentScore > bestEntryScore)
                            {
                                bestEntryScore = currentScore;
                                bestEntryInGroup = e;
                            }
                        }

                        // Now build an .ico in memory for the best entry
                        using (var icoStream = new MemoryStream())
                        using (var writer = new BinaryWriter(icoStream))
                        {
                            writer.Write((short)0); // reserved
                            writer.Write((short)1); // type=1
                            writer.Write((short)1); // count=1

                            // IconDir entry
                            byte w = bestEntryInGroup.Width == 0 ? (byte)255 : bestEntryInGroup.Width;
                            byte ht = bestEntryInGroup.Height == 0 ? (byte)255 : bestEntryInGroup.Height;

                            int imageOffset = Marshal.SizeOf<ICONDIR>() + Marshal.SizeOf<ICONDIRENTRY>();
                            writer.Write(w);
                            writer.Write(ht);
                            writer.Write(bestEntryInGroup.ColorCount);
                            writer.Write(bestEntryInGroup.Reserved);
                            writer.Write(bestEntryInGroup.Planes);
                            writer.Write(bestEntryInGroup.BitCount);
                            writer.Write(bestEntryInGroup.BytesInRes);
                            writer.Write(imageOffset);

                            // Find RT_ICON resource
                            IntPtr iconRes = FindResource(h, (IntPtr)bestEntryInGroup.ID, RT_ICON);
                            if (iconRes != IntPtr.Zero)
                            {
                                IntPtr iconResLock = LoadResource(h, iconRes);
                                IntPtr iconPtr = LockResource(iconResLock);
                                int sizeRes = SizeofResource(h, iconRes);

                                byte[] buffer = new byte[sizeRes];
                                Marshal.Copy(iconPtr, buffer, 0, sizeRes);
                                writer.Write(buffer, 0, sizeRes);
                            }

                            writer.Flush();
                            icoStream.Position = 0;

                            using (var candidateIcon = new Icon(icoStream))
                            {
                                int globalScore = bestEntryScore;
                                if (globalScore > bestScoreSoFar)
                                {
                                    bestScoreSoFar = globalScore;
                                    if (bestIconSoFar != null)
                                        bestIconSoFar.Dispose();

                                    bestIconSoFar = (Icon)candidateIcon.Clone();
                                }
                            }
                        }

                        return true;
                    }, IntPtr.Zero);
                }
                finally
                {
                    FreeLibrary(hModule);
                }

                return bestIconSoFar;
            }
        }

        private void AnimateSelection(ProgramRow row, bool isSelected)
        {
            if (row == null) return;

            // Ensure we have a CompositeTransform
            var transform = row.ColumnPanel.RenderTransform as CompositeTransform;
            if (transform == null)
            {
                transform = new CompositeTransform
                {
                    ScaleX = 1.0,
                    ScaleY = 1.0,
                    TranslateX = 0,
                    TranslateY = 0
                };
                row.ColumnPanel.RenderTransform = transform;
            }

            // Center the pivot
            row.ColumnPanel.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);

            // Target values
            double targetScale = isSelected ? 1.10 : 1.0;   // a bit bigger
            double targetTranslateY = isSelected ? -15.0 : 0.0; // move up 15px if selected

            // If already at the right scale/position, do nothing
            if (Math.Abs(transform.ScaleX - targetScale) < 0.001
                && Math.Abs(transform.TranslateY - targetTranslateY) < 0.001)
            {
                return;
            }

            // Build a Storyboard
            var storyboard = new Storyboard();
            var duration = TimeSpan.FromMilliseconds(150);

            // Easing pour un “rebond lent”
            var bounceEase = new BounceEase
            {
                Bounces = 2,
                Bounciness = 1.5,
                EasingMode = EasingMode.EaseOut
            };

            // 1) ScaleX
            var scaleXAnim = new DoubleAnimation
            {
                To = targetScale,
                Duration = new Duration(duration),
            };
            Storyboard.SetTarget(scaleXAnim, row.ColumnPanel);
            Storyboard.SetTargetProperty(scaleXAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            storyboard.Children.Add(scaleXAnim);

            // 2) ScaleY
            var scaleYAnim = new DoubleAnimation
            {
                To = targetScale,
                Duration = new Duration(duration),
            };
            Storyboard.SetTarget(scaleYAnim, row.ColumnPanel);
            Storyboard.SetTargetProperty(scaleYAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            storyboard.Children.Add(scaleYAnim);

            // 3) TranslateY pour le faire monter
            var translateYAnim = new DoubleAnimation
            {
                To = targetTranslateY,
                Duration = new Duration(duration),
            };
            Storyboard.SetTarget(translateYAnim, row.ColumnPanel);
            Storyboard.SetTargetProperty(translateYAnim, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
            storyboard.Children.Add(translateYAnim);

            // Start
            storyboard.Begin();
        }

        #endregion

        #region Gamepad/Keyboard_Navigation

        private Controller _xinputController;
        private bool _controllerConnected = false;

        private void SetupGamepad()
        {
            _xinputController = new Controller(UserIndex.One);
            _controllerConnected = _xinputController.IsConnected;

            DispatcherTimer gamepadInputTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
            gamepadInputTimer.Tick += (s, e) => GamepadButtonCheck();
            gamepadInputTimer.Start();
        }

        private void GamepadButtonCheck()
        {
            if (!_controllerConnected || !_xinputController.IsConnected)
                return;

            var state = _xinputController.GetState();
            var gamepad = state.Gamepad;

            if ((gamepad.Buttons & GamepadButtonFlags.DPadDown) != 0)
            {
                MoveAction(1);
            }
            else if ((gamepad.Buttons & GamepadButtonFlags.DPadUp) != 0)
            {
                MoveAction(-1);
            }
            else if ((gamepad.Buttons & GamepadButtonFlags.DPadLeft) != 0)
            {
                MoveApp(-1);
            }
            else if ((gamepad.Buttons & GamepadButtonFlags.DPadRight) != 0)
            {
                MoveApp(1);
            }
            else if ((gamepad.Buttons & GamepadButtonFlags.A) != 0)
            {
                ExecuteSelectedAction();
            }
            else if ((gamepad.Buttons & GamepadButtonFlags.Start) != 0 &&
                     (gamepad.Buttons & GamepadButtonFlags.Back) != 0)
            {
                BringWindowToForeground();
            }
        }




        private void MainWindow_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Down:
                    MoveAction(1);
                    break;
                case VirtualKey.Up:
                    MoveAction(-1);
                    break;
                case VirtualKey.Left:
                    MoveApp(-1);
                    break;
                case VirtualKey.Right:
                    MoveApp(1);
                    break;
                case VirtualKey.Enter:
                    ExecuteSelectedAction();
                    break;
            }
        }

        private void BringWindowToForeground()
        {
            try
            {
                Process currentProcess = Process.GetCurrentProcess();
                IntPtr hWnd = currentProcess.MainWindowHandle; // Récupérer la vraie fenêtre principale
                Console.WriteLine($"gcmloader Window detected : {hWnd}");

                if (hWnd == IntPtr.Zero)
                {
                    Console.WriteLine("gcmloader not found !");
                    return;
                }

                // Vérifier si une autre fenêtre est déjà en premier plan
                IntPtr foregroundHwnd = GetForegroundWindow();
                if (foregroundHwnd == IntPtr.Zero)
                {
                    Console.WriteLine("No window in foreground!");
                    return;
                }

                uint activeThreadId = GetWindowThreadProcessId(foregroundHwnd, out uint activeProcessId);
                uint currentThreadId = GetCurrentThreadId();

                // Attacher l'entrée du thread actif à notre fenêtre
                AttachThreadInput(currentThreadId, activeThreadId, true);

                // Afficher la fenêtre si elle est minimisée
                ShowWindow(hWnd, 9); // SW_RESTORE

                // Mettre la fenêtre en avant
                SetForegroundWindow(hWnd);

                // Détacher les threads d'entrée
                AttachThreadInput(currentThreadId, activeThreadId, false);

                // Permettre au programme de prendre le focus
                AllowSetForegroundWindow(-1);

                Console.WriteLine("gcmloader brought to the foreground");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur BringWindowToForeground : {ex.Message}");
            }
        }
        #endregion

        #region Startupvideo

        public static class StartupVideo
        {
            [DllImport("user32.dll")]
            private static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
            private const uint SWP_NOSIZE = 0x0001;
            private const uint SWP_NOMOVE = 0x0002;
            private const uint SWP_SHOWWINDOW = 0x0040;

            #region SteamStartup

            public static void RenameFile(string oldFilePath, string newFilePath)
            {
                try
                {
                    // Vérifie si le fichier existe
                    if (File.Exists(oldFilePath))
                    {
                        // Renomme le fichier
                        File.Move(oldFilePath, newFilePath);
                        Console.WriteLine($"Le fichier a été renommé avec succès : {newFilePath}");
                    }
                    else
                    {
                        Console.WriteLine("Le fichier spécifié n'existe pas.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du renommage du fichier : {ex.Message}");
                }
            }

            public static void RenameSteamStartupVideo_Start()
            {
                string SteamVideoPath = Path.Combine(Path.GetDirectoryName(AppSettings.Load<string>("steamlauncherpath")), "steamui", "movies", "bigpicture_startup.webm");
                string SteamVideoPathNew = Path.Combine(Path.GetDirectoryName(AppSettings.Load<string>("steamlauncherpath")), "steamui", "movies", "bigpicture_startup.old.webm");
                string GCMVideoPath = Path.Combine(Path.GetDirectoryName(AppSettings.Load<string>("steamlauncherpath")), "steamui", "movies", "GCM_vid.webm");
                RenameFile(SteamVideoPath, SteamVideoPathNew); //change the name of the real file
                RenameFile(GCMVideoPath, SteamVideoPath); //put the name of the real file to the selected video
            }

            public static void RenameSteamStartupVideo_End()
            {
                string SteamVideoPath = Path.Combine(Path.GetDirectoryName(AppSettings.Load<string>("steamlauncherpath")), "steamui", "movies", "bigpicture_startup.webm");
                string SteamVideoPathNew = Path.Combine(Path.GetDirectoryName(AppSettings.Load<string>("steamlauncherpath")), "steamui", "movies", "bigpicture_startup.old.webm");
                string GCMVideoPath = Path.Combine(Path.GetDirectoryName(AppSettings.Load<string>("steamlauncherpath")), "steamui", "movies", "GCM_vid.webm");
                RenameFile(SteamVideoPath, GCMVideoPath); // give the GCM Video file its real name
                RenameFile(SteamVideoPathNew, SteamVideoPath); // give the steam file its real name
            }
            #endregion SteamStartup

            public static void Play()
            {
                try
                {
                    Console.WriteLine("Playing startup video...");
                    // Check if startup video is enabled
                    bool useStartupVideo = AppSettings.Load<bool>("usestartupvideo");
                    if (!useStartupVideo)
                        return;

                    if (AppSettings.Load<bool>("usesteamstartupvideo"))
                    {
                        RenameSteamStartupVideo_Start();
                    }
                    else
                    {

                        // Load the video path
                        string videoPath = AppSettings.Load<string>("startupvideo_path");
                        if (string.IsNullOrEmpty(videoPath) || !File.Exists(videoPath))
                        {
                            ShowErrorMessage("The specified video file was not found.");
                            return;
                        }

                        // Check the file extension
                        string extension = Path.GetExtension(videoPath)?.ToLower();
                        string[] validExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv" };
                        if (Array.IndexOf(validExtensions, extension) == -1)
                        {
                            ShowErrorMessage("Unsupported video format.");
                            return;
                        }

                        // Create the video playback window
                        var videoWindow = new Window();
                        var appWindow = GetAppWindow(videoWindow);

                        if (appWindow != null)
                        {
                            var presenter = appWindow.Presenter as OverlappedPresenter;
                            if (presenter != null)
                            {
                                presenter.IsMaximizable = false;
                                presenter.IsMinimizable = false;
                                presenter.IsResizable = false;
                                presenter.SetBorderAndTitleBar(false, false);
                            }
                            appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                        }

                        var mediaElement = CreateMediaElement(videoPath, videoWindow);
                        videoWindow.Content = mediaElement;
                        videoWindow.Activate();

                        //Forcer la fenêtre au premier plan
                        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(videoWindow);
                        SetForegroundWindow(hWnd);
                        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);

                        //Maintenir la fenêtre au premier plan
                        KeepWindowOnTop(hWnd);

                    }


                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Error playing video: {ex.Message}");
                }
            }

            private static MediaPlayerElement CreateMediaElement(string videoPath, Window window)
            {
                var mediaPlayer = new MediaPlayer { AutoPlay = true };
                mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(videoPath));

                // Fermer correctement la fenêtre après la lecture
                mediaPlayer.MediaEnded += (s, e) =>
                {
                    window.DispatcherQueue.TryEnqueue(() =>
                    {
                        window.Close();
                    });
                };

                var mediaPlayerElement = new MediaPlayerElement
                {
                    AreTransportControlsEnabled = false,
                    Stretch = Microsoft.UI.Xaml.Media.Stretch.UniformToFill
                };

                mediaPlayerElement.SetMediaPlayer(mediaPlayer);
                return mediaPlayerElement;
            }

            private static AppWindow GetAppWindow(Window window)
            {
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
                return AppWindow.GetFromWindowId(windowId);
            }

            private static async void KeepWindowOnTop(IntPtr hWnd)
            {
                while (true)
                {
                    await Task.Delay(1000); // Vérifie toutes les secondes
                    SetForegroundWindow(hWnd);
                    SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
                }
            }

            private static void ShowErrorMessage(string message)
            {
                Console.WriteLine(message);
            }
        }

        #endregion Startupvideo
        #endregion methodes
    }
}
