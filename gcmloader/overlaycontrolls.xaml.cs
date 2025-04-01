using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace gcmloader
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class overlaycontrolls : Window
    {
        // Interop: Fensterstile, Transparenz, Position
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwLong);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int LWA_ALPHA = 0x2;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_SHOWWINDOW = 0x0040;

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }


        private const uint SWP_NOACTIVATE = 0x0010;


        // Manuelles Centering (da CenterWindow nicht existiert in WinUI 3)
        private void CenterWindow()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var screen = System.Windows.Forms.Screen.FromHandle(hwnd).WorkingArea;

            int width = 600;  // Breite deines Fensters (anpassen!)
            int height = 400; // Höhe deines Fensters (anpassen!)

            int x = (screen.Width - width) / 2;
            int y = (screen.Height - height) / 2;

            SetWindowPos(hwnd, HWND_TOPMOST, x, y, width, height, SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }

        public overlaycontrolls()
        {
            this.InitializeComponent();

            // Hole das native HWND
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            // Borderless: Kein Rahmen, kein Titel
            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.SetBorderAndTitleBar(false, false);
                presenter.IsAlwaysOnTop = true;
            }

            // Transparenz + Klickdurchlässig
            SetWindowLong(hwnd, GWL_EXSTYLE,
                (IntPtr)((int)GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_LAYERED | WS_EX_TRANSPARENT));
            SetLayeredWindowAttributes(hwnd, 0, 250, LWA_ALPHA); // 220 = ~85% sichtbar

            // Fullscreen erzwingen
            var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            MoveWindow(hwnd, 0, 0, screen.Width, screen.Height, true);

            // Immer im Vordergrund
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

            // Shortcut-Liste befüllen
            ShortcutList.ItemsSource = new List<ShortcutModel>
            {
                new ShortcutModel { Combo = "Back + Start", Action = "Bring to Foreground" },
                new ShortcutModel { Combo = "Back + Y", Action = "ALT+TAB" },
                new ShortcutModel { Combo = "Back + X", Action = "Toggle Overlay" },
                new ShortcutModel { Combo = "Back + RThumb", Action = "Switch Audio Device" },
                new ShortcutModel { Combo = "DPad ↑ / ↓", Action = "Move Menu Row" },
                new ShortcutModel { Combo = "DPad ← / →", Action = "Move Menu Column" },
                new ShortcutModel { Combo = "A", Action = "Execute Action" }
            };


        }

        // Transparente Hintergrund-Unterstützung aktivieren (Win32)
        private void EnableWindowTransparency(IntPtr hwnd)
        {
            var margins = new MARGINS()
            {
                cxLeftWidth = -1,
                cxRightWidth = -1,
                cyTopHeight = -1,
                cyBottomHeight = -1
            };
            DwmExtendFrameIntoClientArea(hwnd, ref margins);
        }
    }

    public class ShortcutModel
    {
        public string Combo { get; set; }
        public string Action { get; set; }
    }

}
