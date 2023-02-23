using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLUTModernWebvpnBrowser.Entities
{
    public class MainWindowAndItem
    {
        public MainWindow _mainWindow;
        public TabViewItem _item;
        public MainWindowAndItem (MainWindow mainWindow,TabViewItem tabViewItem)
        {
            _mainWindow = mainWindow;
            _item= tabViewItem;
        }
    }
}
