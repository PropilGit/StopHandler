using StopHandler.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace StopHandler.Models
{
    class AlertTask: ViewModel
    {
        public string WorkerName { get => Worker.Name; }
        public PlanFixUser Worker { get; }
        public int TaskNum { get; }
        public string TaskName { get; }
        public DateTime StartDate { get; }

        public DateTime FirstAlertDate { get; private set; }

        bool _IsFirstAlertSend;
        public bool IsFirstAlertSend { get => _IsFirstAlertSend; set => Set(ref _IsFirstAlertSend, value); }

        public DateTime SecondAlertDate { get; private set; }
        public bool IsSecondAlertSend { get; private set; }

        public AlertTask(PlanFixUser worker, int taskNum, string taskName, DateTime startDate, int untilFirstAlert, int untilSecondalert)
        {
            Worker = worker;
            TaskNum = taskNum;
            TaskName = taskName;

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

        public string GenerateStringForPlanFix()
        {
            return "{'taskNum':" + TaskNum + ", 'time': " + DateTime.Now.Subtract(StartDate).Hours + "}";
        }
        public string GenerateStringForTelegram()
        {
            return "Задача [" + TaskName + "](https://bankrotforum.planfix.ru/task/" + TaskNum + ") запущена на протяжении *" + DateTime.Now.Subtract(StartDate).Hours + " ч.*\n"
                + "Напишите отчет по задаче и остановите таймер.";
        }
    }
}
