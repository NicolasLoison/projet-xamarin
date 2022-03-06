using System;

namespace Projet.Model
{
    public class Timer
    {
        public int Id
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

        public Timer(int id, DateTime startTime, DateTime endTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}