using StopHandler.Models.Telegram;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace StopHandler.Models
{
    class PlanFixUser
    {
        public Guid Id { get; }
        public string Name { get; }

        public ObservableCollection<TelegramChat> Subscribers { get; set; }

        public PlanFixUser(Guid id, string name, ObservableCollection<TelegramChat> subscribers)
        {
            Id = id;
            Name = name;
            Subscribers = subscribers;
        }

        public bool FindSubscriber(long id)
        {
            if (Subscribers == null || Subscribers.Count == 0) return false;

            var sub = Subscribers.Where(s => s.Id == id).SingleOrDefault();
            return sub != null;
        }
        public bool AddSubscriber(TelegramChat chat)
        {
            if (Subscribers == null) Subscribers = new ObservableCollection<TelegramChat>();
            else if (FindSubscriber(chat.Id)) return false;

            Subscribers.Add(chat);
            return true;
        }
        public bool RemoveSubscriber(long id)
        {
            if (Subscribers == null) return false;
            else if (FindSubscriber(id))
                return Subscribers.Remove(Subscribers.Where(x => x.Id == id).Single());
            else return false;
        }
    }
}
