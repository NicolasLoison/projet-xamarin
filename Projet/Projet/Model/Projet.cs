using System;
using System.Collections.Generic;

namespace Projet.Model
{
    public class Projet
    {
        public Guid Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public int TotalSeconds
        {
            get;
            set;
        }

        public List<Task> Tasks
        {
            get;
            set;
        }

        public Projet(string name, string description)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            TotalSeconds = 0;
            Tasks = new List<Task>();
        }
    }
}