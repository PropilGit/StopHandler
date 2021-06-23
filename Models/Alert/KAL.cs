using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace StopHandler.Models.Alert
{
    class KAL
    {
        class Clock
        {
            static Clock instance;
            public static Clock GetInstance()
            {
                if (instance == null) instance = new Clock(true);
                return instance;
            }

            public Clock(bool isMoscowTime)
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
                dispatcherTimer.Start();

                Hour = DateTime.Now.Hour;
                Min = DateTime.Now.Minute;
            }

            DispatcherTimer dispatcherTimer;
            public int Hour { get; private set; }
            public int Min { get; private set; }

            public delegate void UpdateTime(int hour, int min);
            public event UpdateTime onTimeUpdate;

            private void dispatcherTimer_Tick(object sender, EventArgs e)
            {
                Update();
            }

            public void Update()
            {
                Hour = DateTime.Now.Hour;
                Min = DateTime.Now.Minute;

                onTimeUpdate(Hour, Min);
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
}
