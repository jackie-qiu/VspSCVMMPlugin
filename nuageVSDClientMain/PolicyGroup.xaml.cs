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
    /// PolicyGroup.xaml Logical
    /// </summary>
    public partial class PolicyGroup : Window
    {
        INuageClient rest_client;
        string domain_id;
        ListBox parent;
        public string PGName { set; get; }

        public PolicyGroup(INuageClient client, string domain_id, ListBox parent)
        {
            this.rest_client = client;
            this.parent = parent;
            this.domain_id = domain_id;

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
                NuagePolicyGroup pg = rest_client.CreatePolicyGroup(this.domain_id, _nameBox.Text, desc);
                if (pg != null)
                {
                    this.parent.Items.Add(pg);
                    this.Close();
                }

            }
            catch (NuageException)
            {
                string error = string.Format("Cannot create duplicate entity.Another PolicyGroup with the same name {0} exists.", this._nameBox.Text);
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
