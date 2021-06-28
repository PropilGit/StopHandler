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
using System.Linq;

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

            //Telegram
            SendMessageToTelegramChatCommand = new LambdaCommand(OnSendMessageToTelegramChatCommandExecuted, CanSendMessageToTelegramChatCommandExecute);
            SendTestMessageToTelegramChatCommand = new LambdaCommand(OnSendTestMessageToTelegramChatCommandExecuted,CanSendTestMessageToTelegramChatCommandExecute);

            //Alert setup
            ReduceUntilFirstAlertCommand = new LambdaCommand(OnReduceUntilFirstAlertCommandExecuted, CanReduceUntilFirstAlertCommandExecute);
            IncreaseUntilFirstAlertCommand = new LambdaCommand(OnIncreaseUntilFirstAlertCommandExecuted, CanIncreaseUntilFirstAlertCommandExecute);
            ReduceUntilSecondAlertCommand = new LambdaCommand(OnReduceUntilSecondAlertCommandExecuted, CanReduceUntilSecondAlertCommandExecute);
            IncreaseUntilSecondAlertCommand = new LambdaCommand(OnIncreaseUntilSecondAlertCommandExecuted, CanIncreaseUntilSecondAlertCommandExecute);            

            //AlertMenu setup
            ShowAlertMenuCommand = new LambdaCommand(OnShowAlertMenuCommandExecuted, CanShowAlertMenuCommandExecute);
            ApplyAlertSetupCommand = new LambdaCommand(OnApplyAlertSetupCommandExecuted, CanApplyAlertSetupCommandExecute);
            DenyAlertSetupCommand = new LambdaCommand(OnDenyAlertSetupCommandExecuted, CanDenyAlertSetupCommandExecute);

            _UntilFirstAlert = 1;
            _UntilSecondAlert = 2;
            IsVisibleAlertMenu = Visibility.Hidden;
            AlertTasks = new ObservableCollection<AlertTask>();


            //=======================================================================================
            AlertTasks.Add(new AlertTask("test1", 111111, DateTime.Now, UntilFirstAlert, UntilSecondAlert));
            AlertTasks.Add(new AlertTask("test2", 111112, DateTime.Now, UntilFirstAlert, UntilSecondAlert));
            AlertTasks.Add(new AlertTask("test3", 111113, DateTime.Now, UntilFirstAlert, UntilSecondAlert));
            //
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
            clock.onTimeUpdate += CheckAlertTasks;
            return clock;
        }
        void OnTimeUpdate()
        {
            string strH = DateTime.Now.Hour.ToString();
            if (strH.Length == 1) strH = "0" + strH;

            string strM = DateTime.Now.Minute.ToString();
            if (strM.Length == 1) strM = "0" + strM;

            Time = strH + ":" + strM;
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

        #region DisplayedHoursUntilFirstAlert DisplayedHoursUntilSecondAlert

        int _DisplayedHoursUntilFirstAlert;
        public int DisplayedHoursUntilFirstAlert { get => _DisplayedHoursUntilFirstAlert; set => Set(ref _DisplayedHoursUntilFirstAlert, value); }

        int _DisplayedHoursUntilSecondAlert;
        public int DisplayedHoursUntilSecondAlert { get => _DisplayedHoursUntilSecondAlert; set => Set(ref _DisplayedHoursUntilSecondAlert, value); }

        #endregion

        #region AlertTasks 

        private ObservableCollection<AlertTask> _AlertTasks;
        public ObservableCollection<AlertTask> AlertTasks { get => _AlertTasks; set => Set(ref _AlertTasks, value); }

        void CheckAlertTasks()
        {
            if (AlertTasks == null || AlertTasks.Count == 0) return;

            foreach (var at in AlertTasks)
            {
                if (at.CheckFirstAlert(DateTime.Now) || at.CheckSecondAlert(DateTime.Now)) SendAlert(at);
            }
        }

        void SendAlert(AlertTask alertTask)
        {
            _POSTServer.SendPOSTAsync("https://bankrotforum.planfix.ru/webhook/json/timerAlert", alertTask.GenerateStringForPlanFix());
            _TelegramBot.SendMessageToChat(alertTask.GenerateStringForPlanFix(), GetChatId("#DBG"));

            if (alertTask.IsSecondAlertSend)
            {
                AlertTasks.Remove(AlertTasks.Where(i => i == alertTask).Single());
            }
        }

        #endregion

        #region IsVisibleAlertMenu 

        private Visibility _IsVisibleAlertMenu;
        public Visibility IsVisibleAlertMenu { get => _IsVisibleAlertMenu; set => Set(ref _IsVisibleAlertMenu, value); }

        #endregion

        #region ReduceUntilFirstAlertCommand 

        public ICommand ReduceUntilFirstAlertCommand { get; }
        private bool CanReduceUntilFirstAlertCommandExecute(object p) => true;
        private void OnReduceUntilFirstAlertCommandExecuted(object p)
        {
            if (DisplayedHoursUntilFirstAlert == 1) return;
            DisplayedHoursUntilFirstAlert--;
        }

        #endregion

        #region IncreaseUntilFirstAlertCommand 

        public ICommand IncreaseUntilFirstAlertCommand { get; }
        private bool CanIncreaseUntilFirstAlertCommandExecute(object p) => true;
        private void OnIncreaseUntilFirstAlertCommandExecuted(object p)
        {
            if (DisplayedHoursUntilFirstAlert == DisplayedHoursUntilSecondAlert - 1) return;
            DisplayedHoursUntilFirstAlert++;
        }

        #endregion

        #region ReduceUntilSecondAlertCommand 

        public ICommand ReduceUntilSecondAlertCommand { get; }
        private bool CanReduceUntilSecondAlertCommandExecute(object p) => true;
        private void OnReduceUntilSecondAlertCommandExecuted(object p)
        {
            if (DisplayedHoursUntilSecondAlert == DisplayedHoursUntilFirstAlert + 1 ) return;
            DisplayedHoursUntilSecondAlert--;
        }

        #endregion

        #region IncreaseUntilSecondAlertCommand 

        public ICommand IncreaseUntilSecondAlertCommand { get; }
        private bool CanIncreaseUntilSecondAlertCommandExecute(object p) => true;
        private void OnIncreaseUntilSecondAlertCommandExecuted(object p)
        {
            if (DisplayedHoursUntilSecondAlert == 10) return;
            DisplayedHoursUntilSecondAlert++;
        }

        #endregion

        #region ShowAlertMenuCommand

        public ICommand ShowAlertMenuCommand { get; }
        private bool CanShowAlertMenuCommandExecute(object p) => true;
        private void OnShowAlertMenuCommandExecuted(object p)
        {
            DisplayedHoursUntilFirstAlert = UntilFirstAlert;
            DisplayedHoursUntilSecondAlert = UntilSecondAlert;

            IsVisibleAlertMenu = Visibility.Visible;
        }

        #endregion

        #region ApplyAlertSetupCommand 

        public ICommand ApplyAlertSetupCommand  { get; }
        private bool CanApplyAlertSetupCommandExecute(object p) => true;
        private void OnApplyAlertSetupCommandExecuted(object p)
        {
            UntilFirstAlert = DisplayedHoursUntilFirstAlert;
            UntilSecondAlert = DisplayedHoursUntilSecondAlert;

            foreach (var at in AlertTasks)
            {
                at.UpdateAlertDates(UntilFirstAlert, UntilSecondAlert);
            }

            IsVisibleAlertMenu = Visibility.Hidden;
        }

        #endregion

        #region DenyAlertSetupCommand 

        public ICommand DenyAlertSetupCommand { get; }
        private bool CanDenyAlertSetupCommandExecute(object p) => true;
        private void OnDenyAlertSetupCommandExecuted(object p)
        {
            IsVisibleAlertMenu = Visibility.Hidden;
        }

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
            if (cmd.Identifier == StopCommand.identifier) ApplyCommand((StopCommand)cmd);
            else if (cmd.Identifier == StartCommand.identifier) ApplyCommand((StartCommand)cmd);
            else ApplyCommand((ErrorCommand)cmd);

        }

        void ApplyCommand(StartCommand startCmd)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                AlertTasks.Add(new AlertTask(startCmd.Worker, startCmd.TaskNum, startCmd.Start, UntilFirstAlert, UntilSecondAlert));
            });    
        }
        void ApplyCommand(StopCommand stopCmd)
        {
            if (stopCmd.Chat.IndexOf(debugChat.Tag) != -1 && stopCmd.Chat.IndexOf("#БФ") != -1) _TelegramBot.SendMessageToChat(stopCmd.GenerateMessage(), GetChatId("#БФ"));
            foreach (var chId in GetChatsIdFromString(stopCmd.Chat))
            {
                _TelegramBot.SendMessageToChat(stopCmd.GenerateMessage(), chId);
            }

            //AlertTasks.RemoveAt(AlertTasks.Where(at => at.TaskNum == stopCmd.TaskNum).First());
            AlertTasks.Remove(AlertTasks.Where(i => i.TaskNum == stopCmd.TaskNum).Single());

        }
        void ApplyCommand(ErrorCommand errCmd)
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