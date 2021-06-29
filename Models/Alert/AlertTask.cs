using StopHandler.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace StopHandler.Models.Alert
{
    class AlertTask: ViewModel
    {
        public string WorkerName { get; }
        public int TaskNum { get; }
        public DateTime StartDate { get; }

        public DateTime FirstAlertDate { get; private set; }

        bool _IsFirstAlertSend;
        public bool IsFirstAlertSend { get => _IsFirstAlertSend; set => Set(ref _IsFirstAlertSend, value); }

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
            //debug
            //FirstAlertDate = StartDate.AddMinutes(untilFirstAlert);
            //SecondAlertDate = StartDate.AddMinutes(untilSecondAlert);

            FirstAlertDate = StartDate.AddHours(untilFirstAlert);
            SecondAlertDate = StartDate.AddHours(untilSecondAlert);
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
            return "{'taskNum':" + TaskNum + ", 'time': " + DateTime.Now.Subtract(StartDate).Hours + "}";
        }
    }
}
