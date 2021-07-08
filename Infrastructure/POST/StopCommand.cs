using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.POST
{
    class StopCommand : IPOSTCommand
    {
        /*
        CMD#STOP#CMD
        TNUM#121574#TNUM
        TNAM#НАЗВАНИЕ ЗАДАЧИ#TNAM
        WRK#Хлупичев Владимир#WRK
        REP#Однажды, в студеную зимнюю пору, Я из лесу вышел; был сильный мороз.#REP
        STR#1624776949#STR
        STP#1624777449#STP
        CHAT#ТЕСТ#CHAT
        */
        public string[] Tags { get => tags; }
        public static readonly string[] tags = new string[8] {"CMD", "TNUM", "TNAM", "WRK", "REP", "STR", "STP", "CHAT"};
        public string Identifier { get => identifier; }
        public static readonly string identifier = "STOP";
        public int TaskNum { get; private set; }
        public string TaskName { get; private set; }
        public string Worker { get; private set; }
        public string Report { get; private set; }
        public DateTime Start { get; private set; }
        public DateTime Stop { get; private set; }
        public string Chat { get; private set; }

        public StopCommand(int taskNum, string taskName, string worker, string report, DateTime start, DateTime stop, string chat)
        {
            TaskNum = taskNum;
            TaskName = taskName;
            Worker = worker;
            Report = report;          
            Start = start;
            Stop = stop;
            Chat = chat;
        }
        public static StopCommand Instantiate(string[] values)
        {
            if (values.Length == tags.Length)
            {
                try
                {
                    String msg = values[4].Replace("<br>", "\n");
                    DateTime start = POSTCommand.UnixTimeToLocalTime(long.Parse(values[5]));
                    DateTime stop = POSTCommand.UnixTimeToLocalTime(long.Parse(values[6]));
                    return new StopCommand(Int32.Parse(values[1]), values[2], values[3], msg, start, stop, values[7]);
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
                + tags[0] + ": " + identifier +"\n"
                + tags[1] + ": " + TaskNum + "\n"
                + tags[2] + ": " + Worker +"\n"
                + tags[3] + ": " + Report +"\n"
                + tags[4] + ": " + Start +"\n"
                + tags[5] + ": " + Stop +"\n"
                + tags[6] + ": " + Chat;
        }
        public string GenerateMessage()
        {
            return "*" + Worker + "*\n"
                + "[" + TaskName + "](https://bankrotforum.planfix.ru/task/" + TaskNum + ") *(" + Math.Round((Stop - Start).TotalHours, 1) + " ч.)*\n"
                + Report;
        }
    }
}
