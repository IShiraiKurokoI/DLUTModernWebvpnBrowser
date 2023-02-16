// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using DLUTModernWebvpnBrowser.Entities;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Foundation.Metadata;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DLUTModernWebvpnBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomWebPage : Page
    {
        private TabViewItem tabViewItem;
        private TabviewPage tabviewPage;
        public CustomWebPage()
        {
            this.InitializeComponent();
        }

        public void PrepareConnectedAnimation(ConnectedAnimationConfiguration config)
        {
            var anim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", SourceElement);

            if (config != null && ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                anim.Configuration = config;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Everything everything = ((Everything)e.Parameter);
            tabViewItem = everything._item;
            tabviewPage = everything._tabview;
            WebView.Source = new Uri(everything.url);
            base.OnNavigatedTo(e);

            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackwardConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(SourceElement);
            }
        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            tabviewPage.GetTabView().TabItems.Add(tabviewPage.CreateNewTab(tabviewPage.GetTabView().TabItems.Count));
            tabviewPage.GetTabView().SelectedIndex = tabviewPage.GetTabView().TabItems.Count - 1;
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            tabviewPage.OpenSetting();
        }

        private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            tabviewPage.OpenAbout();
        }

        private void MenuFlyoutItem_Click_3(object sender, RoutedEventArgs e)
        {
            tabviewPage.OpenCustom("反馈", "https://github.com/IShiraiKurokoI/DLUTModernWebvpnBrowser/issues");
        }

        private void MenuFlyoutItem_Click_4(object sender, RoutedEventArgs e)
        {
            tabviewPage.OpenCustom("下载", "edge://downloads/all");
        }

        private void MenuFlyoutItem_Click_5(object sender, RoutedEventArgs e)
        {
            var builder = new AppNotificationBuilder()
                .AddText("⚠抱歉，暂未实现！⚠");
            var notificationManager = AppNotificationManager.Default;
            notificationManager.Show(builder.BuildNotification());
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            WebView.GoBack();
        }

        private void RefreshOrStop_Click(object sender, RoutedEventArgs e)
        {
            if (WebView.IsLoaded)
            {
                WebView.CoreWebView2.Reload();
            }
            else
            {
                WebView.CoreWebView2.Stop();
            }
        }

        private void WebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;
            WebView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
            WebView.CoreWebView2.Settings.IsZoomControlEnabled = true;
            WebView.CoreWebView2.Settings.IsGeneralAutofillEnabled = true;
            WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;
            WebView.CoreWebView2.NewWindowRequested += (sender, args) =>
            {
                WebView.CoreWebView2.ExecuteScriptAsync("window.location.href='" + args.Uri.ToString() + "'");
                args.Handled = true;
            };
            WebView.CoreWebView2.ContentLoading += (sender, args) =>
            {
                RefreshOrStopIcon.Glyph = "\xe711";
            };
            WebView.CoreWebView2.DOMContentLoaded += (sender, args) =>
            {
                RefreshOrStopIcon.Glyph = "\xe72c";
                tabViewItem.Header = WebView.CoreWebView2.DocumentTitle;
            };
        }

        private void MenuFlyoutItem_Click_6(object sender, RoutedEventArgs e)
        {
            tabviewPage.OpenCustom("历史记录", "edge://history/all");
        }
        private void GoForward_Click(object sender, RoutedEventArgs e)
        {
            WebView.GoForward();
        }
    }
}
