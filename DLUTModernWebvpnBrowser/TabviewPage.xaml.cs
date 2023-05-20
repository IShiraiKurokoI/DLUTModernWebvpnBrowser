// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using DLUTModernWebvpnBrowser.Entities;
using DLUTModernWebvpnBrowser.Pages;
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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DLUTModernWebvpnBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabviewPage : Page
    {
        public TabviewPage()
        {
            this.InitializeComponent();
        }

        public TabView GetTabView()
        {
            return TabView;
        }

        private void TabView_Loaded(object sender, RoutedEventArgs e)
        {
           (sender as TabView).TabItems.Add(CreateNewTab(0));
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
            sender.TabItems.Add(CreateNewTab(sender.TabItems.Count));
            sender.SelectedIndex= sender.TabItems.Count-1;
        }

        public void OpenSetting()
        {
            TabViewItem newItem = new TabViewItem();
            newItem.Header = "设置";
            newItem.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Setting };
            Frame frame = new Frame();
            frame.Navigate(typeof(SettingPage), new TabViewAndItem(this, newItem));
            newItem.Content = frame;
            TabView.TabItems.Add(newItem);
            TabView.SelectedIndex= TabView.TabItems.Count-1;
        }

        public void OpenAbout()
        {
            TabViewItem newItem = new TabViewItem();
            newItem.Header = "关于";
            newItem.IconSource = new Microsoft.UI.Xaml.Controls.FontIconSource() { Glyph = "\xe946" };
            Frame frame = new Frame();
            frame.Navigate(typeof(AboutPage), new TabViewAndItem(this, newItem));
            newItem.Content = frame;
            TabView.TabItems.Add(newItem);
            TabView.SelectedIndex= TabView.TabItems.Count-1;
        }

        public void OpenCustom(string Title,string url)
        {
            TabViewItem newItem = new TabViewItem();
            newItem.Header = Title;
            Frame frame = new Frame();
            frame.Navigate(typeof(CustomWebPage), new Everything(this, newItem,url));
            newItem.Content = frame;
            TabView.TabItems.Add(newItem);
            TabView.SelectedIndex= TabView.TabItems.Count-1;
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }

        public TabViewItem CreateNewTab(int index)
        {
            TabViewItem newItem = new TabViewItem();
            newItem.Header = "Webvpn主页";
            Frame frame = new Frame();
            frame.Navigate(typeof(WebViewPage),new TabViewAndItem(this,newItem));
            newItem.Content = frame;
            TabView.SelectedItem = newItem;
            return newItem;
        }
    }
}
