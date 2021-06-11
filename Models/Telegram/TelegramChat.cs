using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.Telegram
{
    class TelegramChat
    {
        long _Id;
        public long Id { get => _Id; }

        string _Name;
        public string Name { get => _Name; set => _Name = value; }
        
        string _Tag;
        public string Tag { get => _Tag; set => _Tag = value; }

        public TelegramChat(long id, string name, string tag)
        {
            _Id = id;
            _Name = name;
            _Tag = tag;
        }
    }
}
