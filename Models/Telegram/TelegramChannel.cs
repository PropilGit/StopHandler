using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.Telegram
{
    class TelegramChannel
    {
        long _Id;
        public long Id { get => _Id; }

        string _Name;
        public string Name { get => _Name; set => _Name = value; }


        public TelegramChannel(long id, string name)
        {
            _Id = id;
            _Name = name;
        }
    }
}
