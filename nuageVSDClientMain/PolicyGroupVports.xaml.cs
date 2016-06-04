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
    /// PolicyGroupVports.xaml Logical
    /// </summary>
    public partial class PolicyGroupVports : Window
    {
        private INuageClient rest_client;
        private string pg_id;
        private string domain_id;
        private ListBox parent;
        public PolicyGroupVports(INuageClient client, string domain_id, string pg_id, ListBox parent)
        {
            this.rest_client = client;
            this.parent = parent;
            this.pg_id = pg_id;
            this.domain_id = domain_id;
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            InitvPortListBox(this.domain_id);
        }

        private void InitvPortListBox(string domain_id)
        {
            try
            {
                List<NuageVport> vports_in_domain = rest_client.GetVportInDomain(domain_id, null);
                List<NuageVport> vports_in_pg = rest_client.GetVportInPolicyGroup(pg_id);

                _vPorts.Items.Clear();
                if (vports_in_domain == null)
                    return;

                if (vports_in_pg != null)
                {
                    NuageCompare comp = new NuageCompare();
                    List<NuageBase> vport_not_in_pg = vports_in_domain.Except(vports_in_pg, comp).ToList();
                    foreach (NuageVport item in vport_not_in_pg)
                    {
                        _vPorts.Items.Add(item);
                    }
                }
                else
                {
                    foreach (NuageVport item in vports_in_domain)
                    {
                        _vPorts.Items.Add(item);
                    }
                }

            }
            catch(NuageException)
            {
                string err = string.Format("Get vport in domain {0} failed.", domain_id);
                MessageBox.Show(err, "Error");
                this.Close();
            }
        }

        private void AddvPorttoPolicyGroup()
        {
            if (this._vPorts.SelectedIndex == -1)
                return;

            try
            {
                List<string> vports_ids = new List<string>();

                foreach (NuageVport item in this._vPorts.SelectedItems)
                {
                    vports_ids.Add(item.ID);
                }

                rest_client.AddvPortsToPolicyGroup(this.pg_id, vports_ids);

                foreach (NuageVport item in this._vPorts.SelectedItems)
                {
                    this.parent.Items.Add(item);
                }

            }
            catch (NuageException)
            {
                string err = string.Format("Add vport to policy group {0} failed.", pg_id);
                MessageBox.Show(err, "Error");
            }
            finally
            {
                this.Close();
            }

        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            AddvPorttoPolicyGroup();
        }

        private void _MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            AddvPorttoPolicyGroup();
        }

        private void _SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_vPorts.SelectedIndex != -1)
                _Select.IsEnabled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _Select.IsEnabled)
                Select_Click(sender, e);
        }
    }


}
