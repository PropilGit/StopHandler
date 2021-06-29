using StopHandler.Models.Telegram;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace StopHandler.Models
{
    class PlanFixUser
    {
        public Guid Id { get; }
        public string Name { get; }

        public ObservableCollection<TelegramChat> Subscribers { get; }

        public PlanFixUser(Guid id, string name, ObservableCollection<TelegramChat> subscribers)
        {
            Id = id;
            Name = name;
            Subscribers = subscribers;
        }
    }
}
