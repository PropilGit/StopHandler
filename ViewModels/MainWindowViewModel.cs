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

            SendMessageToTelegramChatCommand = new LambdaCommand(
                OnSendMessageToTelegramChatCommandExecuted, 
                CanSendMessageToTelegramChatCommandExecute);

            SendTestMessageToTelegramChatCommand = new LambdaCommand(
                OnSendTestMessageToTelegramChatCommandExecuted,
                CanSendTestMessageToTelegramChatCommandExecute);
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

        private string _Message = "";
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
                    ApplyStopCommand((StopCommand)cmd, -1001473601717);
                    break;
                default:
                    break;
            }
        }

        void ApplyStopCommand(StopCommand stopCmd, long chatId)
        {
            _MyTelegramBot.SendMessageToChat(stopCmd.GenerateMessage(), chatId);
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

        // SendTestMessageToTelegramChatCommand
        public ICommand SendTestMessageToTelegramChatCommand{ get; }
        private bool CanSendTestMessageToTelegramChatCommandExecute(object p) => true;
        private void OnSendTestMessageToTelegramChatCommandExecuted(object p)
        {
            StopCommand stopCmd = new StopCommand(
                12345678, 
                "Иванов Иван", 
                "Однажды, в студеную зимнюю пору, Я из лесу вышел; был сильный мороз. Гляжу, поднимается медленно в гору Лошадка, везущая хворосту воз.",
                DateTime.Now,
                137);
            //if (String.IsNullOrEmpty(Message)) return;
            _MyTelegramBot.SendMessageToChat(stopCmd.GenerateMessage(), -1001473601717);
            //Message = "";
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