using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLUTModernWebvpnBrowser.Entities
{
    public class Everything
    {
        public MainWindow _mainwindow;
        public TabViewItem _item;
        public string url;
        public Everything(MainWindow mainWindow, TabViewItem tabViewItem, string url)
        {
            _mainwindow = mainWindow;
            _item = tabViewItem;
            this.url = url;
        }
    }
}
