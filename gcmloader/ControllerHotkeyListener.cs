using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using SharpDX.XInput;

namespace gcmloader
{
    internal class ControllerHotkeyListener
    {
        private Controller _controller;
        private Thread _listenerThread;
        private bool _running = false;

        private bool _comboSelectStartPressed = false;
        private bool _comboSelectDpadRightPressed = false;

        private Window _targetWindow;

        public ControllerHotkeyListener(Window targetWindow)
        {
            _controller = new Controller(UserIndex.One);
            _targetWindow = targetWindow;
        }

        public void Start()
        {
            if (_running) return;
            _running = true;

            _listenerThread = new Thread(() =>
            {
                while (_running)
                {
                    if (!_controller.IsConnected)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    // Check if the current window is in foreground
                    IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(_targetWindow);
                    if (GetForegroundWindow() != hwnd)
                    {
                        Thread.Sleep(100); // Skip if not focused
                        continue;
                    }

                    var state = _controller.GetState();
                    var buttons = state.Gamepad.Buttons;

                    // Combo: Back + Start → bring GCM window to front
                    bool comboStart = (buttons & GamepadButtonFlags.Start) != 0 &&
                                      (buttons & GamepadButtonFlags.Back) != 0;

                    if (comboStart && !_comboSelectStartPressed)
                    {
                        _comboSelectStartPressed = true;

                        _targetWindow.DispatcherQueue.TryEnqueue(() =>
                        {
                            ShowWindow(hwnd, SW_RESTORE);
                            SetForegroundWindow(hwnd);
                        });
                    }
                    else if (!comboStart)
                    {
                        _comboSelectStartPressed = false;
                    }

                    // Combo: Back + DPadRight → simulate Alt+Tab
                    bool comboAltTab = (buttons & GamepadButtonFlags.Back) != 0 &&
                                       (buttons & GamepadButtonFlags.DPadRight) != 0;

                    if (comboAltTab && !_comboSelectDpadRightPressed)
                    {
                        _comboSelectDpadRightPressed = true;
                        SendAltTab();
                    }
                    else if (!comboAltTab)
                    {
                        _comboSelectDpadRightPressed = false;
                    }

                    Thread.Sleep(100); // Polling delay
                }
            });

            _listenerThread.IsBackground = true;
            _listenerThread.Start();
        }

        public void Stop()
        {
            _running = false;
            _listenerThread?.Join();
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private const int KEYEVENTF_KEYDOWN = 0x0000;
        private const int KEYEVENTF_KEYUP = 0x0002;

        private const byte VK_MENU = 0x12;  // Alt
        private const byte VK_TAB = 0x09;   // Tab

        public void SendAltTab()
        {
            // Simulate Alt + Tab key press
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYDOWN, 0);
            Thread.Sleep(50);

            keybd_event(VK_TAB, 0, KEYEVENTF_KEYDOWN, 0);
            Thread.Sleep(50);
            keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);

            Thread.Sleep(50);
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}

