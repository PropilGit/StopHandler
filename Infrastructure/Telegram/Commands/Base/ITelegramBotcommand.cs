using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Infrastructure.Telegram.Commands
{
    internal interface ITelegramBotCommand
    {
        public static string Name;
        public static Type Type;

        public string CommandName { get; }
        public string Description { get; }
        string SuccessMessage { get; }
        string FailMessage { get; }

        public List<string> Attributes { get; }
        bool SetAttribute(string value);
        string GetAttributeQuestion();
        bool IsAllAttributesFilled { get; }
    }
}
