using System;
using Newtonsoft.Json;

namespace Projet.Model
{
    public class Timer
    {
        [JsonProperty("id")]
        public int Id
        {
            get;
            set;
        }
        [JsonProperty("start_time")]
        public DateTime StartTime
        {
            get;
            set;
        }
        [JsonProperty("end_time")]
        public DateTime EndTime
        {
            get;
            set;
        }

        public string StartTimeFormat
        {
            get => Convert.ToDateTime(StartTime).ToString("dd/MM/yyyy HH:mm");
        }
        
        public string EndTimeFormat
        {
            get => Convert.ToDateTime(EndTime).ToString("dd/MM/yyyy HH:mm");
        }

        public Timer(int id, DateTime startTime, DateTime endTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}