using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.core
{
    class StartCommand: IPOSTCommand
    {
        /*
        CMD#START#CMD
        TASK#201488#TASK
        WRK#Иванов Иван#WRK
        TIME#20-01-2021 14:48#TIME
        */
        public string[] Tags { get => tags; }
        public static string[] tags = new string[4] { "CMD", "TASK", "WRK", "TIME" };
        public string Identifier { get => identifier; }
        public static string identifier = "START";

        public int TaskNum { get; private set; }
        public string Worker { get; private set; }
        public DateTime Time { get; private set; }

        public StartCommand(int taskNum, string worker, DateTime time)
        {
            TaskNum = taskNum;
            Worker = worker;
            Time = time;
        }
        public static StartCommand Instantiate(string[] values)
        {
            if (values.Length == tags.Length)
            {
                try
                {
                    return new StartCommand(Int32.Parse(values[1]), values[2], DateTime.Parse(values[3]));
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
            + tags[0] + ": " + identifier + "\n"
            + tags[1] + ": " + TaskNum + "\n"
            + tags[2] + ": " + Worker + "\n"
            + tags[4] + ": " + Time;
        }
    }
}
