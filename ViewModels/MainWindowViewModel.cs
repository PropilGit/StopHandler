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
using System.Collections.ObjectModel;
using System.Windows.Threading;
using StopHandler.Models.Alert;

namespace StopHandler.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel()
        {
            _POSTServer = InitializePOSTServer();
            _TelegramBot = InitializeTelegramBot();
            _Clock = InitializeClock();

            LoadChatsList();
            LoadPFUsersList();

            SendMessageToTelegramChatCommand = new LambdaCommand(OnSendMessageToTelegramChatCommandExecuted, CanSendMessageToTelegramChatCommandExecute);
            SendTestMessageToTelegramChatCommand = new LambdaCommand(OnSendTestMessageToTelegramChatCommandExecuted,CanSendTestMessageToTelegramChatCommandExecute);



            AlertTasks = new ObservableCollection<AlertTask>();
            AlertTasks.Add(new AlertTask("test1", 111111, DateTime.Now, UntilFirstAlert, UntilSecondAlert));
            AlertTasks.Add(new AlertTask("test2", 111112, DateTime.Now, UntilFirstAlert, UntilSecondAlert));
            AlertTasks.Add(new AlertTask("test3", 111113, DateTime.Now, UntilFirstAlert, UntilSecondAlert));
        }

        #region Alert Tab

        #region Clock

        #region Time

        string _Time = "--:--";
        public string Time { get => _Time; set => Set(ref _Time, value); }

        #endregion

        Clock _Clock;

        Clock InitializeClock()
        {
            Clock clock = Clock.GetInstance();
            clock.onTimeUpdate += OnTimeUpdate;
            return clock;
        }

        void OnTimeUpdate(int hour, int min)
        {
            Time = hour.ToString() + ":" + min.ToString();
        }

        #endregion

        #region UntilFirstAlert UntilSecondAlert

        int _UntilFirstAlert = 6;
        public int UntilFirstAlert { 
            get => _UntilFirstAlert; 
            set {
                Set(ref _UntilFirstAlert, value);
                foreach (var at in AlertTasks)
                {
                    at.UpdateAlertDates(UntilFirstAlert, UntilSecondAlert);
                }
            } 
        }

        int _UntilSecondAlert = 8;
        public int UntilSecondAlert { 
            get => _UntilSecondAlert;
            set
            {
                Set(ref _UntilSecondAlert, value);
                foreach (var at in AlertTasks)
                {
                    at.UpdateAlertDates(UntilFirstAlert, UntilSecondAlert);
                }
            } 
        }

        #endregion

        #region AlertTasks 

        private ObservableCollection<AlertTask> _AlertTasks;
        public ObservableCollection<AlertTask> AlertTasks { get => _AlertTasks; set => Set(ref _AlertTasks, value); }



        #endregion

        #region _ 
        #endregion

        #endregion

        #region Telegram Tab

        #region TelegramBot

        TelegramBot _TelegramBot;

        TelegramBot InitializeTelegramBot()
        {
            TelegramBot newTelegramBot = TelegramBot.GetInstance();
            newTelegramBot.onLogUpdate += AddLog;

            return newTelegramBot;
        }

        #endregion

        #region Message

        private string _Message = "";
        public string Message { get => _Message; set => Set(ref _Message, value); }

        #endregion

        #region SendMessageToTelegramChatCommand 

        public ICommand SendMessageToTelegramChatCommand { get; }
        private bool CanSendMessageToTelegramChatCommandExecute(object p) => true;
        private void OnSendMessageToTelegramChatCommandExecuted(object p)
        {
            if (String.IsNullOrEmpty(Message)) return;
            _TelegramBot.SendMessageToChat(Message, _SelectedChat.Id);
            Message = "";
        }

        #endregion

        #region SendTestMessageToTelegramChatCommand 

        public ICommand SendTestMessageToTelegramChatCommand { get; }
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
            _TelegramBot.SendMessageToChat(stopCmd.GenerateMessage(), _SelectedChat.Id);
            //Message = "";
        }

        #endregion

        #endregion

        #region POST Tab

        #region Log

        private string _Log = "";
        public string Log { get => _Log; set => Set(ref _Log, value); }

        public void AddLog(string msg, bool isError = false)
        {
            Log += "[" + DateTime.Now + "] " + msg + "\r\n";
            if (isError)
            {
                _TelegramBot.SendMessageToChat(msg, GetChatId(debugChat.Tag));
            }
        }

        #endregion

        POSTServer _POSTServer;

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
            UpdatePFUserList(cmd.Worker);
            if (cmd.Identifier == StopCommand.identifier) ApplyStopCommand((StopCommand)cmd);
            else ApplyErrorCommand((ErrorCommand)cmd);

        }

        void ApplyStartCommand(StartCommand startCmd)
        {
            AlertTasks.Add(new AlertTask(startCmd.Worker, startCmd.TaskNum, startCmd.Start, UntilFirstAlert, UntilSecondAlert));
        }
        void ApplyStopCommand(StopCommand stopCmd)
        {
            if (stopCmd.Chat.IndexOf(debugChat.Tag) != -1 && stopCmd.Chat.IndexOf("#БФ") != -1) _TelegramBot.SendMessageToChat(stopCmd.GenerateMessage(), GetChatId("#БФ"));
            foreach (var chId in GetChatsIdFromString(stopCmd.Chat))
            {
                _TelegramBot.SendMessageToChat(stopCmd.GenerateMessage(), chId);
            }

        }
        void ApplyErrorCommand(ErrorCommand errCmd)
        {
            _TelegramBot.SendMessageToChat(errCmd.GenerateMessage(), GetChatId(debugChat.Tag));
        }

        #endregion



        #region Chats

        TelegramChat debugChat = new TelegramChat(-1001320796606, "[БФ]Debug", "#DBG");
        string chatsPath = "chats.json";

        private List<TelegramChat> _Chats;
        public List<TelegramChat> Chats { get => _Chats; set => Set(ref _Chats, value); }

        private TelegramChat _SelectedChat;
        public TelegramChat SelectedChat { get => _SelectedChat; set => Set(ref _SelectedChat, value); }

        void LoadChatsList()
        {
            _Chats = JSONConverter.OpenJSONFile<List<TelegramChat>>(chatsPath);
            if (_Chats == null || _Chats.Count == 0)
            {
                _Chats = new List<TelegramChat>();
                _Chats.Add(debugChat);
                AddLog("Ошибка загрузки списка чатов.", true);
            }
            _SelectedChat = _Chats[0];
        }
        List<long> GetChatsIdFromString(string tags = "error")
        {
            List<long> result = new List<long>();
            foreach (var ch in _Chats)
            {
                if (tags.IndexOf(ch.Tag) != -1) result.Add(ch.Id);
            }

            if (result == null || result.Count < 1) result.Add(GetChatId(debugChat.Tag));

            return result;
        }
        long GetChatId(string tag = "error")
        {
            foreach (var ch in _Chats)
            {
                if (tag.IndexOf(ch.Tag) != -1) return ch.Id;
            }
            return GetChatId(debugChat.Tag);
        }

        #endregion

        #region PlanFixUsers

        string PFUsersPath = "PlanFixUsers.json";

        private ObservableCollection<PlanFixUser> _PFUsers;
        public ObservableCollection<PlanFixUser> PFUsers { get => _PFUsers; set => Set(ref _PFUsers, value); }

        void LoadPFUsersList()
        {
            PFUsers = JSONConverter.OpenJSONFile<ObservableCollection<PlanFixUser>>(PFUsersPath);
            if (PFUsers == null || PFUsers.Count == 0)
            {
                PFUsers = new ObservableCollection<PlanFixUser>();
                AddLog("Ошибка загрузки списка пользователей ПланФикса", true);
            }
        }

        bool UpdatePFUserList(string newUserName)
        {
            foreach (var pfu in PFUsers)
            {
                //элемент есть в списке
                if (pfu.Name == newUserName) return true;                
            }

            App.Current.Dispatcher.Invoke((Action)delegate 
            {
                PFUsers.Add(new PlanFixUser(Guid.NewGuid(), newUserName));
            });

            if (JSONConverter.SaveJSONFile<ObservableCollection<PlanFixUser>>(PFUsers, PFUsersPath)) return true;
            else
            {
                AddLog("Не удалось сохранить обновленный список пользователей планФикса", true);
                return true;
            }
        }

        #endregion

        #region Other

        public void CloseApplication()
        {
            _POSTServer.Stop();
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
    _POSTServer.Stop();
    (RootObject as Window)?.Close();
    //Application.Current.Shutdown();
}

#endregion

*/

#region _ 
#endregion