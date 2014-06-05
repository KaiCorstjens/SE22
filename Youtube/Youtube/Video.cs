// <copyright file="Video.cs" company="Kai Corstjens">
//     Copyright (c) Proftaak Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class Video
    {
        public string Title { get; set; }
        // Uploader shouldn't change, so private set (constructor)
        public User Uploader { get; private set; }
        public string Description { get; set; }
        // Add Views via method, with private set?
        public int Views { get; set; }
        // Changed variable name from date to UploadDate, thought this makes more sense.
        public DateTime UploadDate { get; set; }
        // Edited by a method, so private set.
        public int Likes { get; private set; }
        // Add dislikes via method, private set?
        public int DisLikes { get; set; }
        public bool Private { get; private set; }
        public List<Comment> Comments { get; set; }

        public Video(string title, User uploader, string description, bool isPrivate)
        {
            this.Title = title;
            this.Uploader = uploader;
            this.Description = description;
            this.Views = 0;
            // If possible, change this to the date/time on the server.
            this.UploadDate = DateTime.Now;
            this.Likes = 0;
            this.DisLikes = 0;
            this.Private = isPrivate;
            this.Comments = new List<Comment>();
        }
    }
}