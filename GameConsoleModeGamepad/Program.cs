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

    private const byte VK_MENU = 0x12; // Alt key
    private const byte VK_TAB = 0x09;  // Tab key
    private const uint KEYEVENTF_KEYDOWN = 0x0000; // Key down flag
    private const uint KEYEVENTF_KEYUP = 0x0002;   // Key up flag
        static bool AltTab = false;

        static NotifyIcon notifyIcon;
        static bool isRunning = true;

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
            while (isRunning)
            {
               // Thread.Sleep(50);
                if (!controller.IsConnected)
                {                    
                    if(ControllerOn == true)
                    {
                        ControllerOn = false;
                        notifyIcon.ShowBalloonTip(3000, "Erreur", "Controller disconnected", ToolTipIcon.Error);
                    }
                }
                else
                {
                    if (ControllerOn == false)
                    {
                        ControllerOn = true;
                        notifyIcon.ShowBalloonTip(3000, "", "Controller connected", ToolTipIcon.Info);
                    }
                }


                // Start GCM //
                if (controller.IsConnected && !GCMLaunched())
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

                if(true)//controller.IsConnected && GCMLaunched())
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

                //Mouse movement 
                // if (controller.IsConnected && !GCMLaunched()) // controller.IsConnected && GCMLaunched()
                if (true)
                {
                    try
                    {
                        var state = controller.GetState();
                        if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back))
                        {
                            var gamepad = state.Gamepad;

                            // Values ​​of the X and Y axes of the left joystick
                            float leftThumbX = gamepad.LeftThumbX;
                            float leftThumbY = gamepad.LeftThumbY;

                            // Apply a dead zone to avoid small movements
                            const short DEADZONE = 8000; // Zone morte
                            if (Math.Abs(leftThumbX) < DEADZONE) leftThumbX = 0;
                            if (Math.Abs(leftThumbY) < DEADZONE) leftThumbY = 0;
                            if (leftThumbY != 0 || leftThumbX != 0)
                            {
                                // Calculate movements for the mouse
                                const float SENSITIVITY = 20; // Mouse movement sensitivity
                                float deltaX = leftThumbX * SENSITIVITY / 32767; // Normalization
                                float deltaY = -leftThumbY * SENSITIVITY / 32767; // Invert Y to match screen

                                // Move the mouse
                                MoveMouse(deltaX, deltaY);
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Error reading controller.");
                    }
                }

            }
        }

        static void MoveMouse(float deltaX, float deltaY)
        {
            // Number of steps for progressive movement
            const int STEPS = 1;

            // Current mouse position
            Point currentPosition = Cursor.Position;

            // Calculation of displacement per step
            float stepX = deltaX / STEPS;
            float stepY = deltaY / STEPS;

            for (int i = 0; i < STEPS; i++)
            {
                // New partial mouse position
                int newX = (int)(currentPosition.X + stepX * (i + 1));
                int newY = (int)(currentPosition.Y + stepY * (i + 1));

                // Prevent the mouse from leaving the screen
                newX = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Width - 1, newX));
                newY = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Height - 1, newY));

                // Move the mouse
                Cursor.Position = new Point(newX, newY);

                // Short pause to make the move visible
                Thread.Sleep(1);
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