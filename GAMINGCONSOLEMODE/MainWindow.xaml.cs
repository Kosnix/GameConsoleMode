using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.IO;
using System.Linq;
using Windows.Graphics;
using WinRT.Interop;
using GAMINGCONSOLEMODE;
using Windows.UI.ApplicationSettings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GAMINGCONSOLEMODE
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            // Navigate to the 'startup' page on app launch
            contentFrame.Navigate(typeof(Home), null, new SlideNavigationTransitionInfo()
            {
                Effect = SlideNavigationTransitionEffect.FromRight
            });

            // Set the window size
            SetWindowSize(1500, 1100);
            // 1. First Start: Create folder and default config file if needed
            AppSettings.FirstStart();
        }
        


        #region programm start
        


        #endregion programm start
        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {

                if (args.IsSettingsSelected)
                {
                    // Logik für das Settings-Element
                    contentFrame.Navigate(typeof(settings));

                }
                else {

                    string selectedTag = args.SelectedItemContainer.Tag.ToString();

                    // Determine the target page based on the tag
                    Type pageType = selectedTag switch
                    {
                        "HomePage" => typeof(Home),
                        "LauncherPage" => typeof(launcher),
                        "StartupPage" => typeof(startup),
                        _ => null
                    };

                    if (pageType != null && contentFrame.CurrentSourcePageType != pageType)
                    {
                        // Use SlideNavigationTransitionInfo to specify the transition direction
                        var transitionInfo = new SlideNavigationTransitionInfo
                        {
                            Effect = SlideNavigationTransitionEffect.FromRight
                        };

                        // Navigate with transition info
                        contentFrame.Navigate(pageType, null, transitionInfo);
                    }
                }
                }
        }
       
        private void SetWindowSize(int width, int height)
        {
            // Get the native HWND for the current window
            var hWnd = WindowNative.GetWindowHandle(this);

            // Get the WindowId for the HWND
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

            // Get the AppWindow using the WindowId
            var appWindow = AppWindow.GetFromWindowId(windowId); // Korrekt aufgerufen mit dem Typnamen AppWindow

            if (appWindow != null)
            {
                // Set the window size
                appWindow.Resize(new SizeInt32(width, height));
            }
        }

        public static implicit operator string(MainWindow v)
        {
            throw new NotImplementedException();
        }
    }
}