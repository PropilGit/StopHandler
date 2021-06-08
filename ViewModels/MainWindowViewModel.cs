using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using StopHandler.ViewModels.Base;
using StopHandler.Models.POST;
using StopHandler.Models;

namespace StopHandler.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        POSTServer _MyPOSTServer;

        public MainWindowViewModel()
        {
            _MyPOSTServer = POSTServer.GetInstance();
            _MyPOSTServer.onLogUpdate += AddLog;
            _MyPOSTServer.onPOSTRequest += OnPOSTRequest;

            _MyPOSTServer.Start();
        }
        ~MainWindowViewModel()
        {
            _MyPOSTServer.Stop();
        }

        #region Log

        private string _Log = "";
        public string Log { get => _Log; set => Set(ref _Log, value); }
        public void AddLog(string msg)
        {
            Log += "[" + DateTime.Now + "] " + msg + "\r\n";
        }

        #endregion

        #region POST Request

        void OnPOSTRequest(IPOSTCommand cmd)
        {

        }

        #endregion

        #region CloseApplicationCommand

        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            _MyPOSTServer.Stop();
            (RootObject as Window)?.Close();
            //Application.Current.Shutdown();
        }

        #endregion
    }
}


//public string Log { get => controller.Log; set => Set(ref controller.Log, value); }