﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Networking.Functionality;
using Networking.Model;
using MahApps.Metro.Controls;

namespace Networking
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        private ObservableCollection<NetworkDeviceModel> resultList;
        private ObservableCollection<NetworkDeviceModel> tempList;
        private ObservableCollection<NetworkConnectionsModel> connectionsModels; 
        
        private static object _lock = new object();

        public MainWindow()
        {
            InitializeComponent();
            ArpTable.LoadMacVendors();
        }

        #region PingStuff
        private void ScanPingButton_OnClick(object sender, RoutedEventArgs e)
        {
            var bgw1 = new BackgroundWorker();
            bgw1.DoWork += bgw1_DoWork;
            bgw1.RunWorkerCompleted += bgw1_RunCompleted;
            bgw1.RunWorkerAsync();
        }

        private void bgw1_DoWork(object sender, DoWorkEventArgs e)
        {
            var ping = new PingScan();
            tempList =  new ObservableCollection<NetworkDeviceModel>(ping.StartPing());
        }

        private void bgw1_RunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (resultList == null ||resultList.Count == 0 )
            {
                resultList = tempList;
                BindingOperations.EnableCollectionSynchronization(resultList, _lock);
                NetworkInfoGrid.ItemsSource = resultList;

            }
            else
            {
                foreach (var dev in tempList)
                {
                    //if (resultList.Exists(a => a.Ip == dev.Ip))
                    if(resultList.Where(a => a.Ip == dev.Ip).ToList().Count > 0)
                    {
                        resultList.Where(a => a.Ip == dev.Ip).Select(a =>
                        {
                            a.HostName = dev.HostName;
                            a.IsActive = dev.IsActive;
                            return a;
                        });
                    }
                    else
                    {
                        resultList.Add(dev);
                    }
                }
            }
            NetworkInfoGrid.Items.Refresh();


        }
        #endregion

        #region ArpStuff
        private void ScanArpButton_OnClick(object sender, RoutedEventArgs e)
        {
            
            var bgw2 = new BackgroundWorker();
            bgw2.DoWork += bgw2_DoWork;
            bgw2.RunWorkerCompleted += bgw2_RunCompleted;
            bgw2.RunWorkerAsync();

        }

        private void bgw2_DoWork(object sender, DoWorkEventArgs e)
        {
            var arp = new ArpTable();
            tempList = new ObservableCollection<NetworkDeviceModel>(arp.GetRecords());
        }

        private void bgw2_RunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (resultList == null || resultList.Count == 0)
            {
                resultList = tempList;
                BindingOperations.EnableCollectionSynchronization(resultList, _lock);
                NetworkInfoGrid.ItemsSource = resultList;

            }
            else
            {
                foreach (var dev in tempList)
                {
                    if(resultList.Where(a => a.Ip == dev.Ip).ToList().Count > 0)
                    {
                        foreach (var res in resultList.Where(a => a.Ip == dev.Ip))
                        {
                            res.Mac = dev.Mac;
                        }
                    }
                    else
                    {
                        resultList.Add(dev);
                    }
                }
            }

            ArpTable.FillVendors(resultList);
            NetworkInfoGrid.Items.Refresh();
            }
        #endregion

        #region ConnectionsStuff
        private void ShowConnectionsButton_OnClick(object sender, RoutedEventArgs e)
        {
            NetworkControlGrid.Items.Clear();
            var connections = new ActiveConnections();
            connectionsModels = new ObservableCollection<NetworkConnectionsModel>(connections.ShowActiveTcpConnections());
            NetworkControlGrid.ItemsSource = connectionsModels;
        }


        

        private void BlockSelectedConnection_OnClick(object sender, RoutedEventArgs e)
        {
            if (NetworkControlGrid.SelectedItem == null) return;
            var item = (NetworkConnectionsModel) NetworkControlGrid.SelectedItem;
            FWCtrl.AddInRule(item.DestinationIp, item.DestinationPort);
            FWCtrl.AddOutRule(item.DestinationIp, item.DestinationPort);
            foreach (var el in connectionsModels.Where(el => el.DestinationIp == item.DestinationIp))
            {
                el.FirewallRule = true;
            }
            NetworkControlGrid.Items.Refresh();
        }

        

        private void DeleteRuleButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (NetworkControlGrid.SelectedItem == null) return;
            var item = (NetworkConnectionsModel)NetworkControlGrid.SelectedItem;
            FWCtrl.DeleteRule(item.DestinationIp, item.DestinationPort);
            foreach (var el in connectionsModels.Where(el => el.DestinationIp == item.DestinationIp))
            {
                el.FirewallRule = false;
            }
            NetworkControlGrid.Items.Refresh();
        }
        #endregion
    }

}
