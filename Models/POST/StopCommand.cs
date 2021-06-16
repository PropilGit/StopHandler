using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.POST
{
    class StopCommand : IPOSTCommand
    {
        /*
        CMD#STOP#CMD
        TASK#121574#TASK
        WRK#Путин Владимир#WRK
        REP#Однажды, в студеную зимнюю пору, Я из лесу вышел; был сильный мороз.#REP
        STR#20-01-2021 14:48#STR
        STP#20-01-2021 14:59#STP
        CHAT#ТО#CHAT
        */
        public string[] Tags { get => tags; }
        public static readonly string[] tags = new string[7] {"CMD", "TASK", "WRK", "REP", "STR", "STP", "CHAT"};
        public string Identifier { get => identifier; }
        public static readonly string identifier = "STOP";
        public int TaskNum { get; private set; }
        public string Worker { get; private set; }
        public string Report { get; private set; }
        public DateTime Start { get; private set; }
        public DateTime Stop { get; private set; }
        public string Chat { get; private set; }

        public StopCommand(int taskNum, string worker, string report, DateTime start, DateTime stop, string chat)
        {
            Worker = worker;
            Report = report;
            TaskNum = taskNum;
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
                    return new StopCommand(Int32.Parse(values[1]), values[2], values[3], DateTime.Parse(values[4]), DateTime.Parse(values[5]), values[6]);
                }
                catch (Exception)
                {
                    
                }
            }
            return null;
        }
        public string ToLog()
        {
            return "Получен POST-запрос:\n"
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
                + "[" + TaskNum + "](https://bankrotforum.planfix.ru/task/" + TaskNum + ") *(" + Math.Round((Stop - Start).TotalHours, 1) + " ч.)*\n"
                + Report;
        }
    }
}
