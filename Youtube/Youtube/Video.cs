// <copyright file="Video.cs" company="Kai Corstjens">
//     Copyright (c) Proftaak Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.IO;

    public class Video
    {
        // VideoID shouldn't change, so private set (constructor).
        public int VideoID {get; private set;}
        public string Title { get; set; }
        // Uploader shouldn't change, so private set (constructor)
        public string Uploader { get; private set; }
        public string Description { get; set; }
        // Add Views via method, with private set?
        public int Views { get; set; }
        // Changed variable name from date to UploadDate, thought this makes more sense.
        public string UploadDate { get; set; }
        // Edited by a method, so private set.
        public int Likes { get; private set; }
        // Add dislikes via method, private set?
        public int DisLikes { get; set; }
        public bool Private { get; private set; }
        public List<Comment> Comments { get; set; }
        public string Location { get; private set;}

        public Video(int videoID,string title, string uploader, string description,int views, int likes, int disLikes, bool isPrivate,string fileLocation)
        {
            this.VideoID = videoID;
            this.Title = title;
            this.Uploader = uploader;
            this.Description = description;
            this.Views = views;
            // If possible, change this to the date/time on the server.
            string dateString = DateTime.Now.ToShortDateString();
            this.UploadDate = dateString;
            this.Likes = likes;
            this.DisLikes = disLikes;
            this.Private = isPrivate;
            this.Comments = new List<Comment>();
            this.Location = fileLocation;
        }
        public Video(int videoID, string title, string uploader, string description, bool isPrivate, List<Comment> commentList, string fileLocation)
        {
            this.VideoID = videoID;
            this.Title = title;
            this.Uploader = uploader;
            this.Description = description;
            this.Views = 0;
            // If possible, change this to the date/time on the server.
            string dateString = DateTime.Now.ToShortDateString();
            this.UploadDate = dateString;
            this.Likes = 0;
            this.DisLikes = 0;
            this.Private = isPrivate;
            this.Comments = commentList;
            this.Location = fileLocation;
        }

        public void LikeVideo(bool like)
        {
            if (like)
            {
                Likes++;
            }
            else if (!like)
            {
                Likes--;
            }
        }
    }
}