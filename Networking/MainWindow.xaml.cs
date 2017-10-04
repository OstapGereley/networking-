using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Networking.Functionality;
using Networking.Model;

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

        private bool flag = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region PingStuff
        private void ScanPingButton_OnClick(object sender, RoutedEventArgs e)
        {
            ScanPingButton.Visibility = Visibility.Hidden;
            ScanArpButton.Visibility = Visibility.Hidden;
            StatusBlock.Text = "Scanning by ping. Please wait..";
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

            ScanPingButton.Visibility = Visibility.Visible;
            ScanArpButton.Visibility = Visibility.Visible;
            StatusBlock.Text = "Ready.";


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

            if (ArpTable.LoadMacVendors() == 0)
            {
                StatusBlock.Text = "File loaded succesfuly";
                ArpTable.FillVendors(resultList);
                
            }
            else
            {
                StatusBlock.Text = "File with vendors not found";
            }
            
            NetworkInfoGrid.Items.Refresh();
            }
        #endregion

        #region ConnectionsStuff
        private void ShowConnectionsButton_OnClick(object sender, RoutedEventArgs e)
        {
            NetworkControlGrid.Items.Clear();
            connectionsModels = new ObservableCollection<NetworkConnectionsModel>(ActiveConnections.ShowActiveTcpConnections());
            NetworkControlGrid.ItemsSource = connectionsModels;
        }


        

        private void BlockSelectedConnection_OnClick(object sender, RoutedEventArgs e)
        {
            if (NetworkControlGrid.SelectedItem == null) return;
            var item = (NetworkConnectionsModel) NetworkControlGrid.SelectedItem;
            FirewallControl.AddInRule(item.DestinationIp, item.DestinationPort);
            FirewallControl.AddOutRule(item.DestinationIp, item.DestinationPort);
            foreach (var el in connectionsModels.Where(el => el.DestinationIp == item.DestinationIp))
            {
                el.FirewallRule = true;
            }
            NetworkControlGrid.Items.Refresh();
            StatusBlock.Text = "Rule created succesfully";
        }

        

        private void DeleteRuleButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (NetworkControlGrid.SelectedItem == null) return;
            var item = (NetworkConnectionsModel)NetworkControlGrid.SelectedItem;
            FirewallControl.DeleteRule(item.DestinationIp, item.DestinationPort);
            foreach (var el in connectionsModels.Where(el => el.DestinationIp == item.DestinationIp))
            {
                el.FirewallRule = false;
            }
            NetworkControlGrid.Items.Refresh();
            StatusBlock.Text = "Rule deleted succesfully";
        }
        #endregion

        private void ExitItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AboutItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (flag)
            {
                FunctionsControl.Visibility = Visibility.Collapsed;
                AboutBlock.Visibility = Visibility.Visible;
                AboutBlock.Text = "Utility written by Ostap Gereley \n as a thesis project. \n Lviv 2016";
                flag = false;
            }
            else
            {
                FunctionsControl.Visibility = Visibility.Visible;
                AboutBlock.Visibility = Visibility.Collapsed;
                flag = true;
            }

        }
    }

}
