using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using StopHandler.ViewModels.Base;
using StopHandler.Models.POST;
using StopHandler.Models;
using StopHandler.Models.Telegram;
using StopHandler.Infrastructure.Commands;
using StopHandler.Infrastructure.Files;
using System.Collections.ObjectModel;
using StopHandler.Models.Alert;
using System.Linq;

namespace StopHandler.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel()
        {
            _WebHook = JSONConverter.OpenJSONFile<string>("webhook.json");
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

            _UntilFirstAlert = 6;
            _UntilSecondAlert = 8;
            VisibilityAlertMenu = Visibility.Hidden;
            AlertTasks = new ObservableCollection<AlertTask>();

            /*
            ObservableCollection<TelegramChat> testTGChats = new ObservableCollection<TelegramChat>(){
                new TelegramChat(11111111, "test chat 1", "TCH1"),
                new TelegramChat(11111111, "test chat 1", "TCH1"),
                new TelegramChat(11111111, "test chat 1", "TCH1")};

            PFUsers.Add(new PlanFixUser(Guid.NewGuid(), "ИМЯ1", testTGChats));
            PFUsers.Add(new PlanFixUser(Guid.NewGuid(), "ИМЯ2", testTGChats));
            PFUsers.Add(new PlanFixUser(Guid.NewGuid(), "ИМЯ2", testTGChats));

            JSONConverter.SaveJSONFile<ObservableCollection<PlanFixUser>>(PFUsers, PFUsersPath);
            */
        }

        #region Alert Tab

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

        public ObservableCollection<AlertTask> AlertTasks { get; }

        void CheckAlertTasks()
        {
            if (AlertTasks == null || AlertTasks.Count == 0) return;

            foreach (var at in AlertTasks)
            {
                if (at.CheckFirstAlert(DateTime.Now) || at.CheckSecondAlert(DateTime.Now))
                {
                    SendAlert(at);
                    break;
                }
            }
        }

        void SendAlert(AlertTask alertTask)
        {
            try
            {
                _POSTServer.SendPOSTAsync(_WebHook, alertTask.GenerateStringForPlanFix());
            }
            catch (Exception ex)
            {
                AddLog("Ошибка отправки уведомления в ПланФикс: " + ex.Message, true);
                throw;
            }
            

            if (alertTask.IsSecondAlertSend) AlertTasks.Remove(alertTask);
        }

        #endregion

        #region VisibilityAlertMenu 

        //видимость меню
        private Visibility _VisibilityAlertMenu;
        public Visibility VisibilityAlertMenu 
        { 
            get => _VisibilityAlertMenu; 
            set
            {
                Set(ref _VisibilityAlertMenu, value);

                //Скрываем элементы, которые должны быть скрыты при открытии меню
                if (value == Visibility.Visible) ReverseVisibilityAlertMenu = Visibility.Hidden;
                else ReverseVisibilityAlertMenu = Visibility.Visible;
            } 
        }

        #endregion

        #region ReverseVisibilityAlertMenu 

        // Элементы которые должны скрываться при открытии меню
        private Visibility _ReverseVisibilityAlertMenu;
        public Visibility ReverseVisibilityAlertMenu { get => _ReverseVisibilityAlertMenu; set => Set(ref _ReverseVisibilityAlertMenu, value); }

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

            VisibilityAlertMenu = Visibility.Visible;
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

            VisibilityAlertMenu = Visibility.Hidden;
        }

        #endregion

        #region DenyAlertSetupCommand 

        public ICommand DenyAlertSetupCommand { get; }
        private bool CanDenyAlertSetupCommandExecute(object p) => true;
        private void OnDenyAlertSetupCommandExecuted(object p)
        {
            VisibilityAlertMenu = Visibility.Hidden;
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

        int maxLogValue = 150;
        int logCounter = 0;

        private string _Log = "";
        public string Log { get => _Log; set => Set(ref _Log, value); }

        public void AddLog(string msg, bool isError = false)
        {
            Log += "[" + DateTime.Now + "] " + msg + "\r\n";
            
            //если есть пометка об ошибке - отправляем в Дебажный чат
            if (isError)
            {
                _TelegramBot.SendMessageToChat(msg, GetChatId(debugChat.Tag));
            }

            //Очищаем лог
            logCounter++;
            if (logCounter >= maxLogValue)
            {
                logCounter = 0;
                Log = "";
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
            UpdatePFUsersList(cmd.Worker);

            if (cmd.Identifier == StopCommand.identifier) ApplyCommand((StopCommand)cmd);
            else if (cmd.Identifier == StartCommand.identifier) ApplyCommand((StartCommand)cmd);
            else ApplyCommand((ErrorCommand)cmd);
        }

        void ApplyCommand(StartCommand startCmd)
        {
            try
            {
                if(AlertTasks == null)
                {
                    AddLog("Список команд не инициализирован.\n" + startCmd.ToLog(), true);
                    return;
                }
                if (AlertTasks.Count > 0)
                {
                    var at = AlertTasks.Where(i => i.TaskNum == startCmd.TaskNum).SingleOrDefault();
                    if (at != null)
                    {
                        AddLog("Дублированный POST-запрос с командой START.\n" + startCmd.ToLog(), true);
                        return;
                    }
                }
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    AlertTasks.Add(new AlertTask(startCmd.Worker, startCmd.TaskNum, startCmd.Start, UntilFirstAlert, UntilSecondAlert));
                });
            }
            catch (Exception ex)
            {
                AddLog("Непредвиденная ошибка при обработке входящего POST-запроса с командой START.\n" + ex.Message + "\n" + startCmd.ToLog(), true);
            }
        }
        void ApplyCommand(StopCommand stopCmd)
        {
            try
            {
                if (AlertTasks == null)
                {
                    AddLog("Список команд не инициализирован.\n" + stopCmd.ToLog(), true);
                    return;
                }
                if (AlertTasks.Count == 0)
                {
                    AddLog("POST-запрос с командой STOP, список задач пуст.\n" + stopCmd.ToLog(), true);
                    return;
                }
                else
                {
                    var at = AlertTasks.Where(i => i.TaskNum == stopCmd.TaskNum).SingleOrDefault();
                    if (at == null)
                    {
                        AddLog("POST-запрос с командой STOP, в списке отсутствует соответствующая задача.\n" + stopCmd.ToLog(), true);
                        return;
                    }
                    else
                    {
                        App.Current.Dispatcher.Invoke((Action)delegate {
                            AlertTasks.Remove(at);
                        });
                        if (stopCmd.Chat.IndexOf(debugChat.Tag) != -1 && stopCmd.Chat.IndexOf("#БФ") != -1) _TelegramBot.SendMessageToChat(stopCmd.GenerateMessage(), GetChatId("#БФ"));
                        foreach (var chId in GetChatsIdFromString(stopCmd.Chat))
                        {
                            _TelegramBot.SendMessageToChat(stopCmd.GenerateMessage(), chId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog("Непредвиденная ошибка при обработке входящего POST-запроса с командой STOP.\n" + ex.Message + "\n" + stopCmd.ToLog(), true);
            }        
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

        public ObservableCollection<PlanFixUser> PFUsers { get; private set; }

        void LoadPFUsersList()
        {
            PFUsers = JSONConverter.OpenJSONFile<ObservableCollection<PlanFixUser>>(PFUsersPath);
            if (PFUsers == null || PFUsers.Count == 0)
            {
                PFUsers = new ObservableCollection<PlanFixUser>();
                AddLog("Ошибка загрузки списка пользователей ПланФикса", true);
            }
        }

        bool UpdatePFUsersList(string newUserName)
        {
            foreach (var pfu in PFUsers)
            {
                //элемент есть в списке
                if (pfu.Name == newUserName) return true;                
            }

            App.Current.Dispatcher.Invoke((Action)delegate 
            {
                PFUsers.Add(new PlanFixUser(Guid.NewGuid(), newUserName, new ObservableCollection<TelegramChat>()));
            });

            if (JSONConverter.SaveJSONFile<ObservableCollection<PlanFixUser>>(PFUsers, PFUsersPath)) return true;
            else
            {
                AddLog("Не удалось сохранить обновленный список пользователей планФикса", true);
                return true;
            }
        }

        bool UpdatePFUsersList(string pfUserName, TelegramChat subsriber)
        {

            var pfUser = PFUsers.Where(pfu => pfu.Name == pfUserName).SingleOrDefault();
            if (pfUser == null)
            {
                AddLog("Попытка подписаться на [" + pfUserName + "], которого нет в списке.", true);
            }

            var sub = pfUser.Subscribers.Where(s => s.Id == subsriber.Id).SingleOrDefault();
            if (sub != null)
            {
                AddLog("Повторная попытка подписаться на [" + pfUserName + "].", true);
            }

            App.Current.Dispatcher.Invoke((Action)delegate
            {
                pfUser.Subscribers.Add(subsriber);
            });

            if (JSONConverter.SaveJSONFile<ObservableCollection<PlanFixUser>>(PFUsers, PFUsersPath)) return true;
            else
            {
                AddLog("Не удалось сохранить обновленный список подписок на пользователя планФикса [" + pfUserName + "]", true);
                return false;
            }
        }

        #endregion

        #region _WebHook

        string webhookPath = "webhook.json";

        private string _WebHook;

        #endregion

        #region Other

        public void CloseApplication()
        {
            _POSTServer.Stop();
        }

        #endregion

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

            OnTimeUpdate();
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