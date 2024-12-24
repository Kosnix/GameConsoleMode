using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Security.Principal;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using SharpDX.XInput;

namespace ButtonListener
{
    static class Program
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, UIntPtr dwExtraInfo);

        private const byte VK_MENU = 0x12; // Alt key
        private const byte VK_TAB = 0x09;  // Tab key
        private const uint KEYEVENTF_KEYDOWN = 0x0000; // Key down flag
        private const uint KEYEVENTF_KEYUP = 0x0002;   // Key up flag
        #region Mouse click variable
        // Mouse event constants
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        // Mouse event constants
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        // Variable to track the state of mouse buttons
        private static bool isLeftMouseDown = false;
        private static bool isRightMouseDown = false;
        //Move Mouse
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        #endregion Mouse click variable
        static bool AltTab = false;

        static NotifyIcon notifyIcon;
        static bool isRunning = true;

        private static bool StartGCM = false;
        private static bool SwitchWindow = false;
        private static bool Mouse = false;

        [STAThread]
        static void Main()
        {   
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Configure the icon in the notification area
            notifyIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location),
                Text = "GameConsoleMode",
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => Quit());

            // Start monitoring in a separate thread
            Task.Run(() => MonitorXboxController());

            // Garder l'application vivante
            Application.Run();
        }


        static void MonitorXboxController()
        {
            var controller = new Controller(UserIndex.One);
            bool ControllerOn = false;

            // Load Settings
            if (Readconfig("Shortcut0") == "1") { StartGCM = true; }
            if (Readconfig("Shortcut1") == "1") { SwitchWindow = true; }
            if (Readconfig("Shortcut2") == "1") { Mouse = true; }


            while (isRunning)
            {
                
                if (!controller.IsConnected)
                {                    
                    if(ControllerOn == true)
                    {
                        ControllerOn = false;
                        notifyIcon.ShowBalloonTip(3000, "Erreur", "Controller disconnected", ToolTipIcon.Error);
                        Thread.Sleep(200);
                    }
                }
                else
                {
                    if (ControllerOn == false)
                    {
                        ControllerOn = true;
                        notifyIcon.ShowBalloonTip(3000, "", "Controller connected", ToolTipIcon.Info);
                        Thread.Sleep(200);
                    }
                }


                // Start GCM //
                if (controller.IsConnected && !GCMLaunched() && StartGCM)
                {
                    try { 
                        var state = controller.GetState();
                        if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Start)&&state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back)){
                            StartThisFolderExe("GameConsoleMode.exe","");
                        }
                        }
                    catch { }
                    }

                // ALT-TAB

                if(SwitchWindow)//controller.IsConnected && GCMLaunched())
                {
                    try
                    {
                        // Get the state of the controller
                        var state = controller.GetState();

                        // If AltTab is not active
                        if (!AltTab)
                        {
                            // Check if both RightThumb and Back buttons are pressed
                            if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb) &&
                                state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back))
                            {
                                HoldAlt(); // Hold the Alt key
                                PressAndReleaseTab(); // Simulate pressing and releasing Tab
                                AltTab = true; // Set AltTab to active
                            }
                        }
                        else
                        {
                            // If AltTab is active, check if Back is pressed
                            if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back))
                            {
                                // If RightThumb is also pressed, simulate another Tab press
                                if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb))
                                {
                                    PressAndReleaseTab();
                                    
                                }
                            }
                            else
                            {
                                // If Back is not pressed, release Alt and reset AltTab
                                ReleaseAlt();
                                AltTab = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error for debugging purposes (optional)
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        AltTab = false;
                    }


                }


                // MOUSE
                if (Mouse) {
                //Mouse movement  
                const float SENSITIVITY = 15;
                float[] ApplyDeadzoneAndNormalize(float x, float y)
                {
                    const short DEADZONE = 10000;
                    float magnitude = (float)Math.Sqrt(x * x + y * y); // Magnitude of the joystick vector

                    // Ignore small movements within the dead zone
                    if (magnitude < DEADZONE)
                        return new float[] { 0, 0 };

                    // Scale magnitude to range [0, 1] and preserve direction
                    float scaledMagnitude = (magnitude - DEADZONE) / (32767.0f - DEADZONE);
                    scaledMagnitude = scaledMagnitude * scaledMagnitude; // Nonlinear scaling for smoother control

                    // Normalize X and Y components
                    return new float[]
                    {
                        (x / magnitude) * scaledMagnitude,
                        (y / magnitude) * scaledMagnitude
                    };
                }
                // if (controller.IsConnected && !GCMLaunched()) // controller.IsConnected && GCMLaunched()
                if (true)
                {
                    var state = controller.GetState();

                    if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back))
                    {
                        var gamepad = state.Gamepad;

                        // Joystick axes
                        float leftThumbX = gamepad.RightThumbX;
                        float leftThumbY = gamepad.RightThumbY;

                        // Apply dead zone and normalize input
                        float[] normalizedInput = ApplyDeadzoneAndNormalize(leftThumbX, leftThumbY);
                        float deltaX = normalizedInput[0] * SENSITIVITY;
                        float deltaY = -normalizedInput[1] * SENSITIVITY; // Invert Y-axis for screen coordinates

                        // Move the mouse if there is significant movement
                        if (Math.Abs(deltaX) > 0.1f || Math.Abs(deltaY) > 0.1f)
                        {
                            MoveMouse(deltaX, deltaY);
                        }
                    }
                }

                //Right click // Left Click
                // if (controller.IsConnected && !GCMLaunched()) // controller.IsConnected && GCMLaunched()
                if (true)
                {
                    try
                    {
                        var state = controller.GetState();

                        // Check for Right Shoulder (simulate right mouse button hold)
                        if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder))
                        {
                            if (!isRightMouseDown)
                            {
                                // Press right mouse button
                                mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
                                isRightMouseDown = true;
                            }
                        }
                        else if (isRightMouseDown)
                        {
                            // Release right mouse button
                            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
                            isRightMouseDown = false;
                        }

                        // Check for Left Shoulder (simulate left mouse button hold)
                        if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder))
                        {
                            if (!isLeftMouseDown)
                            {
                                // Press left mouse button
                                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                                isLeftMouseDown = true;
                            }
                        }
                        else if (isLeftMouseDown)
                        {
                            // Release left mouse button
                            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                            isLeftMouseDown = false;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Error reading controller.");
                    }
                }

            }}
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

        static void MoveMouse(float deltaX, float deltaY)
        {
            // Number of steps for smooth movement (increase for smoother, decrease for faster)
            const int STEPS = 3000; // Adjust this to control smoothness (e.g., 1 for instant movement)

            // Current mouse position
            Point currentPosition = Cursor.Position;

            // Calculate movement per step
            float stepX = deltaX / STEPS;
            float stepY = deltaY / STEPS;

            for (int i = 0; i < STEPS; i++)
            {
                // Calculate new position for this step
                int newX = (int)(currentPosition.X + stepX * (i + 1));
                int newY = (int)(currentPosition.Y + stepY * (i + 1));

                // Ensure the position stays within the screen bounds
                newX = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Width - 1, newX));
                newY = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Height - 1, newY));

                // Set the mouse position
                SetCursorPos(newX, newY);

                // Short pause for visible movement (adjust for speed)
                Thread.Sleep(0); // Decrease or remove this to make the movement faster
            }
        }



        public static void HoldAlt()
        {
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        }

        public static void ReleaseAlt()
        {
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        public static void PressAndReleaseTab()
        {
            keybd_event(VK_TAB, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            System.Threading.Thread.Sleep(50); // Ajoutez un délai de 50 ms
            keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(100);
        }

        public static bool GCMLaunched()
        {
            Process[] processes;
            string ProcessName = "GameConsoleMode";
            processes = Process.GetProcessesByName(ProcessName);
            if (processes.Length > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        static void KillAllWindowsExcept(string executableNameToKeep)
        {
            // Get all running processes
            Process[] allProcesses = Process.GetProcesses();

            foreach (Process process in allProcesses)
            {
                try
                {
                    // Ensure the process has a main window
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        // Get the executable path
                        string processPath = process.MainModule?.FileName ?? string.Empty;
                        string processFileName = System.IO.Path.GetFileName(processPath);

                        // Compare with the specified executable name (case-insensitive)
                        if (!string.Equals(processFileName, executableNameToKeep, StringComparison.OrdinalIgnoreCase))
                        {
                            // Kill the process
                            process.Kill();
                            Console.WriteLine($"Window process {processFileName} (ID: {process.Id}) killed.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., access denied, process already exited, etc.)
                    Console.WriteLine($"Failed to kill window process (ID: {process.Id}): {ex.Message}");
                }
            }
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

        static string GetLauncherName()
        {
            string Launcher = ReadConfig("Launcher");
            if (Launcher == "Other")
            {
                Launcher = Path.GetFileName(ReadConfig("OtherLauncherPath"));
                return Launcher;
            }
            return string.Concat(Launcher,".exe");
        }

        static void Quit()
        {
            isRunning = false;
            notifyIcon.Visible = false;
            Application.Exit();
        }

        static string exeFolder()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string folderPath = Path.GetDirectoryName(exePath);
            return folderPath;
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


    }
}