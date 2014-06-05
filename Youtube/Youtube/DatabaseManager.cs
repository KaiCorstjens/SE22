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
            string query = "INSERT INTO SE_VIDEO (VideoID,Titel,Uploader,Beschrijving,KeerBekeken,Datum,Likes,Dislikes,Prive,Categorie,Filename) VALUES ("+video.VideoID+",'"+video.Title+"','"+video.Uploader+"','"+video.Description+"',"+0+","+video.UploadDate+","+0+","+0+","+isPrivate+",'"+string.Empty+"','"+video.Location+"')";

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

        public List<Video> GetAllVideos()
        {
            List<Video> videoList = new List<Video>();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
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

        public bool VideoLike(Video video,bool likeDislike)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = string.Empty;
            int newlikes = video.Likes + 1;
            int newdislikes = video.DisLikes+ 1;
            int videoId = video.VideoID;
            if (likeDislike)
            {
                query = "UPDATE SE_VIDEO SET LIKES = '"+newlikes+"' WHERE VIDEOID=" + videoId;
            }
            else if (!likeDislike)
            {
                query = "UPDATE SE_VIDEO SET DISLIKES = '"+newdislikes+"' WHERE VIDEOID=" + videoId;
            }

            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {
                return false;
            }
            connection.Close();
            return true;
        }

        public bool AddUser(User user)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "INSERT INTO SE_GEBRUIKER (GEBRUIKERSNAAM,WACHTWOORD) VALUES ('"+user.Username+"','"+user.Password+"')";

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

        public Video GetVideo (int videoID)
        {
            Video video = new Video(0, string.Empty, string.Empty, string.Empty, true, null, string.Empty);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "SELECT * FROM SE_VIDEO WHERE VIDEOID="+videoID;
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
                    video = new Video(videoId, title, uploader, description, views, likes, disLikes, isPrivateBool, location);
                }
            }
            catch
            {
                // Catch if reading from the database doesn't work
            }
            connection.Close();
            return video;
        }

        public bool AddComment(Video video, Comment comment)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "INSERT INTO SE_REACTIE (REACTIEID,VIDEOID,TEKST,GEBRUIKERSNAAM,DATUM,LIKES) VALUES ('" + 0 + "','" + video.VideoID + "','" +comment.Text+"','"+comment.Poster.Username+"','"+comment.PostedDate+"','"+0+"')";

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

        public List<Comment> GetComments(Video video)
        {
            List<Comment> comments = new List<Comment>();
            Comment comment;
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "SELECT * FROM SE_REACTIE WHERE VIDEOID=" + video.VideoID;
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            OracleDataReader dataReader;
            int commentID = 0;
            string text = string.Empty;
            string username = string.Empty;
            User user;
            int commentOnInt = 0;
            Comment commentOn;
            try
            {
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    commentID = Convert.ToInt32(dataReader["REACTIEID"]);
                    text = Convert.ToString(dataReader["TEKST"]);
                    username = Convert.ToString(dataReader["GEBRUIKERSNAAM"]);
                    user = GetUser(username);
                    try
                    {
                        commentOnInt = Convert.ToInt32(dataReader["REACTIEOP"]);
                        commentOn = GetComment(commentOnInt);
                        comment = new Comment(commentID, video, text, user, commentOn);
                    }
                    catch
                    {
                        comment = new Comment(commentID, video, text, user);
                    }
                    comments.Add(comment);
                }
            }
            catch
            {
                // Catch if reading from the database doesn't work
            }
            connection.Close();
            return comments;
        }

        public User GetUser(string userName)
        {
            User user;
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "SELECT * FROM SE_GEBRUIKER WHERE GEBRUIKERSNAAM='" + userName+"'";
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            OracleDataReader dataReader;
            string username = string.Empty;
            string password = string.Empty;
            try
            {
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    username = Convert.ToString(dataReader["GEBRUIKERSNAAM"]);
                    password = Convert.ToString(dataReader["WACHTWOORD"]);
                }
            }
            catch
            {
                // Catch if reading from the database doesn't work
            }
            user = new User(username, password);
            connection.Close();
            return user;
        }

        public Comment GetComment(int commentID)
        {
            Comment comment = new Comment(0, null, string.Empty, null);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "SELECT * FROM SE_REACTIE WHERE REACTIEID=" + commentID;
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            OracleDataReader dataReader; 
            int commentIDInt = 0;
            int videoID;
            string text = string.Empty;
            string username = string.Empty;
            User user;
            int commentOnInt = 0;
            Comment commentOn;
            try
            {
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    commentIDInt = Convert.ToInt32(dataReader["REACTIEID"]);
                    text = Convert.ToString(dataReader["TEKST"]);
                    videoID = Convert.ToInt32(dataReader["VIDEOID"]);
                    username = Convert.ToString(dataReader["GEBRUIKERSNAAM"]);
                    user = GetUser(username);
                    commentOnInt = Convert.ToInt32(dataReader["REACTIEOP"]);
                    Video video = GetVideo(videoID);
                    if (commentOnInt != 0 && commentOnInt != null)
                    {
                        commentOn = GetComment(commentOnInt);
                        comment = new Comment(commentID, video, text, user, commentOn);
                    }
                    else
                    {
                        comment = new Comment(commentID, video, text, user);
                    }
                }
            }
            catch
            {
                // Catch if reading from the database doesn't work
            }
            connection.Close();
            return comment;
        }
    }
}