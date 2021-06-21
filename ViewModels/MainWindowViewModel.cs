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
using StopHandler.Infrastructure.Files;

namespace StopHandler.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
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

            //SelectedChatChangedCommand = new LambdaCommand(
            //    OnSelectedChatChangedCommandExecuted,
            //    CanSelectedChatChangedCommandExecute);
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

        #region Chats

        long debugChat = -1001320796606;

        private List<TelegramChat> _Chats;
        public List<TelegramChat> Chats { get => _Chats; set => Set(ref _Chats, value); }


        private TelegramChat _SelectedChat;
        public TelegramChat SelectedChat { get => _SelectedChat; set => Set(ref _SelectedChat, value); }

        long GetChatId(string tag = "error")
        {
            foreach (var ch in _Chats)
            {
                if (ch.Tag == tag) return ch.Id;
            }
            return debugChat;
        }

        #endregion

        #region POST

        POSTServer _MyPOSTServer;

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
            if(cmd.Identifier == StopCommand.identifier) ApplyStopCommand((StopCommand)cmd);
            else ApplyErrorCommand((ErrorCommand)cmd);

        }

        void ApplyStopCommand(StopCommand stopCmd)
        {
            if(stopCmd.Chat != "ТЕСТ" && stopCmd.Chat != "БФ") _MyTelegramBot.SendMessageToChat(stopCmd.GenerateMessage(), GetChatId("БФ"));
            _MyTelegramBot.SendMessageToChat(stopCmd.GenerateMessage(), GetChatId(stopCmd.Chat));
        }
        void ApplyErrorCommand(ErrorCommand errCmd)
        {
            _MyTelegramBot.SendMessageToChat(errCmd.GenerateMessage(), GetChatId(errCmd.Chat));
        }

        #endregion

        #region Telegram

        //Переменные
        TelegramBot _MyTelegramBot;
        string jsonPath = "chats.json";

        TelegramBot InitializeTelegramBot()
        {
            TelegramBot newTelegramBot = TelegramBot.GetInstance();
            newTelegramBot.onLogUpdate += AddLog;

            _Chats = JSONConverter.OpenJSONFile<List<TelegramChat>>(jsonPath);
            if (_Chats == null || _Chats.Count == 0) _Chats = new List<TelegramChat>();
            else _SelectedChat = _Chats[0];

            return newTelegramBot;
        }

        // SendMessageToTelegramChatCommand
        public ICommand SendMessageToTelegramChatCommand{ get; }
        private bool CanSendMessageToTelegramChatCommandExecute(object p) => true;
        private void OnSendMessageToTelegramChatCommandExecuted(object p)
        {
            if (String.IsNullOrEmpty(Message)) return;
            _MyTelegramBot.SendMessageToChat(Message, _SelectedChat.Id);
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
                DateTime.Now.AddMinutes(5),
                "TEST");
            //if (String.IsNullOrEmpty(Message)) return;
            _MyTelegramBot.SendMessageToChat(stopCmd.GenerateMessage(), _SelectedChat.Id);
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