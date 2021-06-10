using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.POST
{
    class StopCommand : IPOSTCommand
    {
        /*
        CMD#STOP#CMD
        TASK#201488#TASK
        WRK#Иванов Иван#WRK
        MSG#Отчет по работе#MSG
        TIME#20-01-2021 14:48#TIME
        SPENT#30#SPENT
        */
        public string[] Tags { get => tags; }
        public static string[] tags = new string[6] {"CMD", "TASK", "WRK", "MSG", "TIME", "SPENT"};
        public string Identifier { get => identifier; }
        public static string identifier = "STOP";
        public int TaskNum { get; private set; }
        public string Worker { get; private set; }
        public string Message { get; private set; }
        public DateTime Time { get; private set; }
        public float Spent { get; private set; }

        StopCommand(int taskNum, string worker, string message, DateTime time, int spent)
        {
            Worker = worker;
            Message = message;
            TaskNum = taskNum;
            Time = time;
            Spent = spent;
        }
        public static StopCommand Instantiate(string[] values)
        {
            if (values.Length == tags.Length)
            {
                try
                {
                    return new StopCommand(Int32.Parse(values[1]), values[2], values[3], DateTime.Parse(values[4]), Int32.Parse(values[5]));
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
            + tags[3] + ": " + Message +"\n"
            + tags[4] + ": " + Time +"\n"
            + tags[5] + ": " + Spent;
        }
        public string GenerateMessage()
        {
            return "**" +  + "**"
        }
    }
}
