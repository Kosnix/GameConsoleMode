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
using System.Windows.Threading;
using Image = Microsoft.UI.Xaml.Controls.Image;
using Microsoft.UI.Text;


namespace gcmloader
{

    public sealed partial class MainWindow : Window
    {
        #region needed
        #region taskmanager
        [DllImport("user32.dll")]

        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// If the old selected window still exists in the new slice, restore _selectedTileIndex and SubFocus.
        /// </summary>
        private void RestoreSelection(WindowItem oldItem, int oldSubFocus)
        {
            // If we had no old item, nothing to do
            if (oldItem == null) return;

            // Search the newly-built _tiles for the same window (by Hwnd, for example)
            for (int i = 0; i < _tiles.Count; i++)
            {
                if (_tiles[i].Item != null && _tiles[i].Item.Hwnd == oldItem.Hwnd)
                {
                    _selectedTileIndex = i;
                    _tiles[i].SubFocus = oldSubFocus;
                    UpdateTileSelectionUI();
                    return; // done
                }
            }

            // If we get here, the old item wasn't found in the new slice => 
            // leave the selection as it is (which might default to tile 0).
        }

        // Current offset for which window in _windowList is shown in tile index 0
        private int _offset = 0;

        private class TileControls
        {
            public Border TileBorder { get; set; }      // The outer border
            public Button ShowButton { get; set; }      // The "Show" button
            public Button CloseButton { get; set; }     // The "Close" button

            // We could also store the WindowItem or any data if needed
            public WindowItem Item { get; set; }

            // Current sub-focus (0 = tile, 1 = show btn, 2 = close btn)
            public int SubFocus { get; set; } = 0;
        }

        private void UpdateTileSelectionUI()
        {
            for (int i = 0; i < _tiles.Count; i++)
            {
                var tileC = _tiles[i];

                // normal border color
                var normalBorderBrush = new SolidColorBrush(ConvertColor("#2A475E"));
                var highlightBorderBrush = new SolidColorBrush(ConvertColor("#66C0F4"));

                // normal button opacity
                double normalBtnOpacity = 0.5;
                double highlightBtnOpacity = 1.0;

                if (i == _selectedTileIndex)
                {
                    // The selected tile
                    if (tileC.SubFocus == 0)
                    {
                        // Highlight tile border in #66C0F4, thicker
                        tileC.TileBorder.BorderBrush = highlightBorderBrush;
                        tileC.TileBorder.BorderThickness = new Thickness(4);

                        // Buttons partially dim
                        if (tileC.ShowButton != null) tileC.ShowButton.Opacity = 0.7;
                        if (tileC.CloseButton != null) tileC.CloseButton.Opacity = 0.7;
                    }
                    else if (tileC.SubFocus == 1)
                    {
                        // Normal tile border
                        tileC.TileBorder.BorderBrush = normalBorderBrush;
                        tileC.TileBorder.BorderThickness = new Thickness(2);

                        // Show button fully highlighted
                        if (tileC.ShowButton != null) tileC.ShowButton.Opacity = highlightBtnOpacity;
                        if (tileC.CloseButton != null) tileC.CloseButton.Opacity = normalBtnOpacity;
                    }
                    else if (tileC.SubFocus == 2)
                    {
                        // Normal tile border
                        tileC.TileBorder.BorderBrush = normalBorderBrush;
                        tileC.TileBorder.BorderThickness = new Thickness(2);

                        // Close button fully highlighted
                        if (tileC.ShowButton != null) tileC.ShowButton.Opacity = normalBtnOpacity;
                        if (tileC.CloseButton != null) tileC.CloseButton.Opacity = highlightBtnOpacity;
                    }
                }
                else
                {
                    // Non-selected tile
                    tileC.TileBorder.BorderBrush = normalBorderBrush;
                    tileC.TileBorder.BorderThickness = new Thickness(2);

                    // Dim both buttons
                    if (tileC.ShowButton != null) tileC.ShowButton.Opacity = normalBtnOpacity;
                    if (tileC.CloseButton != null) tileC.CloseButton.Opacity = normalBtnOpacity;
                }
            }
        }

        // Full list of open windows
        private List<WindowItem> _windowList = new List<WindowItem>();
        // We'll keep a list of 5 TileControls
        private List<TileControls> _tiles = new List<TileControls>();

        private void RefreshTiles()
        {
            // 1) Update our window list
            _windowList = GetAllOpenWindows(); // or re-use a previously stored list

            // 2) Get the 5-window slice for display
            var slice = _windowList.Skip(_offset).Take(5).ToList();

            // We'll re-create the _tiles list each time
            _tiles.Clear();

            // Collect the 5 border controls (Tile1..Tile5 already defined in XAML)
            var tileBorders = new List<Border> { Tile1, Tile2, Tile3, Tile4, Tile5 };

            for (int i = 0; i < 5; i++)
            {
                // Create a new TileControls for this slot
                var tileC = new TileControls
                {
                    TileBorder = tileBorders[i],
                    SubFocus = 0 // default to focusing the tile area
                };

                // Clear old child
                tileBorders[i].Child = null;

                if (i < slice.Count)
                {
                    // There's a window for this tile
                    var item = slice[i];
                    tileC.Item = item;

                    // NEW PART: use our modern method to build the tile content
                    // and set tileBorders[i].Child to that UI
                    tileBorders[i].Child = CreateTileContent(item, tileC);
                    tileC.TileBorder.Background = new SolidColorBrush(ConvertColor("#1B2838")); // dunkler Steam-Hintergrund
                    tileC.TileBorder.Opacity = 0.9; // leicht durchsichtig

                }

                _tiles.Add(tileC);
            }

            // Ensure the selectedTileIndex is valid
            if (_selectedTileIndex >= slice.Count)
                _selectedTileIndex = slice.Count - 1;
            if (_selectedTileIndex < 0)
                _selectedTileIndex = 0;

            UpdateTileSelectionUI();
        }
        private UIElement CreateTileContent(WindowItem item, TileControls tileC)
        {
            var stack = new StackPanel
            {
                Orientation = Microsoft.UI.Xaml.Controls.Orientation.Vertical,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 10
            };

            // 1. App Icon (optional fallback)
            var iconImage = new Image
            {
                Width = 64,
                Height = 64,
                Margin = new Thickness(0, 5, 0, 5),
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center
            };

            // Try to load icon from process
            try
            {
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(item.Proc.MainModule.FileName);
                if (icon != null)
                {
                    using (var bmp = icon.ToBitmap())
                    {
                        var stream = new MemoryStream();
                        bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        stream.Position = 0;

                        var bitmap = new BitmapImage();
                        bitmap.SetSource(stream.AsRandomAccessStream());
                        iconImage.Source = bitmap;
                    }
                }
            }
            catch
            {
                // Leave blank or use placeholder
            }

            // 2. App Title
            var titleText = new TextBlock
            {
                Text = item.DisplayName,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(ConvertColor("#C7D5E0")),
                TextWrapping = TextWrapping.Wrap,
                MaxLines = 2,
                TextTrimming = TextTrimming.CharacterEllipsis,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center
            };

            // 3. Close Button (OBEN)
            var closeBtn = new Button
            {
                Content = "Close",
                Width = 200,
                Height = 40,
                FontSize = 16,
                Foreground = new SolidColorBrush(ConvertColor("#C7D5E0")),
                Background = new SolidColorBrush(ConvertColor("#A02A2A")),
                BorderBrush = new SolidColorBrush(ConvertColor("#66C0F4")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                 IsTabStop = false 
            };
            closeBtn.Click += (s, e) =>
            {
                try { item.Proc.CloseMainWindow(); RefreshTiles(); } catch { }
            };

            // 4. Show Button (UNTEN)
            var showBtn = new Button
            {
                Content = "Show",
                Width = 200,
                Height = 40,
                FontSize = 16,
                Foreground = new SolidColorBrush(ConvertColor("#C7D5E0")),
                Background = new SolidColorBrush(ConvertColor("#2A475E")),
                BorderBrush = new SolidColorBrush(ConvertColor("#66C0F4")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                IsTabStop = false 
            };
            showBtn.Click += (s, e) =>
            {
                ShowWindow(item.Hwnd, SW_RESTORE);
                SetForegroundWindow(item.Hwnd);
            };

            tileC.ShowButton = showBtn;
            tileC.CloseButton = closeBtn;

            // Add in correct order
            stack.Children.Add(iconImage);
            stack.Children.Add(titleText);
            stack.Children.Add(closeBtn);
            stack.Children.Add(showBtn);

            return stack;
        }


        private Windows.UI.Color ConvertColor(string hex)
        {
            // Expected format: #RRGGBB or #AARRGGBB
            hex = hex.Replace("#", "");
            byte a = 255; // default alpha
            byte r = 0;
            byte g = 0;
            byte b = 0;

            if (hex.Length == 8)
            {
                // AARRGGBB
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                r = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            else if (hex.Length == 6)
            {
                // RRGGBB
                r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return Windows.UI.Color.FromArgb(a, r, g, b);
        }



        private static string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            if (length == 0)
                return string.Empty;

            var sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }


        private List<WindowItem> GetAllOpenWindows()
        {
            var results = new List<WindowItem>();

            // List of excluded process names (case-insensitive)
            var excludedProcesses = new List<string>
    {
        "explorer",
        "RuntimeBroker"
        // Add more as needed
    };

            // Enumerate all top-level windows
            EnumWindows((hWnd, lParam) =>
            {
                // Skip windows that are not visible
                if (!IsWindowVisible(hWnd))
                    return true;

                // Get window title
                string title = GetWindowTitle(hWnd);
                if (string.IsNullOrWhiteSpace(title))
                    return true;

                // Get the process ID for this window
                GetWindowThreadProcessId(hWnd, out uint pid);
                if (pid == 0)
                    return true;

                // Skip our own process
                if (pid == (uint)Process.GetCurrentProcess().Id)
                    return true;

                // Try to get the process from the PID
                Process proc;
                try
                {
                    proc = Process.GetProcessById((int)pid);
                }
                catch
                {
                    return true;
                }

                // Skip processes without a main window handle
                if (proc.MainWindowHandle == IntPtr.Zero)
                    return true;

                // Skip if the process name is in the exclusion list
                if (excludedProcesses.Contains(proc.ProcessName, StringComparer.OrdinalIgnoreCase))
                    return true;

                // Try to get a friendly display name from the product info
                string displayName = null;
                try
                {
                    displayName = proc.MainModule?.FileVersionInfo?.ProductName;
                }
                catch
                {
                    // Fallback to process name
                }

                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = proc.ProcessName;

                // Add the window to the result list
                results.Add(new WindowItem
                {
                    Hwnd = hWnd,
                    Proc = proc,
                    DisplayName = displayName,
                    WindowTitle = title
                });

                return true;
            }, IntPtr.Zero);

            return results;
        }




        private class WindowItem
        {
            public IntPtr Hwnd { get; set; }
            public Process Proc { get; set; }
            public string DisplayName { get; set; }
            public string WindowTitle { get; set; } // optional
        }


        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);


        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);



        #endregion taskmanager
        #region taskmanager controll
        // Index of which tile is currently selected (0..4)
        private int _selectedTileIndex = 0;

        // We'll populate this after InitializeComponent
        private List<Border> _tileList;

        private void MainWindow_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Left:
                    HandleLeftArrow();
                    break;

                case VirtualKey.Right:
                    HandleRightArrow();
                    break;

                case VirtualKey.Up:
                    {
                        var tile = _tiles[_selectedTileIndex];
                        tile.SubFocus++;
                        if (tile.SubFocus > 2) tile.SubFocus = 2;
                        UpdateTileSelectionUI();
                    }
                    break;

                case VirtualKey.Down:
                    {
                        var tile = _tiles[_selectedTileIndex];
                        tile.SubFocus--;
                        if (tile.SubFocus < 0) tile.SubFocus = 0;
                        UpdateTileSelectionUI();
                    }
                    break;

                case VirtualKey.Enter:
                    OnEnterPressed();
                    break;
            }
        }

        private void HandleLeftArrow()
        {
            // If we're not at tile 0, just move left
            if (_selectedTileIndex > 0)
            {
                _selectedTileIndex--;
                _tiles[_selectedTileIndex].SubFocus = 0;
                UpdateTileSelectionUI();
            }
            else
            {
                // We ARE at tile 0. Let's see if we can scroll the offset left:
                if (_offset > 0)
                {
                    // Move offset one left
                    _offset--;
                    // Keep selectedTileIndex at 0 so we remain on the left tile
                    RefreshTiles();
                    // SubFocus resets to 0 or keep it? Let's keep it 0 for now:
                    _tiles[_selectedTileIndex].SubFocus = 0;
                    UpdateTileSelectionUI();
                }
                else
                {
                    // offset=0 => there's no more to the left
                    // Possibly wrap or do nothing
                }
            }
        }

        private void HandleRightArrow()
        {
            // How many windows are in the big list?
            int totalWindows = _windowList.Count;
            // The last index in that big list is totalWindows-1

            // First see how many are in the current slice
            int sliceCount = _windowList.Skip(_offset).Take(5).Count();

            if (_selectedTileIndex < sliceCount - 1 && _selectedTileIndex < 4)
            {
                // Move within the 5 displayed tiles if there's room to the right
                _selectedTileIndex++;
                _tiles[_selectedTileIndex].SubFocus = 0;
                UpdateTileSelectionUI();
            }
            else
            {
                // We are at the far-right displayed tile
                // Check if there's more windows beyond offset+4
                if ((_offset + 5) < totalWindows)
                {
                    // Shift offset one to the right
                    _offset++;
                    // We'll remain on tile 4 (the rightmost tile),
                    // but it will now represent the *next* window
                    RefreshTiles();
                    _tiles[_selectedTileIndex].SubFocus = 0;
                    UpdateTileSelectionUI();
                }
                else
                {
                    // There's no more windows to the right
                    // Possibly wrap or do nothing
                }
            }
        }


        private void OnEnterPressed()
        {
            if (_selectedTileIndex < 0 || _selectedTileIndex >= _tiles.Count) return;

            var tileC = _tiles[_selectedTileIndex];
            if (tileC.Item == null) return;

            switch (tileC.SubFocus)
            {
                case 0:
                case 1:
                    // Always bring the window to front
                    ShowWindow(tileC.Item.Hwnd, SW_RESTORE);
                    SetForegroundWindow(tileC.Item.Hwnd);
                    break;
                case 2:
                    try
                    {
                        tileC.Item.Proc.CloseMainWindow();
                        RefreshTiles();
                    }
                    catch { }
                    break;
            }
        }





        private List<WindowItem> _currentWindowItems = new List<WindowItem>();

        private void OnTileEnterPressed(int index)
        {
            // Check if there's even a WindowItem for that tile
            if (index >= 0 && index < _currentWindowItems.Count)
            {
                var item = _currentWindowItems[index];
                if (item != null)
                {
                    // For instance, bring the window to foreground:
                    ShowWindow(item.Hwnd, SW_RESTORE);
                    SetForegroundWindow(item.Hwnd);
                }
            }
            else
            {
                Console.WriteLine("No window data for this tile index.");
            }
        }

        #endregion taskmanager controll
        #region taskmanager xbox controll
        private Controller _controller;
        private bool _controllerConnected = false;
        private GamepadButtonFlags _lastButtons = GamepadButtonFlags.None;
        private System.Threading.Timer _controllerPollTimer;
        private bool _broughtToFront = false;


        private void StartControllerPolling()
        {
            // We create an empty controller to initialize the polling loop
            _controller = new Controller(UserIndex.One);

            _controllerPollTimer = new System.Threading.Timer(state =>
            {
                // Always create a new controller instance to check connection state
                var newController = new Controller(UserIndex.One);

                if (newController.IsConnected)
                {
                    // Update the main controller reference
                    _controller = newController;

                    // Handle first-time connection
                    if (!_controllerConnected)
                    {
                        _controllerConnected = true;
                        Console.WriteLine("Controller connected.");
                    }

                    // Read the current state of the controller
                    var controllerstate = _controller.GetState();
                    var buttons = controllerstate.Gamepad.Buttons;

                    // Detect newly pressed buttons (only edge trigger)
                    var newlyPressed = buttons & ~_lastButtons;

                    // 👉 1. GLOBAL: Select + Start → bring GCM window to front
                    bool comboStart = (buttons & GamepadButtonFlags.Start) != 0 &&
                                      (buttons & GamepadButtonFlags.Back) != 0;

                    if (comboStart)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                            ShowWindow(hwnd, SW_RESTORE);
                            SetForegroundWindow(hwnd);
                        });
                    }

                    // 👉 2. GLOBAL: Select + DPadRight → simulate Alt+Tab
                    bool comboAltTab = (buttons & GamepadButtonFlags.Back) != 0 &&
                                       (buttons & GamepadButtonFlags.DPadRight) != 0;

                    if (comboAltTab)
                    {
                        _controllerHotkeys.SendAltTab(); // Call your existing Alt+Tab method
                    }

                    // 👉 3. UI Navigation: only if window is focused
                    IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
                    bool isFocused = GetForegroundWindow() == windowHandle;

                    if (isFocused)
                    {
                        if ((newlyPressed & GamepadButtonFlags.DPadLeft) != 0)
                            DispatcherQueue.TryEnqueue(() => HandleLeftArrow());

                        if ((newlyPressed & GamepadButtonFlags.DPadRight) != 0)
                            DispatcherQueue.TryEnqueue(() => HandleRightArrow());

                        if ((newlyPressed & GamepadButtonFlags.DPadUp) != 0)
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                var tile = _tiles[_selectedTileIndex];
                                tile.SubFocus++;
                                if (tile.SubFocus > 2) tile.SubFocus = 2;
                                UpdateTileSelectionUI();
                            });

                        if ((newlyPressed & GamepadButtonFlags.DPadDown) != 0)
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                var tile = _tiles[_selectedTileIndex];
                                tile.SubFocus--;
                                if (tile.SubFocus < 0) tile.SubFocus = 0;
                                UpdateTileSelectionUI();
                            });

                        if ((newlyPressed & GamepadButtonFlags.A) != 0)
                            DispatcherQueue.TryEnqueue(() => OnEnterPressed());
                    }

                    // Store current button state for next poll comparison
                    _lastButtons = buttons;
                }
                else
                {
                    // Controller is disconnected
                    if (_controllerConnected)
                    {
                        _controllerConnected = false;
                        Console.WriteLine("Controller disconnected.");
                    }

                    // Reset button tracking
                    _lastButtons = GamepadButtonFlags.None;
                }
            }, null, 0, 100); // Poll every 100ms
        }



        #endregion taskmanager xbox controll
        #region shortcuts
        private ControllerHotkeyListener _controllerHotkey;

        private ControllerHotkeyListener _controllerHotkeys;
        #endregion shortcuts

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
            #region taskmanager

           


            // Make sure we can receive key input
            this.Activated += (s, e) => this.Content.Focus(FocusState.Programmatic);

            // Suppose you already define the 5 tile borders: _tileList
            _tileList = new List<Border> { Tile1, Tile2, Tile3, Tile4, Tile5 };

            // Attach KeyDown
            this.Content.KeyDown += MainWindow_KeyDown;

            // Set up a timer that runs every 5 seconds
            var dispatcherTimer = new Microsoft.UI.Xaml.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            dispatcherTimer.Tick += (s, e) =>
            {
                // 1) Capture current tile selection (which item & subFocus is selected)
                TileControls oldTile = null;
                if (_selectedTileIndex >= 0 && _selectedTileIndex < _tiles.Count)
                {
                    oldTile = _tiles[_selectedTileIndex];
                }
                WindowItem oldItem = oldTile?.Item;
                int oldSubFocus = oldTile?.SubFocus ?? 0;

                // 2) Refresh the tiles
                RefreshTiles();

                // 3) Attempt to restore the same selection if that window still exists
                RestoreSelection(oldItem, oldSubFocus);
            };

            // Start the timer
            dispatcherTimer.Start();

            // Initial population of the tiles
            RefreshTiles();

            #endregion taskmanager

            Start();
            //ASYNC PROZES
            StartAsynctasks();
            StartControllerPolling();
            _controllerHotkeys = new ControllerHotkeyListener(this);
            _controllerHotkeys.Start();
        }


        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            try
            {
                // Get the current window handle and AppWindow
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = AppWindow.GetFromWindowId(windowId);

                if (appWindow != null)
                {
                    // Set fullscreen via WinUI 3 API
                    appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                }

                // Optional: set wallpaper as background
                // This uses window bounds, no need to get screen resolution manually
                var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
                int width = displayArea.WorkArea.Width;
                int height = displayArea.WorkArea.Height;

                SetBackgroundImage(width, height); // your existing method for wallpaper

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting fullscreen: {ex.Message}");
            }
        }

        #endregion needed
        #region methodes
        #region methodes for code

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
                if (!gcmwallpaper) return;

                string imagePath = Settwallpaper();
                if (!File.Exists(imagePath)) return;

                var backgroundImage = new Microsoft.UI.Xaml.Controls.Image
                {
                    Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
                    Stretch = Stretch.UniformToFill, // DPI-aware scaling
                    HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                if (this.Content is Grid mainGrid)
                {
                    mainGrid.Children.Insert(0, backgroundImage);
                }
                else
                {
                    Grid grid = new Grid();
                    grid.Children.Add(backgroundImage);
                    if (this.Content != null)
                        grid.Children.Add((UIElement)this.Content);

                    this.Content = grid;
                }
            }
            catch
            {
                Console.WriteLine("Wallpaper GUI error");
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
                    
                    IsJoyxoffInstalledAndStart(); //only check if is installed, than start
                    cssloader(); //only check if is installed, than start
                    StartLauncher();
                    ConsoleModeToShell();
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
}
