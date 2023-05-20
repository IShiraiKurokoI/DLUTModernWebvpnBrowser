using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLUTModernWebvpnBrowser.Entities
{
    public class TabViewAndItem
    {
        public MainWindow _tabview;
        public TabViewItem _item;
        public TabViewAndItem (MainWindow tabviewPage,TabViewItem tabViewItem)
        {
            _tabview= tabviewPage;
            _item= tabViewItem;
        }
    }
}
