using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics;
using WinRT.Interop;
using WinUICommunity.Common.Helpers;

namespace DLUTModernWebvpnBrowser.Helpers
{

    public static class TitleBarHelper
    {

        private static Grid _LeftDragColumn { get; set; }

        private static Grid _RightDragColumn { get; set; }

        private static Window _MainWindowObject { get; set; }

        private static AppWindow m_AppWindow { get; set; }
        private static TabView _Tabview { get; set; }
        private static int _Height { get; set; }

        public static void Initialize(Window MainWindowObject, TabView tabView, Grid LeftDragColumn, Grid RightDragColumn, int Height)
        {
            _LeftDragColumn = LeftDragColumn;
            _RightDragColumn = RightDragColumn;
            _MainWindowObject = MainWindowObject;
            _Height= Height;
            _Tabview = tabView;
            m_AppWindow = WindowHelper.GetAppWindowForCurrentWindow(_MainWindowObject);
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                m_AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                tabView.Loaded += AppTitleBar_Loaded;
                tabView.SizeChanged += AppTitleBar_SizeChanged;
            }
        }

        private static void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                SetDragRegionForCustomTitleBar(m_AppWindow);
            }
        }

        private static void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AppWindowTitleBar.IsCustomizationSupported() && m_AppWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                SetDragRegionForCustomTitleBar(m_AppWindow);
            }
        }

        public static double GetScaleAdjustment()
        {
            if (NativeMethods.GetDpiForMonitor(Win32Interop.GetMonitorFromDisplayId(DisplayArea.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(_MainWindowObject)), DisplayAreaFallback.Primary).DisplayId), NativeMethods.Monitor_DPI_Type.MDT_Effective_DPI, out var dpiX, out var _) != 0)
            {
                throw new Exception("Could not get DPI for monitor.");
            }

            return (double)(uint)(((long)dpiX * 100L + 48) / 96) / 100.0;
        }

        public static void SetDragRegionForCustomTitleBar(AppWindow appWindow)
        {
            if (AppWindowTitleBar.IsCustomizationSupported() && appWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                double scaleAdjustment = GetScaleAdjustment();
                List<RectInt32> list = new List<RectInt32>();
                RectInt32 item = default(RectInt32);
                item.X = 0;
                item.Y = 0;
                item.Height = (int)(_Height * scaleAdjustment);
                item.Width = (int)(( _LeftDragColumn.ActualWidth) * scaleAdjustment);
                list.Add(item);
                RectInt32 item2 = default(RectInt32);
                item2.X = (int)((_Tabview.ActualWidth - _RightDragColumn.ActualWidth) * scaleAdjustment);
                item2.Y = 0;
                item2.Height = (int)(_Height * scaleAdjustment);
                item2.Width = (int)(_RightDragColumn.ActualWidth * scaleAdjustment);
                list.Add(item2);
                RectInt32[] dragRectangles = list.ToArray();
                appWindow.TitleBar.SetDragRectangles(dragRectangles);
            }
        }
    }
}

