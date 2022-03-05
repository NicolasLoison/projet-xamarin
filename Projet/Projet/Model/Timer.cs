using System;

namespace Projet.Model
{
    public class Timer
    {
        public Guid Id
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

        public Timer(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}