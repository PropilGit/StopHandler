﻿using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.POST
{
    interface IPOSTCommand
    {
        public string[] Tags { get; }
        public string Identifier { get; }
        public string Worker { get; }
        public int TaskNum { get; }
        //public DateTime  { get; }

        public static StopCommand Instantiate(string[] values)
        {
            return null;
        }
        public string ToLog();
    }
}
