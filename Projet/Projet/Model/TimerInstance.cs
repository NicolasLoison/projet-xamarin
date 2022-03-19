using System;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace Projet.Model
{
    public class TimerInstance
    {
        public static TimerInstance Timer = new TimerInstance();

        public HomeViewModel HomeViewModel
        {
            get;
            set;
        }

        public ProjectViewModel ProjectViewModel
        {
            get;
            set;
        }

        public TaskViewModel TaskViewModel
        {
            get;
            set;
        }
        
        public string CurrentTime
        {
            get;
            set;
        }
        
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
            Device.StartTimer (new TimeSpan (0, 0, 0, 0, 10), () =>
            {
                Device.BeginInvokeOnMainThread (() =>
                {
                    CurrentTime = GetCurrentTotalTime().ToString("hh':'mm':'ss");
                    if (HomeViewModel != null)
                    {
                        HomeViewModel.TimerValue = CurrentTime;
                    }
                    if (ProjectViewModel != null)
                    {
                        ProjectViewModel.TimerValue = CurrentTime;
                    }
                    if (TaskViewModel != null)
                    {
                        TaskViewModel.TimerValue = CurrentTime;
                    }
                });
                
                return Started; // runs again, or false to stop
            });
        }
        
        public void Stop()
        {
            EndTime = DateTime.Now;
            Started = false;
        }
        
        public TimeSpan GetCurrentTotalTime()
        {
            if (Started)
            {
                return DateTime.Now.Subtract(StartTime);
            }

            return GetTotalTime();
        }
        
        public TimeSpan GetTotalTime()
        {
            return EndTime.Subtract(StartTime);
        }
    }
}