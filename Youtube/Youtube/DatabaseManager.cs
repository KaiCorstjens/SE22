// <copyright file="DatabaseManager.cs" company="Kai Corstjens">
//     Copyright (c) Proftaak Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class DatabaseManager
    {
        private string domain;
        private string forest;
        // Not found in the class diagram.
        private string username;
        // Not found in the class diagram.
        private string password;

        public bool Authenticate(User user)
        {
            return true;
        }

        public bool Register(User user)
        {
            return true;
        }
        // Not according to class diagram, changed Add_Video to AddVideo.
        public bool AddVideo(Video video)
        {
            return true;
        }
        // Not according to class diagram, changed Add_Playlist to AddPlaylist.
        public bool AddPlaylist(Playlist playlist)
        {
            return true;
        }
        // Not according to class diagram, changed Add_Comment to AddComment.
        public bool AddComment(Comment comment)
        {
            return true;
        }
    }
}