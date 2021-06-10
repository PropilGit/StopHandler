using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using StopHandler.ViewModels.Base;
using StopHandler.Models.POST;
using StopHandler.Models;
using StopHandler.Models.Telegram;
using StopHandler.Infrastructure.Commands;

namespace StopHandler.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        POSTServer _MyPOSTServer;

        public MainWindowViewModel()
        {
            _MyPOSTServer = InitializePOSTServer();
            _MyTelegramBot = InitializeTelegramBot();

            SendMessageToTelegramChatCommand = new LambdaCommand(OnSendMessageToTelegramChatCommandExecuted, CanSendMessageToTelegramChatCommandExecute);
        }

        #region Log

        private string _Log = "";
        public string Log { get => _Log; set => Set(ref _Log, value); }

        public void AddLog(string msg)
        {
            Log += "[" + DateTime.Now + "] " + msg + "\r\n";
        }

        #endregion

        #region Message

        private string _Message = "009";
        public string Message { get => _Message; set => Set(ref _Message, value); }

        #endregion

        #region POST

        POSTServer InitializePOSTServer()
        {
            POSTServer newPOSTServer = POSTServer.GetInstance();
            newPOSTServer.onLogUpdate += AddLog;
            newPOSTServer.onPOSTRequest += OnPOSTRequest;

            newPOSTServer.Start();

            return newPOSTServer;
        }

        void OnPOSTRequest(IPOSTCommand cmd)
        {
            switch (cmd.Identifier)
            {
                case "STOP":
                    ApplyStopCommand(cmd);
                default:
            }
        }

        #endregion

        #region Telegram

        TelegramBot _MyTelegramBot;

        TelegramBot InitializeTelegramBot()
        {
            TelegramBot newTelegramBot = TelegramBot.GetInstance();
            newTelegramBot.onLogUpdate += AddLog;

            return newTelegramBot;
        }

        // SendMessageToTelegramChatCommand
        public ICommand SendMessageToTelegramChatCommand{ get; }
        private bool CanSendMessageToTelegramChatCommandExecute(object p) => true;
        private void OnSendMessageToTelegramChatCommandExecuted(object p)
        {
            if (String.IsNullOrEmpty(Message)) return;
            _MyTelegramBot.SendMessageToChat(Message, -1001473601717);
            Message = "";
        }

        #endregion

        #region Other

        public void CloseApplication()
        {
            _MyPOSTServer.Stop();
        }

        #endregion
    }
}


//public string Log { get => controller.Log; set => Set(ref controller.Log, value); }
/*
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






*/