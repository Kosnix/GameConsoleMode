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

        #region Mouse click variables
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;

        private static bool isLeftMouseDown = false;
        private static bool isRightMouseDown = false;

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        #endregion

        static bool AltTab = false;

        static NotifyIcon notifyIcon;
        static bool isRunning = true;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            notifyIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location),
                Text = "GameConsoleMode",
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => Quit());

            Task.Run(() => MonitorXboxController());
            Application.Run();
        }

        static void MonitorXboxController()
        {
            var controller = new Controller(UserIndex.One);
            bool ControllerOn = false;

            while (isRunning)
            {
                try
                {
                    // Controller connection status
                    if (!controller.IsConnected)
                    {
                        if (ControllerOn)
                        {
                            ControllerOn = false;
                            notifyIcon.ShowBalloonTip(3000, "Error", "Controller disconnected", ToolTipIcon.Error);
                        }
                    }
                    else if (!ControllerOn)
                    {
                        ControllerOn = true;
                        notifyIcon.ShowBalloonTip(3000, "", "Controller connected", ToolTipIcon.Info);
                    }

                    // ALT-TAB Logic
                    HandleAltTabLogic(controller);

                    // Mouse movement
                    HandleMouseMovement(controller);

                    // Mouse clicks
                    HandleMouseClicks(controller);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in MonitorXboxController loop: {ex.Message}");
                }

                Thread.Sleep(10); // Avoid excessive CPU usage
            }
        }

        static void HandleAltTabLogic(Controller controller)
        {
            try
            {
                if (!controller.IsConnected) return;

                var state = controller.GetState();
                if (!AltTab)
                {
                    if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb) &&
                        state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back))
                    {
                        HoldAlt();
                        PressAndReleaseTab();
                        AltTab = true;
                    }
                }
                else if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back))
                {
                    if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb))
                    {
                        PressAndReleaseTab();
                    }
                }
                else
                {
                    ReleaseAlt();
                    AltTab = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Alt-Tab error: {ex.Message}");
                AltTab = false;
            }
        }

        static void HandleMouseMovement(Controller controller)
        {
            if (!controller.IsConnected) return;

            const float SENSITIVITY = 15;
            const short DEADZONE = 10000;

            float[] ApplyDeadzoneAndNormalize(float x, float y)
            {
                float magnitude = (float)Math.Sqrt(x * x + y * y);

                if (magnitude < DEADZONE) return new float[] { 0, 0 };

                float scaledMagnitude = (magnitude - DEADZONE) / (32767.0f - DEADZONE);
                scaledMagnitude = scaledMagnitude * scaledMagnitude;

                return new float[]
                {
                    (x / magnitude) * scaledMagnitude,
                    (y / magnitude) * scaledMagnitude
                };
            }

            var state = controller.GetState();
            if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back))
            {
                var gamepad = state.Gamepad;
                float leftThumbX = gamepad.RightThumbX;
                float leftThumbY = gamepad.RightThumbY;

                float[] normalizedInput = ApplyDeadzoneAndNormalize(leftThumbX, leftThumbY);
                float deltaX = normalizedInput[0] * SENSITIVITY;
                float deltaY = -normalizedInput[1] * SENSITIVITY;

                if (Math.Abs(deltaX) > 0.1f || Math.Abs(deltaY) > 0.1f)
                {
                    MoveMouse(deltaX, deltaY);
                }
            }
        }

        static void HandleMouseClicks(Controller controller)
        {
            if (!controller.IsConnected) return;

            try
            {
                var state = controller.GetState();

                if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder))
                {
                    if (!isRightMouseDown)
                    {
                        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
                        isRightMouseDown = true;
                    }
                }
                else if (isRightMouseDown)
                {
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
                    isRightMouseDown = false;
                }

                if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder))
                {
                    if (!isLeftMouseDown)
                    {
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                        isLeftMouseDown = true;
                    }
                }
                else if (isLeftMouseDown)
                {
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                    isLeftMouseDown = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mouse click error: {ex.Message}");
            }
        }

        static void MoveMouse(float deltaX, float deltaY)
        {
            Point currentPosition = Cursor.Position;
            int newX = (int)(currentPosition.X + deltaX);
            int newY = (int)(currentPosition.Y + deltaY);

            newX = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Width - 1, newX));
            newY = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Height - 1, newY));

            SetCursorPos(newX, newY);
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
            Thread.Sleep(50);
            keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        static void Quit()
        {
            isRunning = false;
            notifyIcon.Visible = false;
            Application.Exit();
        }
    }
}