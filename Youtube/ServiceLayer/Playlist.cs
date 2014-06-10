// <copyright file="Playlist.cs" company="Kai Corstjens">
//     Copyright (c) Individuele opdracht Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube.ServiceLayer
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    public class Playlist
    {
        public int PlaylistID { get; set; }
        public User User { get; set; }
        public List<Video> Videos { get; set; }
        public string Name { get; set; }

        public Playlist(int playlistID, User user,string name)
        {
            this.PlaylistID = playlistID;
            this.User = user;
            this.Name = name;
            Videos = new List<Video>();
        }
        public bool AddVideo(Video video)
        {
            return true;
        }
    }
}