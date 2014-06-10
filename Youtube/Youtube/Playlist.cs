// <copyright file="Playlist.cs" company="Kai Corstjens">
//     Copyright (c) Individuele opdracht Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    public class Playlist
    {
        public User User { get; set; }
        public List<Video> Videos { get; set; }

        public Playlist(User user)
        {
            this.User = user;
            Videos = new List<Video>();
        }
        public bool AddVideo(Video video)
        {
            return true;
        }
    }
}