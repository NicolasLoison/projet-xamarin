using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace Projet.Model
{
    public class TimerInstance
    {
        public static TimerInstance Timer = new TimerInstance();

        // public string CurrentTime
        // {
        //     get;
        //     set;
        // }
        // public TimerInstance()
        // {
        //     Device.StartTimer (new TimeSpan (0, 0, 0, 0, 10), () =>
        //     {
        //         if (Timer.Started)
        //         {
        //             Device.BeginInvokeOnMainThread (() =>
        //             {
        //                 Timer.CurrentTime = Timer.GetCurrentTotalTime().ToString();
        //             });
        //         }
        //         return true; // runs again, or false to stop
        //     });
        // }
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
        
        public TimeSpan GetCurrentTotalTime()
        {
            return DateTime.Now.Subtract(StartTime);
        }
        
        public TimeSpan GetTotalTime()
        {
            return EndTime.Subtract(StartTime);
        }
    }
}