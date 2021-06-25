using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace StopHandler.Models
{
    class Clock
    {
        #region Singleton

        static Clock instance;
        public static Clock GetInstance()
        {
            if (instance == null) instance = new Clock();
            return instance;
        }

        #endregion

        DispatcherTimer dispatcherTimer;
        public int Hour { get; private set; }
        public int Min { get; private set; }

        public delegate void UpdateTime(int hour, int min);
        public event UpdateTime onTimeUpdate;

        public Clock()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            
            Hour = DateTime.Now.Hour;
            Min = DateTime.Now.Minute;
            Update();

            SynchronizeClockAsync();
        }

        async void SynchronizeClockAsync()
        {
            await Task.Run(() => SynchronizeClock());
        }

        void SynchronizeClock()
        {
            int delta = 60 * 1000 - DateTime.Now.Second * 1000;
            Thread.Sleep(delta);

            dispatcherTimer.Start();
            Update();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Update();
        }

        public void Update()
        {
            Hour = DateTime.Now.Hour;
            Min = DateTime.Now.Minute;

            if(onTimeUpdate != null) onTimeUpdate(Hour, Min);
        }
    }
}
