using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.Alert
{
    class AlertTask
    {
        string WorkerName { get; }
        int TaskNum { get; }
        DateTime StartDate { get; }

        public DateTime FirstAlertDate { get; private set; }
        public bool IsFirstAlert { get; private set; }

        public DateTime SecondAlertDate { get; private set; }
        public bool IsSecondAlert { get; private set; }

        public AlertTask(string workerName, int taskNum, DateTime startDate, int untilFirstAlert, int untilSecondalert)
        {
            WorkerName = workerName;
            TaskNum = taskNum;
            StartDate = startDate;

            UpdateAlertDates(untilFirstAlert, untilSecondalert);
        }

        public void UpdateAlertDates(int untilFirstAlert, int untilSecondAlert)
        {
            FirstAlertDate = StartDate.AddHours(untilFirstAlert);
            SecondAlertDate = StartDate.AddHours(untilSecondAlert);
        }

    }
}
