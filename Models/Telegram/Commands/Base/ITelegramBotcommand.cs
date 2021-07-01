using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.Telegram.Commands
{
    interface ITelegramBotCommand
    {
        public static string Name { get; }
        public static Type Type { get; }
        public static string Description { get; }
        string SuccessMessage { get; }
        string FailMessage { get; }


        bool SetAttribute(string value);
        string GetAttributeQuestion();
        bool IsAllAttributesFilled { get; }
    }
}
