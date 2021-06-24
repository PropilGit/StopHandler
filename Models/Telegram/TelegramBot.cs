﻿using Newtonsoft.Json;
using StopHandler.Infrastructure.Files;
using System.Collections.Generic;
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

        TelegramBot()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string token = JSONConverter.OpenJSONFile<string>("token.json");
            botClient = new TelegramBotClient(token);

            var me = botClient.GetMeAsync().Result;

            botClient.StartReceiving();
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
#region Получение сообщений
/* ===== MESSAGE ==============================================
async void Bot_CheckMessage(object sender, MessageEventArgs e)
{
    if (e.Message.Text != null)
    {
        if (RememberChat(e.Message.Chat.Id))
        {
            await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: "Вэлком!");
            return;
        }

        if (isActiveRequest)
        {
            onResultReceiving(e.Message.Text);
            isActiveRequest = false;
        }
        //await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: "");
    }
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