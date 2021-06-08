using System;
using System.Collections.Generic;
using System.Text;
using StopHandler.Models.POST;

namespace StopHandler.Models
{
    class Controller
    {
        #region Singleton

        private static Controller instance;
        public static Controller GetInstance(int port = 48654)
        {
            if (instance == null) instance = new Controller();
            return instance;
        }

        #endregion

        POSTServer _MyPOSTServer;

        public Controller()
        {



            _MyPOSTServer = POSTServer.GetInstance();
            _MyPOSTServer.onLogUpdate += AddLog;
            //_MyPOSTServer.onPOSTRequest

            _MyPOSTServer.Start();
        }

        #region Log

        private string _Log = "";
        public string Log { get => _Log; set => _Log = value; }
        public void AddLog(string msg)
        {
            Log += "[" + DateTime.Now + "] " + msg + "\r\n";
        }

        #endregion
    }
}
