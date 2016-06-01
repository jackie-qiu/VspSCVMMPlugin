﻿using System;
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
    /// IngressACL.xaml 的交互逻辑
    /// </summary>
    public partial class IngressACL : Window
    {
        INuageClient rest_client;
        string domain_id;
        ListBox parent;
        public string IngressACLName { set; get; }
        public IngressACL(INuageClient client, string domain_id, ListBox parent)
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
            string priority = null;
            if (this.parent.HasItems)
            {
                MessageBox.Show("Unable to create duplicate ACL template", "Error");
                return;
            }

            if (!string.IsNullOrEmpty(this._description.Text))
            {
                desc = this._description.Text;
            }

            if (!string.IsNullOrEmpty(this._priority.Text))
            {
                priority = this._priority.Text.Replace(" ", "");
            }

            try
            {
                NuageInboundACL ingress = rest_client.CreateL3DomainIngressACLTmplt(domain_id, _nameBox.Text, desc,
                    priority, _addressSpoofing.IsChecked.Value, _allowIP.IsChecked.Value, _allowNonIP.IsChecked.Value);
                if (ingress != null)
                {
                    this.parent.Items.Add(ingress);
                    this.Close();
                }
            }
            catch (NuageException)
            {
                string error = string.Format("name = {0}, assocEntityType = domain is in use, retry with a different set of values.", this._nameBox.Text);
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
    }
}