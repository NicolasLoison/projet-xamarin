using System;
using System.Security.Cryptography;

namespace Projet.Model
{
    public class User
    {
        public Guid Id
        {
            get;
            set;
        }
        public string FirstName
        {
            get;
            set;
        }
        public string LastName
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public string Password
        {
            get;
            set;
        }
        public string ClientId
        {
            get;
            set;
        }
        public string ClientSecret
        {
            get;
            set;
        }

        public User(string firstName, string lastName, string email, string password, string clientId, string clientSecret)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }
}