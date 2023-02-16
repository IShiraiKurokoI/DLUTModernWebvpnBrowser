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
using WinUICommunity.Common.Helpers;
using DLUTModernWebvpnBrowser.Configurations;
using Microsoft.Web.WebView2.Core;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DLUTModernWebvpnBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private TabViewItem tabViewItem;
        private TabviewPage tabviewPage;
        public SettingPage()
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
            TabViewAndItem tabViewAndItem = ((TabViewAndItem)e.Parameter);
            tabViewItem = tabViewAndItem._item;
            tabviewPage = tabViewAndItem._tabview;
            base.OnNavigatedTo(e);

            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackwardConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(SourceElement);
            }
        }

        private void SourceElement_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeHelper.SetComboBoxDefaultItem(ThemePanel);
            Uid.Text = ApplicationConfig.GetSettings("Uid");
            Password.Password = ApplicationConfig.GetSettings("Password");
        }

        private void ThemePanel_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ApplicationConfig.SaveSettings("Theme", ((ComboBoxItem)ThemePanel.SelectedItem).Tag.ToString());
            ThemeHelper.ComboBoxSelectionChanged(sender);
        }
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        }

        private void Uid_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplicationConfig.SaveSettings("Uid", Uid.Text);
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ApplicationConfig.SaveSettings("Password", Password.Password);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var builder = new AppNotificationBuilder()
                .AddText("⚠抱歉，暂未实现！⚠");
            var notificationManager = AppNotificationManager.Default;
            notificationManager.Show(builder.BuildNotification());
        }
    }
}
