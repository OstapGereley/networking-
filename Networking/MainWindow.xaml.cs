using System.Threading;
using System.Windows;
using Networking.Functionality;

namespace Networking
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
            var test = new PingScan();
            object resultList = null;

            //TODO: threads sync\background worker
            var thread = new Thread(() => resultList = test.StartPing());
            thread.Start();
            thread.Join();
            var arpWork = new ArpTable();
            var arpResultList = arpWork.GetRecords();
            arpWork.Dispose();
        }
    }
}