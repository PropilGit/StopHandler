using Newtonsoft.Json;
using StopHandler.Infrastructure.Files;
using StopHandler.Models.Telegram.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Message = Telegram.Bot.Types.Message;

//Используется: https://github.com/TelegramBots/telegram.bot

namespace StopHandler.Models.Telegram
{
    class TelegramBot
    {

        #region Singleton

        private static TelegramBot instance;
        public static TelegramBot GetInstance()
        {
            if (instance == null) instance = new TelegramBot();
            return instance;
        }

        #endregion

        static ITelegramBotClient botClient;

        public delegate bool ExecuteCommand(string commandName, long chatId, string shatName, List<string> attributes);
        public event ExecuteCommand onCommandExecute;

        Dictionary<long, ITelegramBotCommand> activeCommands;

        TelegramBot()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string token = JSONConverter.OpenJSONFile<string>("token.json");
            botClient = new TelegramBotClient(token);

            //var me = botClient.GetMeAsync().Result;
            activeCommands = new Dictionary<long, ITelegramBotCommand>();

            botClient.StartReceiving();
            botClient.OnMessage += GetMessageFromchat;
        }
        ~TelegramBot()
        {
            botClient.StopReceiving();
        }

        #region UpdateLog

        public delegate void UpdateLog(string msg, bool isError = false);
        public event UpdateLog onLogUpdate;
        void AddLog(string msg, bool isError = false)
        {
            if (onLogUpdate != null)
            {
                onLogUpdate(msg, isError);
            }
        }

        #endregion

        #region Отправка сообщений
        public async void SendMessageToChat(string msg, long chatId)
        {
            try
            {
                await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: msg,
                parseMode: ParseMode.Markdown,
                disableNotification: true);
            }
            catch (System.Exception ex)
            {
                AddLog("ОШИБКА: " + ex.Message, true);
            }
          
        }
        #endregion

        #region Получение сообщений

        async void GetMessageFromchat(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                long chId = e.Message.Chat.Id;
                string msg = e.Message.Text;

                ITelegramBotCommand command;
                if (activeCommands.Count == 0) command = null;
                else command = activeCommands.Where(ac => ac.Key == chId).SingleOrDefault().Value;

                if (command != null) command.SetAttribute(msg);
                else
                {
                    command = TelegramBotCommand.InstantiateCommand(msg);
                    if (command != null) activeCommands.Add(chId, command);
                }

                if (command != null)
                {
                    string question = command.GetAttributeQuestion();
                    if (!string.IsNullOrEmpty(question)) SendMessageToChat(question, chId);

                    if (command.IsAllAttributesFilled)
                    {
                        //Execute
                        bool result = false;
                        if(onCommandExecute != null) result = onCommandExecute(command.CommandName, chId, e.Message.Chat.Username, command.Attributes);

                        if (result) SendMessageToChat(command.SuccessMessage + "\n🥳", chId);
                        else SendMessageToChat(command.FailMessage, chId);

                        activeCommands.Remove(chId);
                    }
                }
                else SendMessageToChat("Ну и зачем вы написали: *\"" + msg + "\"*?", chId);

                //onMessage(e.Message.Chat.Id, e.Message.Text);
            }
        }

        #endregion

        #region Commands



        #endregion
    }
}

/*

        public delegate void GetResultFromUser(string result);
        public event GetResultFromUser onResultReceiving;
        public bool isActiveRequest = false;
*/
/*
        public async void Bot_SendDocument(Captcha captcha)
        { 
            using (Stream fs = new MemoryStream(captcha.image))
            {
                InputOnlineFile inputOnlineFile = new InputOnlineFile(fs, captcha.name + ".jpg");

                if (chats == null || chats.Count == 0) return;

                Telegram.Bot.Types.Message message = await botClient.SendDocumentAsync(chats[0], inputOnlineFile);
                Thread.Sleep(5000);

                string file_id = message.Document.FileId;
                InputOnlineFile inputOnlineFileId = new InputOnlineFile(file_id);

                for (int i = 1; i < chats.Count; i++)
                {
                    await botClient.SendDocumentAsync(chats[i], inputOnlineFileId);
                }
            }
        }
*/
/*
        public async void Bot_SendAllImageFromHTML(string path)
        {
            foreach (var ch in chats)
            {
                Message message = await botClient.SendPhotoAsync(
                     chatId: ch.Id,
                     photo: path,
                     caption: "<b>bbbbb</b>. <i>iiiii</i>: <a href=\"https://ya.ru\">link</a>",
                     parseMode: ParseMode.Html
                );
            }
        }
*/
#region Операции с чатами
    /*
bool RememberChat(long id)
{
    if (chats == null || chats.Count == 0) return false;
    foreach (var ch in chats)
    {
        if (ch.Id == id) return false;

        chats.Add(ch);
        JSONConverter.SaveJSONFile<List<TelegramChat>>(chats, jsonPath);
        return true;
    }
    return false;
}
*/
#endregion

#region CAPTCHA
// ===== CAPTCHA ==============================================
/*public async void Bot_SendCaptchaAsync(Captcha captcha)
{
    Stream fs = new MemoryStream(captcha.image);
    InputOnlineFile inputOnlineFile = new InputOnlineFile(fs, captcha.name + ".jpg");

    if (chats == null || chats.Count == 0) return;

    //Telegram.Bot.Types.Message message = await Bot_SendImageAsync(inputOnlineFile);
    //string file_id = message.Photo.
    //InputOnlineFile inputOnlineFileId = new InputOnlineFile(file_id);

    foreach (var ch in chats)
    {
        await Bot_SendImageAsync(inputOnlineFile, ch.Id);
    }
    isActiveRequest = true;
}

public async Task<Message> Bot_SendImageAsync(InputOnlineFile inputOnlineFile, long chat_id)
{
    return await botClient.SendPhotoAsync(
         chatId: chat_id,
         photo: inputOnlineFile,
         caption: "<b>CAPTCHA</b>",//. <i></i>: <a href=\"https://ya.ru\">link</a>",
         parseMode: ParseMode.Html
    );
}
*/

#endregion