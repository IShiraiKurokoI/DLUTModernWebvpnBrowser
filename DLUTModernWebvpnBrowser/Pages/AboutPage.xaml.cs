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
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DLUTModernWebvpnBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public string Version = string.Format("版本：{0}.{1}.{2}.{3}",
                        Package.Current.Id.Version.Major,
                        Package.Current.Id.Version.Minor,
                        Package.Current.Id.Version.Build,
                        Package.Current.Id.Version.Revision);
        private TabViewItem tabViewItem;
        private TabviewPage tabviewPage;
        public AboutPage()
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

        private void HyperlinkButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText("https://github.com/IShiraiKurokoI/DLUTModernWebvpnBrowser");
            Clipboard.SetContent(dataPackage);
            var builder = new AppNotificationBuilder()
                .AddText("复制成功！");
            var notificationManager = AppNotificationManager.Default;
            notificationManager.Show(builder.BuildNotification());
        }

        private void SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            tabviewPage.OpenCustom("项目地址", "https://github.com/IShiraiKurokoI/DLUTModernWebvpnBrowser");
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            tabviewPage.OpenCustom("Github主页", "https://github.com/IShiraiKurokoI");
        }

        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            tabviewPage.OpenCustom("BiliBili主页", "https://space.bilibili.com/310144483");
        }
    }
}
