using StopHandler.ViewModels;
using System.Windows;

namespace StopHandler
{
    
    public partial class MainWindow : Window
    {
        MainWindowViewModel vm;

        public MainWindow()
        {
            //InitializeComponent();

            vm = new MainWindowViewModel();
            this.DataContext = vm;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            vm.CloseApplication();
        }
    }
}
