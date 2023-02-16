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
        public TabviewPage _tabview;
        public TabViewItem _item;
        public string url;
        public Everything(TabviewPage tabviewPage, TabViewItem tabViewItem,string url)
        {
            _tabview = tabviewPage;
            _item = tabViewItem;
            this.url = url;
        }
    }
}
