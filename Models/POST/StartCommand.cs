﻿using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.POST
{
    class StartCommand: IPOSTCommand
    {
        /*
        CMD#START#CMD
        TASK#121574#TASK
        WRK#Иванов Иван#WRK
        STR#1624776949#STR
        */

        public string[] Tags { get => tags; }
        public static string[] tags = new string[4] { "CMD", "TASK", "WRK", "STR"};
        public string Identifier { get => identifier; }
        public static string identifier = "START";

        public int TaskNum { get; private set; }
        public string Worker { get; private set; }
        public DateTime Start { get; private set; }

        public StartCommand(int taskNum, string worker, DateTime start)
        {
            TaskNum = taskNum;
            Worker = worker;
            Start = start;
        }
        public static StartCommand Instantiate(string[] values)
        {
            if (values.Length == tags.Length)
            {
                try
                {
                    DateTime start = POSTCommand.UnixTimeToLocalTime(long.Parse(values[3]));
                    return new StartCommand(Int32.Parse(values[1]), values[2], start);
                }
                catch (Exception)
                {

                }
            }
            return null;
        }
        public string ToLog()
        {
            return "POST-запрос:\n"
            + tags[0] + ": " + identifier + "\n"
            + tags[1] + ": " + TaskNum.ToString() + "\n"
            + tags[2] + ": " + Worker + "\n"
            + tags[3] + ": " + Start.ToString();
        }
    }
}
