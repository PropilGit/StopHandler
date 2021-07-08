using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.POST
{
    class StartCommand: IPOSTCommand
    {
        /*
        CMD#START#CMD
        TNUM#121574#TNUM
        TNAM#НАЗВАНИЕ ЗАДАЧИ#TNAM
        WRK#Хлупичев Владимир#WRK
        STR#1624776949#STR
        */

        public string[] Tags { get => tags; }
        public static string[] tags = new string[5] { "CMD", "TNUM", "TNAM", "WRK", "STR"};
        public string Identifier { get => identifier; }
        public static string identifier = "START";

        public int TaskNum { get; private set; }
        public string TaskName { get; private set; }
        public string Worker { get; private set; }
        public DateTime Start { get; private set; }

        public StartCommand(int taskNum, string taskName, string worker, DateTime start)
        {
            TaskNum = taskNum;
            TaskName = taskName;
            Worker = worker;
            Start = start;
        }
        public static StartCommand Instantiate(string[] values)
        {
            if (values.Length == tags.Length)
            {
                try
                {
                    DateTime start = POSTCommand.UnixTimeToLocalTime(long.Parse(values[4]));
                    return new StartCommand(Int32.Parse(values[1]), values[2], values[3], start);
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
            + tags[2] + ": " + TaskName + "\n"
            + tags[3] + ": " + Worker + "\n"
            + tags[4] + ": " + Start.ToString();
        }
    }
}
