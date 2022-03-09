﻿using System;
using System.Collections.Generic;

namespace Projet.Model
{
    public class Project
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

        public Project(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
            TotalSeconds = 0;
            Tasks = new List<Task>();
        }
    }
}