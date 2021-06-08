using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using StopHandler.ViewModels.Base;
using StopHandler.Models.POST;

namespace StopHandler.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        #region Служебные поля

        private POSTServer _MyPOSTServer;
        
        #endregion

        #region Log
        private string _Log = "";

        public string Log { get => _Log; set => Set(ref _Log, value); }
        #endregion

        #region Functions

        public void AddLog(string msg)
        {
            _Log += "[" + DateTime.Now + "] " + msg + "\n";
        }

        #endregion

        public MainWindowViewModel()
        {
            _MyPOSTServer = POSTServer.GetInstance();
            _MyPOSTServer.onLogUpdate += AddLog;

            _MyPOSTServer.Start();
        }
    }
}
