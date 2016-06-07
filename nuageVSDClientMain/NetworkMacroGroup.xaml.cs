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
    /// NetworkMacroGroup.xaml Logical
    /// </summary>
    public partial class NetworkMacroGroup : Window
    {
        INuageClient rest_client;
        string ent_id;
        ListBox parent;
        public string NetworkMacroGroupName { set; get; }

        public NetworkMacroGroup(INuageClient client, string ent_id, ListBox parent)
        {
            this.rest_client = client;
            this.parent = parent;
            this.ent_id = ent_id;

            InitializeComponent();
            _nameBox.DataContext = this;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string desc = null;

            if (!string.IsNullOrEmpty(this._description.Text))
            {
                desc = this._description.Text;
            }

            try
            {
                NuageNetworkMacroGroups network_macro_group = rest_client.CreateNetworkMacroGroup(this.ent_id, _nameBox.Text, desc);
                if (network_macro_group != null)
                {
                    this.parent.Items.Add(network_macro_group);
                    parent.SelectedIndex = parent.Items.Count - 1;
                    this.Close();
                }

            }
            catch (NuageException)
            {
                string error = string.Format("name({0}) is in use.Please retry with a different value.", this._nameBox.Text);
                MessageBox.Show(error);
                return;
            }
        }

        private void _name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Windows.Controls.Validation.GetHasError(_nameBox) == true)
                _Create.IsEnabled = false;
            else
                _Create.IsEnabled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _Create.IsEnabled)
                Create_Click(sender, e);
        }
    }
}
