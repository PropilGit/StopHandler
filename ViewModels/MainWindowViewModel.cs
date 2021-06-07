using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using StopHandler.ViewModels.Base;

namespace StopHandler.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        #region Port
        private string _Port = "48654";

        public string Port { get => _Port; set => Set(ref _Port, value); }
        #endregion

        #region Log
        private string _Log = "";

        public string Log { get => _Log; set => Set(ref _Log, value); }
        #endregion

        #region Commands

        #endregion

        public MainWindowViewModel()
        {
        }
    }
}
