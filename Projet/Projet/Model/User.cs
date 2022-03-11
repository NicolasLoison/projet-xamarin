using System.Collections.Generic;
using TimeTracker.Dtos;

namespace Projet.Model
{
    public class User
    {

        public List<Project> Projets
        {
            get;
            set;
        }

        public string AccessToken
        {
            get;
            set;
        }

        public string RefreshToken
        {
            get;
            set;
        }

        public string TokenType
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

        public User(string accessToken, string refreshToken, string tokenType, string firstName, string lastName, string email, string password)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            TokenType = tokenType;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            ClientId = Urls.CLIENT_ID;
            ClientSecret = Urls.CLIENT_SECRET;
            Projets = new List<Project>();
        }

        public User(string accessToken, string refreshToken, string tokenType, string firstName, string lastName, string email, string password, List<Project> projets)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            TokenType = tokenType;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            ClientId = Urls.CLIENT_ID;
            ClientSecret = Urls.CLIENT_SECRET;
            Projets = projets;
        }
    }
}