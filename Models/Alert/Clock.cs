using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace StopHandler.Models
{
    class Clock
    {
        #region Singleton

        static Clock instance;
        public static Clock GetInstance()
        {
            if (instance == null) instance = new Clock(true);
            return instance;
        }

        #endregion

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
}
