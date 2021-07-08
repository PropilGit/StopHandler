using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Infrastructure.Telegram.Commands
{
    class RemoveAlertCommand : TelegramBotCommand
    {
        public new static string Name => "remalert";
        public new static Type Type => Type.GetType("StopHandler.Infrastructure.Telegram.Commands.RemoveAlertCommand");
        public new static string Description => "Отписка от уведомлений о запущенной задаче";

        public RemoveAlertCommand()
        {
            _CommandName = "remalert";
            _Attributes = new TelegramComandAttribute[1] {
                new TelegramComandAttribute("WorkerName", "Введите фамилию и имя сотрудника")
            };

            _Description = "Отписка от уведомлений о запущенной задаче";
            _SuccessMessage = "Вы успешно отписались от уведомлений о запущенной задаче для сотрудника " + _Attributes[0].Value + "";
            _FailMessage = "Не удалось отписаться от уведомлений о запущенной задаче для сотрудника " + _Attributes[0].Value + "";
        }
    }
}
