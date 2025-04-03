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
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Point = System.Drawing.Point;



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
            this.Content.KeyUp += MainWindow_KeyUp;
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

        #region flowlauncher
      
        private void SendAltSpace()
        {
            // Press ALT
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);

            // Press SPACE
            keybd_event(VK_SPACE, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            keybd_event(VK_SPACE, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            // Release ALT
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            Console.WriteLine("ALT + SPACE simulated.");
        }


        public async void flowlauncher()
        {

            bool launcher = AppSettings.Load<bool>("useflowlauncher");

            if (launcher == true)
            {
                // Get the base directory where the app was started from
                string basePath = AppContext.BaseDirectory;

                // Build full path to the Flow Launcher executable
                string flowLauncherPath = Path.Combine(basePath, "flowlauncher", "Flow.Launcher.exe");

                // Check if the file exists
                if (File.Exists(flowLauncherPath))
                {
                    try
                    {
                        // Start the Flow Launcher executable
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = flowLauncherPath,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error starting Flow Launcher: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Flow Launcher executable not found.");
                }
            }
            else
            {

                Console.WriteLine("flowlauncher is off or not set");
            }
        }

        #endregion flowlauncher
        public void prestartlist()
        {
            try
            {
                bool prestartlist = AppSettings.Load<bool>("usepreloadlist");

                if (prestartlist == true)
                {
                    string prestartlistpath = AppSettings.Load<string>("prealoadlistpath");

                    if (File.Exists(prestartlistpath))
                    {
                        string[] lines = File.ReadAllLines(prestartlistpath);

                        foreach (var line in lines)
                        {
                            string entry = line.Trim();

                            // Skip empty lines or comments
                            if (string.IsNullOrWhiteSpace(entry) || entry.StartsWith("#"))
                                continue;

                            try
                            {
                                // Open URLs in default browser
                                if (entry.StartsWith("http://") || entry.StartsWith("https://"))
                                {
                                    Process.Start(new ProcessStartInfo
                                    {
                                        FileName = entry,
                                        UseShellExecute = true
                                    });
                                }
                                // Run executable files
                                else if (entry.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (File.Exists(entry))
                                    {
                                        Process.Start(new ProcessStartInfo
                                        {
                                            FileName = entry,
                                            UseShellExecute = true
                                        });
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Executable not found: {entry}");
                                    }
                                }
                                // Open other files (e.g., images, txt, etc.)
                                else if (File.Exists(entry))
                                {
                                    Process.Start(new ProcessStartInfo
                                    {
                                        FileName = entry,
                                        UseShellExecute = true
                                    });
                                }
                                else
                                {
                                    Console.WriteLine($"File not found: {entry}");
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log error but continue
                                Console.WriteLine($"Error with entry '{entry}': {ex.Message}");
                            }
                        }

                        Console.WriteLine("Finished running preload list.");
                    }
                    else
                    {
                        Console.WriteLine("prestartlist not found");
                    }
                }
                else
                {
                    Console.WriteLine("no prestartlist set");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled error in preload list processing: {ex.Message}");
            }
        }
        public void preaudio(bool start,bool end)
        {
            try
            {
                bool preaudio = AppSettings.Load<bool>("usepreaudio");
                if (preaudio == true)
                {
                    if (end == true)
                    {
                        string preaudioend = AppSettings.Load<string>("preaudioend");
                        NirCmdUtil.NirCmdHelper.ExecuteCommand($"setdefaultsounddevice \"{preaudioend}\"");
                    }
                    else if (start == true)
                    {
                        string preaudiostart = AppSettings.Load<string>("preaudiostart");
                        NirCmdUtil.NirCmdHelper.ExecuteCommand($"setdefaultsounddevice \"{preaudiostart}\"");
                    }
                }
                else
                {
                    Console.WriteLine("no preaudio set");
                }
            }
            catch
            {

            }
        }
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

                        // Fill the entire space while maintaining aspect ratio
                        backgroundImage.Stretch = Stretch.UniformToFill;

                        // Set image to stretch automatically by removing fixed Width and Height
                        backgroundImage.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch;
                        backgroundImage.VerticalAlignment = VerticalAlignment.Stretch;

                        // Make sure the image is behind all other elements
                        backgroundImage.SetValue(Canvas.ZIndexProperty, -1);

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
        static void KillTargetProcess(string processName)
        {
            // Get all processes with the specified name
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process proc in processes)
            {
                try
                {
                    // Attempt to kill the process
                    proc.Kill();
                    Console.WriteLine($"Terminated process: {proc.ProcessName} (ID: {proc.Id})");
                }
                catch (Exception ex)
                {
                    // Log if something goes wrong
                    Console.WriteLine($"Could not terminate process {proc.ProcessName}: {ex.Message}");
                }
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

                        //End Decky Loader process if running
                        Process[] deckyLoaderProcesses = Process.GetProcessesByName("PluginLoader_noconsole");
                        if (deckyLoaderProcesses.Length > 0)
                        {
                            foreach (var process in deckyLoaderProcesses)
                            {
                                process.Kill();
                                process.WaitForExit();
                                Console.WriteLine("Decky Loader process killed successfully.");
                            }
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

                        // Try to load deckyloader setting
                        bool usedeckyloader = false;

                        try
                        {
                            // Try to load the value from settings
                            usedeckyloader = AppSettings.Load<bool>("usedeckyloader");
                        }
                        catch
                        {
                            // Key doesn't exist or loading failed → set to false
                            AppSettings.Save("usedeckyloader", false);
                            usedeckyloader = false;
                        }

                        // Now handle logic cleanly
                        if (usedeckyloader)
                        {
                            // deckyloader is enabled
                            Console.WriteLine("DeckyLoader is enabled");
                        }
                        else
                        {
                            // deckyloader is disabled or was not set and now defaulted
                            Console.WriteLine("DeckyLoader is disabled or not set");
                            //set deckyloader disabled
                            AppSettings.Save("usedeckyloader", false);
                        }


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
                //flowlauncher autokill



            }
            catch (Exception ex)
            {

            }
        }
        public void deckyloader()
        { 
        
        
        
        }
        #region discord 
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
                                    NirCmdUtil.NirCmdHelper.ExecuteCommand($"setdefaultsounddevice \"{enddiscord}\"");
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
                                    NirCmdUtil.NirCmdHelper.ExecuteCommand($"setdefaultsounddevice \"{enddiscord}\"");
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
                                    NirCmdUtil.NirCmdHelper.ExecuteCommand($"setdefaultsounddevice \"{startdiscord}\"");
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

            //for deckyloader
            bool deckyloadertrigger = false;

            if (string.IsNullOrWhiteSpace(AppSettings.Load<string>("steamlauncherpath")) || !File.Exists(AppSettings.Load<string>("steamlauncherpath")))
            {
                Console.WriteLine("Error: SteamPath is empty, invalid, or does not exist.");
                BackToWindows();
                return;
            }

            KillProcess("steam.exe");
            Console.WriteLine("try start Steam");

            // Check if decky Loader is activated
            if (AppSettings.Load<bool>("usedeckyloader") == true)
            {

                deckyloadertrigger = true;
                //first clear other process with deckyloader
                //End Decky Loader process if running
                Process[] deckyLoaderProcesses = Process.GetProcessesByName("PluginLoader_noconsole");
                if (deckyLoaderProcesses.Length > 0)
                {
                    foreach (var process in deckyLoaderProcesses)
                    {
                        process.Kill();
                        process.WaitForExit();
                        Console.WriteLine("Decky Loader process killed successfully.");
                    }
                }
                //set the trigger for Deckyloader
               

                //Start Decky Loader Steam
                //search and start decky loader no console for steam.
                // Get the user's home directory dynamically
                string userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                // Construct the full path to PluginLoader_noconsole.exe
                string pluginLoaderPath = Path.Combine(userHome, "homebrew", "services", "PluginLoader_noconsole.exe");

                // Check if the executable file exists
                if (File.Exists(pluginLoaderPath))
                {
                    Console.WriteLine("PluginLoader_noconsole.exe found. Starting...");

                    // Start the process
                    Process process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = pluginLoaderPath,
                            UseShellExecute = true // Ensures the executable runs properly
                        }
                    };
                    process.Start();

                    // Wait until the process is running and not exited
                    while (true)
                    {
                        process.Refresh(); // Update the process info

                        if (process.HasExited)
                        {
                            Console.WriteLine("PluginLoader exited unexpectedly.");
                            break;
                        }

                        try
                        {
                            // Check if the process has a valid start time (indicates it has initialized)
                            var _ = process.StartTime;
                            break;
                        }
                        catch
                        {
                            // StartTime not yet available, wait and try again
                        }

                        Thread.Sleep(1000); // Wait a bit before checking again
                    }

                    Console.WriteLine("PluginLoader is running. Continuing...");

                    try
                    {
                        string Path = AppSettings.Load<string>("steamlauncherpath");
                        string arguments;
                        //  if (AppSettings.Load<bool>("usestartupvideo")){
                        // arguments = "-gamepadui -noverifyfiles -nobootstrapupdate -skipinitialbootstrap -overridepackageurl";
                        //  }
                        //  else
                        //  {
                        arguments = "-dev -gamepadui -noverifyfiles -nobootstrapupdate -skipinitialbootstrap -overridepackageurl -noinstro ";
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
                else
                {
                    Console.WriteLine("Error: PluginLoader_noconsole.exe not found start normal!");
                    // Start Steam normal
                    try
                    {
                        string Path = AppSettings.Load<string>("steamlauncherpath");
                        string arguments;
                        //  if (AppSettings.Load<bool>("usestartupvideo")){
                        // arguments = "-gamepadui -noverifyfiles -nobootstrapupdate -skipinitialbootstrap -overridepackageurl";
                        //  }
                        //  else
                        //  {
                        arguments = "-gamepadui -noverifyfiles -nobootstrapupdate -skipinitialbootstrap -overridepackageurl -nointro";
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

              
            }
            else
            {
          
            }

            if (deckyloadertrigger == true) // deckyloader is not triggered
            {
            }
            else if (deckyloadertrigger == false)
            {
                // Start Steam normal
                try
                {
                    string Path = AppSettings.Load<string>("steamlauncherpath");
                    string arguments;
                    //  if (AppSettings.Load<bool>("usestartupvideo")){
                    // arguments = "-gamepadui -noverifyfiles -nobootstrapupdate -skipinitialbootstrap -overridepackageurl";
                    //  }
                    //  else
                    //  {
                    arguments = "-gamepadui -noverifyfiles -nobootstrapupdate -skipinitialbootstrap -overridepackageurl -nointro";
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
                    #region kill distubing process
                    KillTargetProcess("JoyxSvc");
                    KillTargetProcess("JoyXoff");
                    #endregion kill distubing process
                    cssloader(); //only check if is installed, than start
                    flowlauncher();
                    StartLauncher();
                    SetupGamepad();
                    // TaskManager //
                    LoadTaskManagerList();
                    InitializeTaskManagerRefresh();
                    ///////////////
                    ConsoleModeToShell();
                    LoadTaskManagerList();
                    preaudio(true,false);
                    prestartlist();
                    await Task.Run(() =>
                    {
                        WaitForLauncherToClose();

                    });
                    try
                    {
                        StartupVideo.RenameSteamStartupVideo_End();
                    }
                    catch { }
                    preaudio(false, true);
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
            public StackPanel RowPanel;   // the horizontal "row"
            public TextBlock NameText;    // the name of the application
            public Button FocusButton;    // Focus button
            public Button KillButton;     // Kill button

            public IntPtr Hwnd;           // window handle
            public Process Proc;          // corresponding Process
        }

        // Index for 2D navigation: _selectedRow (row) and _selectedCol (column)
        private int _selectedRow = 0;     // current row index
        private int _selectedCol = 0;     // 0 = Focus, 1 = Kill
        private DateTime _lastInputTime = DateTime.Now;

        // Stores whether the window is currently in the foreground
        private bool _isForeground = false;

        // Timer to refresh the list every second
        private DispatcherTimer _refreshTimer;

        private List<ProgramRow> _rows = new List<ProgramRow>();

        // AppWindow, for detecting minimized / non-minimized
        private AppWindow _appWindow;

        // ====================================================================
        // Initializes the window and the refresh timer
        // ====================================================================
        private void InitializeTaskManagerRefresh()
        {
            // 1) Retrieve the AppWindow
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = AppWindow.GetFromWindowId(windowId);

            if (_appWindow != null)
            {
                _appWindow.Changed += OnAppWindowChanged;
            }

            // 2) One-second timer for refresh
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
            // Creates a timer for 10 seconds
            var hideTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            hideTimer.Tick += (s, e) =>
            {
                // Stops the timer and shows the StackPanel
                hideTimer.Stop();
                TaskManagerPanel.Visibility = Visibility.Visible;
            };
            hideTimer.Start();
        }

        // ====================================================================
        // Loads the list of applications
        // ====================================================================
        private void LoadTaskManagerList()
        {
            if (TaskManagerPanel == null) return;

            TaskManagerPanel.Children.Clear();
            _rows.Clear();

            // Enumerates windows (P/Invoke already declared elsewhere)
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    // Retrieve the process
                    uint pid;
                    GetWindowThreadProcessId(hWnd, out pid);

                    Process p;
                    try
                    {
                        p = Process.GetProcessById((int)pid);
                    }
                    catch
                    {
                        return true;
                    }

                    // Ignore if it's GCMLoader
                    if (pid == (uint)Process.GetCurrentProcess().Id)
                        return true;

                    // Program name
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

                    // Exclusions
                    if (productName?.Contains("Windows", StringComparison.OrdinalIgnoreCase) == true
                        || p.ProcessName.Equals("explorer", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    if (productName?.Contains("Steam Client WebHelper", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        productName = "Steam";
                    }

                    // Builds the row
                    var row = CreateProgramRow(productName, p, hWnd);
                    TaskManagerPanel.Children.Add(row.RowPanel);

                    _rows.Add(row);
                }
                return true;
            }, IntPtr.Zero);

            if (_rows.Count > 0)
            {
                if (_selectedRow >= _rows.Count)
                    _selectedRow = _rows.Count - 1;
                if (_selectedCol > 1)
                    _selectedCol = 0;

                UpdateRowSelection();
            }
        }

        private ProgramRow CreateProgramRow(string programName, Process proc, IntPtr hwnd)
        {
            double fontSize = 24;
            int buttonWidth = 120;
            int rowHeight = 80;
            var margin = new Thickness(0);
            var buttonmargin = new Thickness(10);

            // Creation of the row
            var rowPanel = new StackPanel
            {
                Orientation = Microsoft.UI.Xaml.Controls.Orientation.Horizontal,
                Margin = margin,
                Height = rowHeight
            };

            var nameText = new TextBlock
            {
                Text = programName,
                FontSize = fontSize,
                Width = 400,
                Margin = new Thickness(20, 0, 20, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            // Base color (very dark) + white text
            var baseGrey = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 30, 30, 30));
            var baseText = new SolidColorBrush(Colors.White);

            // Color when selected = lighter gray + black text
            var highlightGrey = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 210, 210, 210));
            var highlightText = new SolidColorBrush(Colors.Black);

            var focusButton = new Button
            {
                Content = "Show",
                FontSize = fontSize,
                Width = buttonWidth,
                Margin = buttonmargin,
                Background = baseGrey,
                Foreground = baseText,
                Visibility = Visibility.Collapsed
            };
            focusButton.Click += (s, e) => SetForegroundWindow(hwnd);

            var killButton = new Button
            {
                Content = "Close",
                FontSize = fontSize,
                Width = buttonWidth,
                Margin = margin,
                Background = baseGrey,
                Foreground = baseText,
                Visibility = Visibility.Collapsed
            };
            killButton.Click += (s, e) =>
            {
                try { proc.Kill(); } catch { }
                LoadTaskManagerList();
            };

            rowPanel.Children.Add(nameText);
            rowPanel.Children.Add(focusButton);
            rowPanel.Children.Add(killButton);

            return new ProgramRow
            {
                RowPanel = rowPanel,
                NameText = nameText,
                FocusButton = focusButton,
                KillButton = killButton,
                Hwnd = hwnd,
                Proc = proc
            };
        }

        private void UpdateRowSelection()
        {
            // Base color (very dark) + white text
            var baseGrey = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 30, 30, 30));
            var baseText = new SolidColorBrush(Colors.White);

            // Color when selected = lighter gray + black text
            var highlightGrey = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 210, 210, 210));
            var highlightText = new SolidColorBrush(Colors.Black);

            for (int i = 0; i < _rows.Count; i++)
            {
                var row = _rows[i];
                if (i == _selectedRow)
                {
                    // Apply a lighter background if you wish to color the entire selected row
                    row.RowPanel.Background = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 50, 50, 50));

                    // Show the buttons
                    row.FocusButton.Visibility = Visibility.Visible;
                    row.KillButton.Visibility = Visibility.Visible;

                    // Standard color (very dark gray + white text)
                    row.FocusButton.Background = baseGrey;
                    row.FocusButton.Foreground = baseText;
                    row.KillButton.Background = baseGrey;
                    row.KillButton.Foreground = baseText;

                    // Highlight the selected button (Focus or Kill)
                    if (_selectedCol == 0)
                    {
                        row.FocusButton.Background = highlightGrey;
                        row.FocusButton.Foreground = highlightText;
                    }
                    else
                    {
                        row.KillButton.Background = highlightGrey;
                        row.KillButton.Foreground = highlightText;
                    }
                }
                else
                {
                    // Non-selected row:
                    row.RowPanel.Background = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 30, 30, 30));

                    // Hide the buttons
                    row.FocusButton.Visibility = Visibility.Collapsed;
                    row.KillButton.Visibility = Visibility.Collapsed;

                    // Revert to the base color (dark gray + white text)
                    row.FocusButton.Background = baseGrey;
                    row.FocusButton.Foreground = baseText;
                    row.KillButton.Background = baseGrey;
                    row.KillButton.Foreground = baseText;
                }
            }
        }

        private void MoveRow(int delta)
        {
            if (IsWindowInForeground()) // Checks if the window is in the foreground
            {
                PlayNavigationSound();
                if (_rows.Count == 0) return;

                _selectedRow += delta;
                if (_selectedRow < 0)
                    _selectedRow = _rows.Count - 1;
                else if (_selectedRow >= _rows.Count)
                    _selectedRow = 0;

                UpdateRowSelection();
            }
        }

        private void MoveCol(int delta)
        {
            if (IsWindowInForeground()) // Checks if the window is in the foreground
            {
                PlayNavigationSound();
                _selectedCol += delta;
                if (_selectedCol < 0) _selectedCol = 1;
                else if (_selectedCol > 1) _selectedCol = 0;

                UpdateRowSelection();
            }
        }


        private void ExecuteSelectedAction()
        {
            if (IsWindowInForeground()) // Checks if the window is in the foreground
            {
                PlayActivationSound();
                if (_selectedRow < 0 || _selectedRow >= _rows.Count) return;

                var row = _rows[_selectedRow];
                if (_selectedCol == 0)
                {
                    // Focus
                    ShowWindow(row.Hwnd, SW_RESTORE);
                    SetForegroundWindow(row.Hwnd);
                }
                else
                {
                    // Kill
                    try { row.Proc.Kill(); } catch { }
                    LoadTaskManagerList();
                }
            }
        }

        private void PlayNavigationSound()
        {
            try
            {
                SoundPlayer player = new SoundPlayer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\navigation.wav"));
                player.Play();
            }
            catch { }
        }

        private void PlayActivationSound()
        {
            try
            {
                SoundPlayer player = new SoundPlayer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\activation.wav"));
                player.Play();
            }
            catch { }
        }

        #endregion // TaskManager
        #region Gamepad/Keyboard_Navigation
        #region shortcuts
        #region alt tab
        private const byte VK_MENU = 0x12; // ALT key
        private const byte VK_TAB = 0x09;
        private const byte VK_SPACE = 0x20;
        private const byte VK_R = 0x52;    // R


        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;
       
        private void SendAltTab()
        {
            // Press ALT
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);

            // Press TAB
            keybd_event(VK_TAB, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            // Release ALT
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        #endregion alt tab
        #region performance overlay shortcut AHK
        private void TriggerPerformanceOverlay()
        {
            // Build full path to the overlay .exe located in the same folder 
            string overlayPath = Path.Combine(AppContext.BaseDirectory, "amdnvidiap.exe");

            if (File.Exists(overlayPath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = overlayPath,
                        UseShellExecute = true
                    });

                    Console.WriteLine("Performance overlay trigger executed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error launching overlay trigger: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Overlay trigger executable not found.");
            }
        }

        #endregion performance overlay shortcut AHK
        #region audio management
        public static void SwitchToNextAudioDevice()
        {
            try
            {
                var enumerator = new MMDeviceEnumerator();
                var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                string currentName = defaultDevice.FriendlyName;

                // Get all active playback devices and exclude Steam-related devices
                List<MMDevice> devices = enumerator
                    .EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                    .Where(d => !d.FriendlyName.ToLower().Contains("steam streaming"))
                    .ToList();

                List<string> deviceNames = devices.Select(d => d.FriendlyName).ToList();

                int currentIndex = deviceNames.FindIndex(name => name.Equals(currentName, StringComparison.OrdinalIgnoreCase));
                int nextIndex = (currentIndex + 1) % deviceNames.Count;
                string rawDeviceName = deviceNames[nextIndex];
                string cleanedDeviceName = rawDeviceName.Split('(')[0].Trim();
                NirCmdUtil.NirCmdHelper.ExecuteCommand($"setdefaultsounddevice \"{cleanedDeviceName}\"");
                Console.WriteLine($"Switched to audio device: {cleanedDeviceName}");
                NativeToastOverlay.Show("Switched to: " + cleanedDeviceName, SystemIcons.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error switching audio device: {ex.Message}");
            }
        }
        #endregion audio management
        #region shortcut overlay
      
        public static void ShowOverlayControlsTemporarily()
        {
          

        }
        #endregion shortcut overlay
        #endregion shortcuts


        private Controller _xinputController;
        private bool _controllerConnected = false;

        private void SetupGamepad()
        {
            _xinputController = new Controller(UserIndex.One);
            _controllerConnected = _xinputController.IsConnected;

            // Create and start a timer that checks for controller input/connection
            DispatcherTimer gamepadInputTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(140)
            };

            gamepadInputTimer.Tick += (s, e) =>
            {
                // If controller is not connected yet, check for it
                if (!_controllerConnected)
                {
                    _controllerConnected = _xinputController.IsConnected;

                    if (_controllerConnected)
                    {
                        // Controller has just been connected
                        Debug.WriteLine("Controller connected!");
                    }
                }

                // Run the gamepad button check continuously if controller is connected
                if (_controllerConnected)
                {
                    GamepadButtonCheck();
                }
            };

            gamepadInputTimer.Start();
        }

        //Taskmanager
        private GamepadButtonFlags _lastButtonState = GamepadButtonFlags.None;
        //overlay ui
        private static overlaycontrolls _overlayInstance;

        private void GamepadButtonCheck()
        {
            if (!_controllerConnected || !_xinputController.IsConnected)
                return;

            var state = _xinputController.GetState();
            var gamepad = state.Gamepad;
           
            // Only react when button state has changed from the last check
            var currentButtons = gamepad.Buttons;
            var newButtons = currentButtons & ~_lastButtonState; // Only buttons that are newly pressed

            if ((newButtons & GamepadButtonFlags.DPadDown) != 0)
            {
                MoveRow(1);
            }
            else if ((newButtons & GamepadButtonFlags.DPadUp) != 0)
            {
                MoveRow(-1);
            }
            else if ((newButtons & GamepadButtonFlags.DPadLeft) != 0)
            {
                MoveCol(-1);
            }
            else if ((newButtons & GamepadButtonFlags.DPadRight) != 0)
            {
                MoveCol(1);
            }
            else if ((newButtons & GamepadButtonFlags.A) != 0)
            {
                ExecuteSelectedAction();
            }
            else if ((newButtons & GamepadButtonFlags.Back) != 0 &&
                     (newButtons & GamepadButtonFlags.Start) != 0)
            {
                BringWindowToForeground();
            }
            else if ((newButtons & GamepadButtonFlags.Back) != 0 &&
                     (newButtons & GamepadButtonFlags.Y) != 0)
            {
                SendAltTab();
               
            }
            else if ((newButtons & GamepadButtonFlags.Back) != 0 &&
                     (newButtons & GamepadButtonFlags.X) != 0)
            {
                TriggerPerformanceOverlay();
            }
            else if ((newButtons & GamepadButtonFlags.Back) != 0 &&
                    (newButtons & GamepadButtonFlags.RightThumb) != 0)
            {
                SwitchToNextAudioDevice();
            }
            else if ((newButtons & GamepadButtonFlags.Back) != 0 &&
         (newButtons & GamepadButtonFlags.B) != 0)
            {
                if (_overlayInstance == null)
                {
                    _overlayInstance = new overlaycontrolls();
                    _overlayInstance.Closed += (s, e) => _overlayInstance = null;
                    _overlayInstance.Activate();
                }
                else
                {
                    _overlayInstance.Close();
                    _overlayInstance = null;
                }
            }

            // Save the current state for next tick comparison
            _lastButtonState = currentButtons;
        }


        private bool _altPressed = false;

        private void MainWindow_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Handle other keys
            switch (e.Key)
            {
                case VirtualKey.Down:
                    MoveRow(1);
                    break;
                case VirtualKey.Up:
                    MoveRow(-1);
                    break;
                case VirtualKey.Left:
                    MoveCol(-1);
                    break;
                case VirtualKey.Right:
                    MoveCol(1);
                    break;
                case VirtualKey.Enter:
                    ExecuteSelectedAction();
                    break;
            }
        }

        private void MainWindow_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Menu) // ALT released
            {
                _altPressed = false;
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

    //nircmd code
    namespace NirCmdUtil
    {
        public static class NirCmdHelper
        {
            /// <summary>
            /// Executes a command using nircmd.exe.
            /// </summary>
            /// <param name="command">The command to pass to nircmd (e.g., "changesysvolume 5000")</param>
            public static void ExecuteCommand(string command)
            {
                // Determine the path to nircmd.exe in the current directory
                string nircmdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nircmd.exe");

                if (!File.Exists(nircmdPath))
                {
                    throw new FileNotFoundException("nircmd.exe was not found in the current directory.");
                }

                // Configure ProcessStartInfo for nircmd
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = nircmdPath,
                    Arguments = command,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();

                    // Optionally capture output and error streams
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        throw new Exception("Error executing nircmd command: " + error);
                    }
                }
            }
        }
    }
    //Toast code
    public static class NativeToastOverlay
    {
        private static readonly object syncLock = new();
        private static readonly List<Form> activeToasts = new();

        public static void Show(string message, Icon icon = null)
        {
            Thread toastThread = new Thread(() =>
            {
                Form toastForm = new Form
                {
                    Width = 520,
                    Height = 80,
                    FormBorderStyle = FormBorderStyle.None,
                    TopMost = true,
                    ShowInTaskbar = false,
                    BackColor = System.Drawing.Color.Black,
                    Opacity = 0, // Start transparent for fade-in
                    StartPosition = FormStartPosition.Manual
                };

                // Layout
                System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(10),
                    BackColor = System.Drawing.Color.Black
                };

                PictureBox iconBox = new PictureBox
                {
                    Width = 32,
                    Height = 32,
                    Margin = new Padding(0, 0, 10, 0),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Visible = icon != null
                };

                if (icon != null)
                    iconBox.Image = icon.ToBitmap();

                Label label = new Label
                {
                    Text = message,
                    ForeColor = System.Drawing.Color.White,
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 12),
                    MinimumSize = new System.Drawing.Size(0, 32)

                };

                FlowLayoutPanel flow = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight
                };

                flow.Controls.Add(iconBox);
                flow.Controls.Add(label);
                panel.Controls.Add(flow);
                toastForm.Controls.Add(panel);

                // Stack positioning
                lock (syncLock)
                {
                    var screen = Screen.PrimaryScreen.WorkingArea;
                    int offset = (activeToasts.Count * (toastForm.Height + 10)) + 20;
                    toastForm.Location = new System.Drawing.Point(screen.Right - toastForm.Width - 20, screen.Bottom - offset);
                    activeToasts.Add(toastForm);
                }

                toastForm.Load += (s, e) => SetWindowStyles(toastForm);

                toastForm.Shown += (s, e) =>
                {
                    FadeIn(toastForm, () =>
                    {
                        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 3000 };
                        timer.Tick += (s2, e2) =>
                        {
                            timer.Stop();
                            FadeOut(toastForm, () =>
                            {
                                lock (syncLock)
                                {
                                    activeToasts.Remove(toastForm);
                                    RepositionToasts();
                                }
                                toastForm.Close();
                            });
                        };
                        timer.Start();
                    });
                };

                System.Windows.Forms.Application.Run(toastForm);
            });

            toastThread.SetApartmentState(ApartmentState.STA);
            toastThread.IsBackground = true;
            toastThread.Start();
        }

        private static void RepositionToasts()
        {
            var screen = Screen.PrimaryScreen.WorkingArea;
            for (int i = 0; i < activeToasts.Count; i++)
            {
                var f = activeToasts[i];
                f.Invoke(() =>
                {
                    int offset = (i * (f.Height + 10)) + 20;
                    f.Location = new Point(screen.Right - f.Width - 20, screen.Bottom - offset);
                });
            }
        }

        private static void FadeIn(Form form, Action onComplete)
        {
            System.Windows.Forms.Timer fadeIn = new System.Windows.Forms.Timer { Interval = 15 };
            fadeIn.Tick += (s, e) =>
            {
                form.Opacity += 0.05;
                if (form.Opacity >= 0.95)
                {
                    form.Opacity = 0.95;
                    fadeIn.Stop();
                    onComplete?.Invoke();
                }
            };
            fadeIn.Start();
        }

        private static void FadeOut(Form form, Action onComplete)
        {
            System.Windows.Forms.Timer fadeOut = new System.Windows.Forms.Timer { Interval = 15 };
            fadeOut.Tick += (s, e) =>
            {
                form.Opacity -= 0.05;
                if (form.Opacity <= 0)
                {
                    fadeOut.Stop();
                    onComplete?.Invoke();
                }
            };
            fadeOut.Start();
        }

        private static void SetWindowStyles(Form form)
        {
            IntPtr hwnd = form.Handle;
            int exStyle = (int)GetWindowLong(hwnd, GWL_EXSTYLE);
            exStyle |= WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE | WS_EX_TOPMOST;
            SetWindowLong(hwnd, GWL_EXSTYLE, (IntPtr)exStyle);

            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOPMOST = 0x00000008;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);
    }
}
