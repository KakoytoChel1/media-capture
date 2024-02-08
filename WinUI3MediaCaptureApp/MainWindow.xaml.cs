using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI;
using WinRT.Interop;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using AppWindowTitleBar = Microsoft.UI.Windowing.AppWindowTitleBar;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3MediaCaptureApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        private AppWindow m_AppWindow;

        private bool _isOpen = false;

        public MainWindow()
        {
            this.InitializeComponent();

            SetTitleBarColors();

            PasswordBox pb = new PasswordBox();
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        private bool SetTitleBarColors()
        {
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                if (m_AppWindow is null)
                {
                    m_AppWindow = GetAppWindowForCurrentWindow();
                    m_AppWindow.SetIcon("Assets/cameraIcon.ico");
                }
                var titleBar = m_AppWindow.TitleBar;

                titleBar.ForegroundColor = Colors.White;
                titleBar.BackgroundColor = ConvertHexToColor("#121212");
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonBackgroundColor = ConvertHexToColor("#121212");
                titleBar.ButtonHoverForegroundColor = Colors.Gainsboro;
                titleBar.ButtonHoverBackgroundColor = ConvertHexToColor("#1f2023");
                titleBar.ButtonPressedForegroundColor = Colors.Gray;
                titleBar.ButtonPressedBackgroundColor = ConvertHexToColor("#121212");

                titleBar.InactiveBackgroundColor = ConvertHexToColor("#1a1b1c");
                titleBar.ButtonInactiveBackgroundColor = ConvertHexToColor("#1a1b1c");
                return true;
            }
            return false;
        }

        public static Color ConvertHexToColor(string hex)
        {
            // Remove the # symbol (if present)
            hex = hex.Replace("#", "");

            // Parsing the HEX code into components (R, G, B)
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            // Create and return a Color object
            return Color.FromArgb(255, r, g, b);
        }

        private void OpenSettingsPanel_Click(object sender, RoutedEventArgs e)
        {
            if (!_isOpen)
            {
                OpenPanel.Begin();
                _isOpen = true;
            }
        }

        private void CloseSettingsPanel_Click(object sender, RoutedEventArgs e)
        {
            ClosePanel.Begin();
            _isOpen = false;
        }

        private void takePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenPhotoPanel.Begin();
        }

        private void closePhPanel_Click(object sender, RoutedEventArgs e)
        {
            ClosePhotoPanel.Begin();
        }
    }
}
