using System;

namespace StopHandler.Infrastructure.Telegram.Commands
{
    class AddAlertCommand : TelegramBotCommand
    {
        public new static string Name => "addalert";
        public new static Type Type => Type.GetType("StopHandler.Infrastructure.Telegram.Commands.AddAlertCommand");
        public new static string Description => "Подписка на уведомления о запущенной задаче";

        public AddAlertCommand()
        {
            _CommandName = "addalert";
            _Attributes = new TelegramComandAttribute[1] {
                new TelegramComandAttribute("WorkerName", "Введите фамилию и имя сотрудника")
            };

            _Description = "Подписка на уведомления о запущенной задаче";
            _SuccessMessage = "Вы успешно подписались на уведомления о запущенной задаче для сотрудника " + _Attributes[0].Value + "";
            _FailMessage = "Не удалось подписаться на уведомления о запущенной задаче для сотрудника " + _Attributes[0].Value + "";
        }
    }
}
