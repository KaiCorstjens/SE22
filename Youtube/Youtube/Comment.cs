// <copyright file="Comment.cs" company="Kai Corstjens">
//     Copyright (c) Proftaak Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class Comment
    {
        // Can't/Shouldn't be changed afterwards, so private set (set in constructor)
        public Video Video { get; private set; }
        public string Text { get; set; }
        // Not according to class diagram, changed name from User to Poster, thought it makes more sense.
        public User Poster { get; set; }
        //Not according to class diagram, changed name from Date to PostedDate, thought it makes more sense.
        public DateTime PostedDate { get; set; }
        public int Likes { get; set; }
        public Comment CommentOn { get; set; }

        public Comment(Video video, string text, User poster)
        {
            this.Video = video;
            this.Text = text;
            this.Poster = poster;
            // If possible, change this to date/time on the server.
            this.PostedDate = DateTime.Now;
            this.Likes = 0;
        }

        public Comment(Video video, string text, User poster, Comment commentOn)
        {
            this.Video = video;
            this.Text = text;
            this.Poster = poster;
            // If possible, change this to date/time on the server.
            this.PostedDate = DateTime.Now;
            this.Likes = 0;
            this.CommentOn = commentOn;
        }

        // Not according to class diagram, changed name from New_Reaction to NewComment, thought it makes more sense.
        public bool NewComment(Video video)
        {
            return true;
        }

        public bool NewComment(Video video,Comment comment)
        {
            return true;
        }

    }
}