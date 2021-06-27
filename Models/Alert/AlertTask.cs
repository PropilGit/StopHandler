using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.Alert
{
    class AlertTask
    {
        public string WorkerName { get; }
        public int TaskNum { get; }
        public DateTime StartDate { get; }

        public DateTime FirstAlertDate { get; private set; }
        public bool IsFirstAlertSend { get; private set; }

        public DateTime SecondAlertDate { get; private set; }
        public bool IsSecondAlertSend { get; private set; }

        public AlertTask(string workerName, int taskNum, DateTime startDate, int untilFirstAlert, int untilSecondalert)
        {
            WorkerName = workerName;
            TaskNum = taskNum;

            StartDate = startDate;

            UpdateAlertDates(untilFirstAlert, untilSecondalert);
        }

        public void UpdateAlertDates(int untilFirstAlert, int untilSecondAlert)
        {
            FirstAlertDate = StartDate.AddMinutes(untilFirstAlert);
            SecondAlertDate = StartDate.AddMinutes(untilSecondAlert);

            //FirstAlertDate = StartDate.AddHours(untilFirstAlert);
            //SecondAlertDate = StartDate.AddHours(untilSecondAlert);
        }

        public bool CheckFirstAlert(DateTime dt)
        {
            if (IsFirstAlertSend) return false;
            if (dt > FirstAlertDate)
            {
                IsFirstAlertSend = true;
                return true;
            }
            return false;
        }
        public bool CheckSecondAlert(DateTime dt)
        {
            if (IsSecondAlertSend) return false;
            if (dt > SecondAlertDate)
            {
                IsSecondAlertSend = true;
                return true;
            }
            return false;
        }

        public Dictionary<string, string> GenerateValuesForPlanFix()
        {
            return new Dictionary<string, string> {
                { "taskNum", TaskNum.ToString() },
                { "time", StartDate.Subtract(DateTime.Now).Hours.ToString() }
            };
        }
        public string GenerateStringForPlanFix()
        {
            return "{'taskNum':" + TaskNum + ", 'time': " + StartDate.Subtract(DateTime.Now).Hours + "}";
            //{'taskNum':121574,'time':1}
        }
    }
}
