using System;

namespace Projet.Model
{
    public class TimerInstance
    {
        public static TimerInstance Timer = new TimerInstance();

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
        
        public void Start()
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