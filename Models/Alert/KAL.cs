using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace StopHandler.Models.Alert
{
    /*
    class KAL
    {
        class Controller
        {
            static Controller instance;
            public static Controller GetInstance()
            {
                if (instance == null) instance = new Controller();
                return instance;
            }

            public Controller()
            {
                firstAlertTimeValue = 6;
                secondAlertTimeValue = 8;
                ttasks = new List<TimerTask>();
            }

            public delegate void UpdateTaskList();
            public event UpdateTaskList onTaskListupdate;

            public delegate void SMTP_NoticeReqes(int taskNum, int time);
            public event SMTP_NoticeReqes onSMTP_AlertReqest;

            public int MinFirstTime { get => minFirstTime; }
            int minFirstTime = 1;
            public int MaxFirstTime { get => maxFirstTime; }
            int maxFirstTime = 11;

            public int MinSecondTime { get => minSecondTime; }
            int minSecondTime = 2;
            public int MaxSecondTime { get => maxSecondTime; }
            int maxSecondTime = 12;

            public int FirstAlertTime
            {
                get { return firstAlertTimeValue; }
                set
                {
                    firstAlertTimeValue = value;
                }
            }
            int firstAlertTimeValue;

            public int SecondAlertTime
            {
                get { return secondAlertTimeValue; }
                set
                {
                    secondAlertTimeValue = value;
                }
            }
            int secondAlertTimeValue;

            public List<TimerTask> TTasks { get => ttasks; }
            List<TimerTask> ttasks;

            public void ApplyTimerTask(POSTCommand cmd)
            {
                if (cmd.Command == "STOP")
                {
                    if (FindTimerTask(cmd.TaskNum) >= 0)
                    {
                        RemoveTimerTask(FindTimerTask(cmd.TaskNum));
                    }
                    else Error();
                }

                if (cmd.Command == "START")
                {
                    AddTimerTask(cmd);
                }
            }
            public void AddTimerTask(POSTCommand cmd)
            {
                if (FindTimerTask(cmd.TaskNum) != -1)
                {
                    Error();
                    return;
                }

                TTasks.Add(
                    new TimerTask(cmd.TaskNum, cmd.WorkerName));
                onTaskListupdate();
            }
            int FindTimerTask(int taskNum)
            {
                if (TTasks == null || TTasks.Count < 1)
                {
                    return -1;
                }
                for (int i = 0; i < TTasks.Count; i++)
                {
                    if (TTasks[i].TaskNum == taskNum)
                    {
                        return i;
                    }
                }
                return -1;
            }
            void RemoveTimerTask(int index)
            {
                if (index < 0 || index > TTasks.Count - 1)
                {
                    Error();
                    return;
                }

                TTasks.Remove(TTasks[index]);
                onTaskListupdate();
            }
            public void OnTimeUpdate(int hour, int min)
            {
                for (int i = 0; i < TTasks.Count; i++)
                {
                    if (TTasks[i].Start.AddHours(firstAlertTimeValue) <= DateTime.Now && !TTasks[i].IsSendFirstAlert)
                    {
                        TTasks[i].IsSendFirstAlert = true;
                        onSMTP_AlertReqest(TTasks[i].TaskNum, firstAlertTimeValue);
                    }
                    else if (TTasks[i].Start.AddHours(secondAlertTimeValue) <= DateTime.Now)
                    {
                        onSMTP_AlertReqest(TTasks[i].TaskNum, secondAlertTimeValue);
                        RemoveTimerTask(FindTimerTask(TTasks[i].TaskNum));
                    }
                }
                onTaskListupdate();
            }
            void Error()
            {

            }
        }

        class TimerTask
        {
            public int TaskNum { get; private set; }
            public string WorkerName { get; private set; }
            public DateTime Start { get; private set; }
            public bool IsSendFirstAlert { get; set; }

            public TimerTask(int taskNum, string workerName)
            {
                this.TaskNum = taskNum;
                this.WorkerName = workerName;
                this.Start = RemoveSeconds(DateTime.Now);
                this.IsSendFirstAlert = false;
            }


            DateTime RemoveSeconds(DateTime value)
            {
                int sec = value.Second;
                return value.AddSeconds(-sec);
            }
            public List<string> ToString()
            {
                List<string> result = new List<string>();

                result.Add(TaskNum.ToString());
                result.Add(WorkerName);
                result.Add(Start.ToString());

                return result;
            }
        }
    }
    */
}
