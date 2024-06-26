﻿// Copyright (c) Microsoft Corporation and Contributors.
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
using WinUICommunity;
using DLUTModernWebvpnBrowser.Configurations;
using Microsoft.Web.WebView2.Core;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;
using NLog;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DLUTModernWebvpnBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public NLog.Logger logger;
        private TabViewItem tabViewItem;
        private MainWindow mainWindow;
        public SettingPage()
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("打开设置页面");
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
            mainWindow = everything._mainwindow;
            base.OnNavigatedTo(e);

            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackwardConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(SourceElement);
            }
        }

        private void SourceElement_Loaded(object sender, RoutedEventArgs e)
        {
            App.themeService.SetThemeComboBoxDefaultItem(ThemePanel);
            App.themeService.SetBackdropComboBoxDefaultItem(Backdrop);
            Uid.Text = ApplicationConfig.GetSettings("Uid");
            Password.Password = ApplicationConfig.GetSettings("Password");
        }

        private void ThemePanel_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ApplicationConfig.SaveSettings("Theme", ((ComboBoxItem)ThemePanel.SelectedItem).Tag.ToString());
            App.themeService.OnThemeComboBoxSelectionChanged(sender);
        }

        private void Backdrop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.themeService.OnBackdropComboBoxSelectionChanged(sender);
        }
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        }

        private void Uid_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplicationConfig.SaveSettings("Uid", Uid.Text);
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ApplicationConfig.SaveSettings("Password", Password.Password);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("清除cookie");
            ContentDialog dialog = new ContentDialog();
            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "确认清除所有Cookie吗？";
            dialog.PrimaryButtonText = "确定";
            dialog.CloseButtonText = "取消";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = "你所有打开的网页状态都会丢失！";

            var result = await dialog.ShowAsync();
            if(result==ContentDialogResult.Primary)
            {
                WebView2 webView2 = new WebView2();
                webView2.CoreWebView2Initialized += (sender, args) =>
                {
                    webView2.CoreWebView2.CookieManager.DeleteAllCookies();
                    var builder = new AppNotificationBuilder()
                        .AddText("清除成功！");
                    var notificationManager = AppNotificationManager.Default;
                    notificationManager.Show(builder.BuildNotification());
                    webView2.CoreWebView2.Stop();
                    webView2.Close();
                };
                await webView2.EnsureCoreWebView2Async();
            }
        }
        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            logger.Info("打开日志文件夹");
            await Windows.System.Launcher.LaunchUriAsync(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\DLUTModernWebvpnBrowser\\Log"));
        }
    }
}
