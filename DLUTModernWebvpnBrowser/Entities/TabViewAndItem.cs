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
        public TabviewPage _tabview;
        public TabViewItem _item;
        public TabViewAndItem (TabviewPage tabviewPage,TabViewItem tabViewItem)
        {
            _tabview= tabviewPage;
            _item= tabViewItem;
        }
    }
}
