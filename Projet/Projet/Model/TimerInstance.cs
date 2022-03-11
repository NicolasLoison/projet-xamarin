using System;

namespace Projet.Model
{
    public class TimerInstance
    {
        public static TimerInstance Timer;

        public bool Started
        {
            get;
            set;
        }
        public DateTime StartTime
        {
            get;
            set;
        }
        
        public DateTime EndTime
        {
            get;
            set;
        }
        
        public TimerInstance()
        {
            StartTime = DateTime.Now;
            Started = true;
        }

        public void Stop()
        {
            EndTime = DateTime.Now;
            Started = false;
        }
        
        public TimeSpan GetTotalTime()
        {
            return EndTime.Subtract(StartTime);
        }
    }
}