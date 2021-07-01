using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.Telegram.Commands
{
    class AddAlertCommand : ITelegramBotCommand
    {
        public static string Name => "addalert";

        public static Type Type => Type.GetType("StopHandler.Models.Telegram.Commands.AddAlertCommand");

        public static string Description => "Подписка на уведомления о запущенной задаче";

        public string SuccessMessage => "Вы успешно подписались на уведомления о запущенной задаче для сотрудника [" + WorkerName + "]";

        public string FailMessage => "Не удалось подписаться на уведомления о запущенной задаче для сотрудника [" + WorkerName + "] \n" + _FailReason;

        string _FailReason = "";


        new TelegramComandAttribute[] Attributes = new TelegramComandAttribute[1] {
            new TelegramComandAttribute("WorkerName", "Введите фамилию и имя сотрудника")
        };

        int _CurrentAttributeIndex;

        public bool SetAttribute(string value)
        {
            if (_CurrentAttributeIndex == 0)
            {
                WorkerName = value;
                _CurrentAttributeIndex++;
                return true;
            }
            else return false;
        }

        public string GetAttributeQuestion()
        {
            if (_CurrentAttributeIndex >= Attributes.Length) return null;
            return Attributes[_CurrentAttributeIndex].Question;
        }

        public bool IsAllAttributesFilled {
            get {
                if (_CurrentAttributeIndex == Attributes.Length) return true;
                else return false;
            }
        }

        public string WorkerName { get; private set; }
    }
}
