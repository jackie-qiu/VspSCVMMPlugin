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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nuage.VSDClient.Main
{
    /// <summary>
    /// NewOrganization.xaml Logical
    /// </summary>
    public partial class Organization : Window
    {
        INuageClient rest_client;
        ListBox parent;
        public string Name { set; get; }
        public Organization(INuageClient client, ListBox parent)
        {
            this.rest_client = client;
            this.parent = parent;
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
                NuageEnterprise ent = rest_client.CreateEnterprise(this._nameBox.Text, desc);
                if (ent != null)
                {
                    this.parent.Items.Add(ent);
                    this.Close();
                }
            }
            catch(NuageException)
            {
                string error = string.Format("Another Enterprise with the same name = {0} exists.", this._nameBox.Text);
                MessageBox.Show(error, "Error");
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

    public class RequiredValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null )
            {
                return new ValidationResult(false, "Must not be empty");
            }

            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "Must not be empty");
            }

            return ValidationResult.ValidResult;
        }
    }
}
