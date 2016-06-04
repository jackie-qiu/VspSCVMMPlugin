using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Nuage.VSDClient.Main
{
    /// <summary>
    /// SelectWin.xaml Logical
    /// </summary>
    public partial class SelectWin : Window
    {
        IEnumerable<object> objects;
        ListBox parent;
        string object_type;
        public SelectWin(IEnumerable<object> objects, ListBox parent, string object_type)
        {
            this.objects = objects;
            this.parent = parent;
            this.object_type = object_type;
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            _selectWin.Title += this.object_type;
            foreach (object item in objects)
                _selectObjects.Items.Add(item);
        }
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (_selectObjects.SelectedIndex == -1)
                return;

            parent.Items.Add(_selectObjects.SelectedItem);
            this.Close();
        }

        private void _MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Select_Click(sender, e);
        }

        private void _SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectObjects.SelectedIndex != -1)
                _Select.IsEnabled = true;
        }

        private void _selectWin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _Select.IsEnabled)
                Select_Click(sender, e);
        }
    }
}
