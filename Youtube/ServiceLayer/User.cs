// <copyright file="User.cs" company="Kai Corstjens">
//     Copyright (c) Proftaak Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube.ServiceLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    public class User
    {
        public string Username { get; set; }
        // Possibly find a securer way for the password?
        public string Password { get; set; }
        public List<Video> Videos { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Playlist> Playlists { get; set; }

        public User(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            Videos = new List<Video>();
            Comments = new List<Comment>();
            Playlists = new List<Playlist>();
        }
    }
}