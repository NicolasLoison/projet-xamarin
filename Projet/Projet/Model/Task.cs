using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Projet.Model
{
    public class Task
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

        public List<Timer> Times
        {
            get;
            set;
        }

        public Task(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            Times = new List<Timer>();
        }
    }
}