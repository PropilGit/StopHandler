using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;

namespace StopHandler.Models.Telegram.Commands
{
    abstract class TelegramBotCommand
    {
        
        public static Dictionary<string, Type> Commands = new Dictionary<string, Type>()
        {
            {AddAlertCommand.Name, AddAlertCommand.Type}
        };
        
        public static ITelegramBotCommand InstantiateCommand(string msg)
        {

            foreach (var com in Commands)
            {
                // если найдено имя комманды с впередистоящим слешем, стоящее вначале строки
                if (msg.IndexOf("/" + com.Key) == 0) 
                {
                    return (ITelegramBotCommand)Activator.CreateInstance(com.Value);
                }
            }
            return null;
        }

    }
    struct TelegramComandAttribute
    {
        public TelegramComandAttribute(string name, string question)
        {
            Name = name;
            Question = question;
        }

        public string Name { get; }
        public string Question { get; }
    }
}
