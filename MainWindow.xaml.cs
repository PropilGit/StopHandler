using StopHandler.core;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StopHandler
{
    
    public partial class MainWindow : Window
    {
        POSTServer serv;

        public MainWindow()
        {
            InitializeComponent();
        }

        void OnLogUpdate(string mes)
        {
            Dispatcher.Invoke(() =>
            {
                rtbl_log.Text += "[" + DateTime.Now + "] " + mes + "\n";
            });
        }
        void OnPOSTReqest(IPOSTCommand cmd)
        {
            Dispatcher.Invoke(() =>
            {
                
            });
        }

        private void StopHandler_Loaded(object sender, RoutedEventArgs e)
        {
            serv = POSTServer.GetInstance(Int32.Parse(tb_port.Text));
            serv.onLogUpdate += OnLogUpdate;
            serv.onPOSTRequest += OnPOSTReqest;
            serv.Start();
        }

        private void StopHandler_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            serv.Stop();
        }
    }
}
