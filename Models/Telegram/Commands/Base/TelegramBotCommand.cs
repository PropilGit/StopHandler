using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;

namespace StopHandler.Models.Telegram.Commands
{
    abstract class TelegramBotCommand : ITelegramBotCommand
    {
        #region Static

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

        #endregion

        public static string Name;
        public static Type Type;

        protected string _CommandName;
        public string CommandName { get => _CommandName; }

        protected string _Description;
        public string Description { get => _Description; }

        int _CurrentAttributeIndex;
        protected TelegramComandAttribute[] _Attributes { get; set; }
        public List<string> Attributes { get => _Attributes.Select(a => a.Value).ToList(); }

        public bool SetAttribute(string value)
        {
            if (_CurrentAttributeIndex < 0 || _CurrentAttributeIndex >= _Attributes.Length) return false;
            else
            {
                _Attributes[_CurrentAttributeIndex].Value = value;
                _CurrentAttributeIndex++;
                return true;
            }
        }
        public string GetAttributeQuestion()
        {
            if (_CurrentAttributeIndex < 0 || _CurrentAttributeIndex >= _Attributes.Length) return null;
            return _Attributes[_CurrentAttributeIndex].Question;
        }
        public bool IsAllAttributesFilled
        {
            get
            {
                if (_CurrentAttributeIndex == _Attributes.Length) return true;
                else return false;
            }
        }


        protected string _SuccessMessage;
        public string SuccessMessage { get => _SuccessMessage; }

        protected string _FailMessage;
        public string FailMessage { get => _FailMessage; }
    }
    struct TelegramComandAttribute
    {
        public TelegramComandAttribute(string name, string question)
        {
            Name = name;
            Question = question;
            Value = null;
        }

        public string Name { get; }
        public string Question { get; }
        public string Value { get; set; }
    }
}
