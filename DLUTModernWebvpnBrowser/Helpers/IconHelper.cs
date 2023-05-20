using Microsoft.UI.Xaml.Controls;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLUTModernWebvpnBrowser.Helpers
{
    public static class IconHelper
    {
        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static Microsoft.UI.Xaml.Controls.IconSource ConvFavURLToIconSource(string url)
        {
            try
            {
                Uri faviconUrl = new(url);
                Microsoft.UI.Xaml.Controls.BitmapIconSource iconsource = new() { UriSource = faviconUrl, ShowAsMonochrome = false };
                return iconsource;
            }
            catch
            {
                logger.Error("处理页面图标时出错：" + url);
                Microsoft.UI.Xaml.Controls.IconSource iconsource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document };
                return iconsource;
            }
        }
    }
}
