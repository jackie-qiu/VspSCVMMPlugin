using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid;
namespace nuageVSDClientMain
{
    /// <summary>
    /// MainWindow.xaml Logical
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var selObject = new Person();
            selObject.FirstName = "Zhigang";
            selObject.LastName = "Qiu";

            this.DataContext = selObject;
        }

        private void OnPreparePropertyItem(object sender, PropertyItemEventArgs e)
        {

        }

        private void onAddClick(object sender, RoutedEventArgs e)
        {
            var selObject = new Person2();
            selObject.FirstName = "Xueying";
            selObject.LastName = "Yuan";
            selObject.Sex = "Female";
            this.DataContext = selObject;


        }

        private void onDelClick(object sender, RoutedEventArgs e)
        {

        }

        private void _Enterprises_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void _Subnets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void _Domains_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void _Zones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void _IngressPolicy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void _EgressPolicy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void _PolicyGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        
        private void _layoutPolicy_IsSelectedChanged(object sender, EventArgs e)
        {
            if (this._layoutSubnet != null)
                this._layoutSubnet.Title = "Security Policy Entries";
        }

        private void _layoutPolicyGroup_IsSelectedChanged(object sender, EventArgs e)
        {
            if (this._layoutSubnet != null)
                this._layoutSubnet.Title = "vPorts";

        }
        private void _layoutDesighDocument_IsSelectedChanged(object sender, EventArgs e)
        {
            if (this._layoutSubnet != null)
                this._layoutSubnet.Title = "Subnet";

        }

    }


    public class Person
    {
        [Category("Information")]
        public string FirstName { get; set; }
        [Category("Information")]
        public string LastName { get; set; }
    }

    public class Person2
    {
        [Category("Information")]
        public string FirstName { get; set; }
        [Category("Information")]
        public string LastName { get; set; }
        [Category("Information")]
        public string Sex { get; set; }
    }
}
