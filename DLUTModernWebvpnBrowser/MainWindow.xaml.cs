// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Windowing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUICommunity;
using DLUTModernWebvpnBrowser.Helpers;
using Microsoft.UI;
using Windows.Graphics;
using WinRT.Interop;
using DLUTModernWebvpnBrowser.Entities;
using DLUTModernWebvpnBrowser.Pages;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DLUTModernWebvpnBrowser
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        [DllImport("user32.dll")]
        static extern int GetDpiForWindow(IntPtr hwnd);
        private AppWindow m_AppWindow;
        public MainWindow()
        {
            this.InitializeComponent();
            int dpi = GetDpiForWindow(WinRT.Interop.WindowNative.GetWindowHandle(this));
            m_AppWindow = this.AppWindow;
            m_AppWindow.Resize(new SizeInt32((int)(1570 * (double)((double)dpi / (double)120)), (int)(800 * (double)((double)dpi / (double)120))));
            m_AppWindow.SetIcon("Assets/logo.ico");
            this.Title = "大连理工大学WebVPN浏览器";
        }

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            Tabs.TabItemsChanged += Tabs_TabItemsChanged;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(CustomDragRegion);
            CustomDragRegion.MinWidth = 188;
            SetupWindow();
        }

        private void Tabs_TabItemsChanged(TabView sender, Windows.Foundation.Collections.IVectorChangedEventArgs args)
        {
            // If there are no more tabs, close the window.
            if (sender.TabItems.Count == 0)
            {
                this.Close();
            }
            // If there is only one tab left, disable dragging and reordering of Tabs.
            else if (sender.TabItems.Count == 1)
            {
                sender.CanReorderTabs = false;
                sender.CanDragTabs = false;
            }
            else
            {
                sender.CanReorderTabs = true;
                sender.CanDragTabs = true;
            }
        }

        public void AddNewTab()
        {
            var tab = CreateNewTab();
            Tabs.TabItems.Add(tab);
            Tabs.SelectedIndex = Tabs.TabItems.Count-1;
        }

        void SetupWindow()
        {
            AddNewTab();
        }


        public TabViewItem CreateNewTab()
        {
            TabViewItem newItem = new TabViewItem();
            newItem.Header = "新页面";
            Frame frame = new Frame();
            frame.Navigate(typeof(WebviewPage), new Everything(this, newItem,null));
            newItem.Content = frame;
            return newItem;
        }

        public void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            AddNewTab();
        }

        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }

        public TabView GetTabView()
        {
            return Tabs;
        }

        public void OpenAbout()
        {
            TabViewItem newItem = new TabViewItem();
            newItem.Header = "关于";
            newItem.IconSource = new Microsoft.UI.Xaml.Controls.FontIconSource() { Glyph = "\xe946" };
            Frame frame = new Frame();
            frame.Navigate(typeof(AboutPage), new Everything(this, newItem,null));
            newItem.Content = frame;
            Tabs.TabItems.Add(newItem);
            Tabs.SelectedIndex = Tabs.TabItems.Count - 1;
        }

        public void OpenSetting()
        {
            TabViewItem newItem = new TabViewItem();
            newItem.Header = "设置";
            newItem.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Setting };
            Frame frame = new Frame();
            frame.Navigate(typeof(SettingPage), new Everything(this, newItem, null));
            newItem.Content = frame;
            Tabs.TabItems.Add(newItem);
            Tabs.SelectedIndex = Tabs.TabItems.Count - 1;
        }

        public void OpenCustom(string name, string url)
        {
            TabViewItem newItem = new TabViewItem();
            newItem.Header = name;
            Frame frame = new Frame();
            frame.Navigate(typeof(WebviewPage), new Everything(this, newItem, url));
            newItem.Content = frame;
            Tabs.TabItems.Add(newItem);
            Tabs.SelectedIndex = Tabs.TabItems.Count - 1;
        }
    }
}
