// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUICommunity;
using DLUTModernWebvpnBrowser.Configurations;
using DLUTModernWebvpnBrowser.Entities;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DLUTModernWebvpnBrowser
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static ThemeManager themeManager { get; private set; }
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            //WebvpnKey.Key = "Wxzxvpn2023key@$";
            //WebvpnKey.IV = "Wxzxvpn2023key@$";
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--proxy-server=\"direct://\"");
            m_window = new MainWindow();
            ElementTheme SettingsTheme = ElementTheme.Default;
            if (ApplicationConfig.GetSettings("Theme") != null)
            {
                if (ApplicationConfig.GetSettings("Theme") == "Light")
                {
                    SettingsTheme = ElementTheme.Light;
                }
                if (ApplicationConfig.GetSettings("Theme") == "Dark")
                {
                    SettingsTheme = ElementTheme.Dark;
                }
            }
            else
            {
                ApplicationConfig.SaveSettings("Theme", "Default");
            }

            themeManager = ThemeManager.Initialize(m_window, new ThemeOptions
            {
                BackdropType = BackdropType.DesktopAcrylic,
                ElementTheme = SettingsTheme,
                TitleBarCustomization = new TitleBarCustomization
                {
                    TitleBarType = TitleBarType.AppWindow
                }
            });
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(m_window);
            Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            if (appWindow is not null)
            {
                Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
                if (displayArea is not null)
                {
                    var CenteredPosition = appWindow.Position;
                    CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
                    CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
                    appWindow.Move(CenteredPosition);
                }
            }
            m_window.Activate();
        }

        private Window m_window;
    }
}
