// <copyright file="DatabaseManager.cs" company="Kai Corstjens">
//     Copyright (c) Proftaak Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Data;
    using Oracle.DataAccess.Client;
    using Oracle.DataAccess.Types;

    public class DatabaseManager
    {
        private OracleConnection connection = new OracleConnection();
        // Not found in the class diagram.
        private string pcn;
        // Not found in the class diagram.
        private string password;

        public DatabaseManager()
        {
            pcn = "dbi297953";
            password = "hN0VRtdlS0";
            connection.ConnectionString = "User Id=" + pcn + ";Password=" + password + ";Data Source=" + " //192.168.15.50:1521/fhictora" + ";";
        }
        public bool Authenticate(User user)
        {
            connection.Open();
            string query = "SELECT * FROM SE_GEBRUIKER WHERE GEBRUIKERSNAAM = '"+user.Username+"'";// SQL-injection?
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            OracleDataReader dataReader;
            string username = string.Empty;
            string userpassword = string.Empty;
            try
            {
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    username = Convert.ToString(dataReader["GEBRUIKERSNAAM"]);
                    userpassword = Convert.ToString(dataReader["WACHTWOORD"]);
                }
            }
            catch
            {
                // Catch if reading from the database doesn't work
            }
            
            connection.Close();
            if (user.Username == username && user.Password == userpassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Register(User user)
        {
            return true;
        }
        // Not according to class diagram, changed Add_Video to AddVideo.
        public bool AddVideo(Video video)
        {
           int isPrivate = 0;
            connection.Open();
            // Oracle doesn't support bools, so change it into a number (0=false, 1=true).
            if (video.Private)
            {
                isPrivate = 1;
            }
            else if (!video.Private)
            {
                isPrivate = 0;
            }
            string query = "INSERT INTO SE_VIDEO (VideoID,Titel,Uploader,Beschrijving,KeerBekeken,Datum,Likes,Dislikes,Prive,Categorie,Filename) VALUES ("+0+",'"+video.Title+"','"+video.Uploader+"','"+video.Description+"',"+0+","+video.UploadDate+","+0+","+0+","+isPrivate+",'"+string.Empty+"','"+video.Location+"')";

            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;

            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {
                return false;
                // Catch if the command was not succesfully executed.
            }

            connection.Close();
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

        public List<Video> GetAllVideos()
        {
            List<Video> videoList = new List<Video>();
            connection.Open();
            string query = "SELECT * FROM SE_VIDEO";
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            OracleDataReader dataReader;
            int videoId = 0;
            string title = string.Empty;
            string uploader = string.Empty;
            string description = string.Empty;
            int views = 0;
            string uploadDate = string.Empty;
            int likes = 0;
            int disLikes = 0;
            int isPrivate = 0;
            bool isPrivateBool = false;
            List<Comment> comments = new List<Comment>();
            string location = string.Empty;
            try                                                                                     
            {
                dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        videoId = Convert.ToInt32(dataReader["VIDEOID"]);
                        title = Convert.ToString(dataReader["TITEL"]);
                        uploader = Convert.ToString(dataReader["UPLOADER"]);
                        description = Convert.ToString(dataReader["BESCHRIJVING"]);
                        views = Convert.ToInt32(dataReader["KEERBEKEKEN"]);
                        uploadDate = Convert.ToString(dataReader["DATUM"]);
                        likes = Convert.ToInt32(dataReader["LIKES"]);
                        disLikes = Convert.ToInt32(dataReader["DISLIKES"]);
                        isPrivate = Convert.ToInt32(dataReader["PRIVE"]);
                        //Comments
                        location = Convert.ToString(dataReader["FILENAME"]);
                        if (isPrivate == 1)
                        {
                            isPrivateBool = true;
                        }
                        else if (isPrivate == 0)
                        {
                            isPrivateBool = false;
                        }
                        Video video = new Video(videoId, title, uploader, description,views,likes,disLikes, isPrivateBool, location);
                        videoList.Add(video);
                    }
            }
            catch
            {
                // Catch if reading from the database doesn't work
            }
            connection.Close();
            return videoList;
        }
    }
}