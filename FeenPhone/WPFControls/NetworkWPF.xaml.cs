﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FeenPhone.WPFControls
{
    /// <summary>
    /// Interaction logic for NetworkWPF.xaml
    /// </summary>
    public partial class NetworkWPF : UserControl
    {

        public NetworkWPF()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static DependencyProperty IsServerProperty = DependencyProperty.Register("IsServer", typeof(bool?), typeof(NetworkWPF), new PropertyMetadata(false, OnIsServerChanged));

        public bool? IsServer
        {
            get { return (bool?)this.GetValue(IsServerProperty); }
            set { this.SetValue(IsServerProperty, value); }
        }

        Alienseed.BaseNetworkServer.ServerHost server = null;
        private static void OnIsServerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NetworkWPF target = d as NetworkWPF;
            if (target != null)
            {
                if ((bool?)e.NewValue == true && target.server == null)
                {
                    target.server = new Alienseed.BaseNetworkServer.ServerHost();
                }
                else
                {
                    target.server.Dispose();
                    target.server = null;
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

    }
}