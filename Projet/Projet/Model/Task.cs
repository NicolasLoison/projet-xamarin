using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Projet.Model
{
    public class Task
    {
        public int Id
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

        public Task(int id, string name)
        {
            Id = id;
            Name = name;
            Times = new List<Timer>();
        }
    }
}