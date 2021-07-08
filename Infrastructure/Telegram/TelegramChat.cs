using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Infrastructure.Telegram
{
    class TelegramChat
    {
        public long Id { get; private set; }

        public string Name { get; private set; }
        
        public string Tag { get; private set; }

        public TelegramChat(long id, string name, string tag)
        {
            Id = id;
            Name = name;
            Tag = tag;
        }
    }
}
