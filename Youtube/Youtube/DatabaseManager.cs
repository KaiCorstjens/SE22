// <copyright file="DatabaseManager.cs" company="Kai Corstjens">
//     Copyright (c) Individuele opdracht Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.IO;
    using System.Data.OracleClient;
    using System.Data;
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
        public byte[] GetVideoAsBlob(int videoID)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "SELECT * FROM SE_VIDEO WHERE videoID ="+videoID;
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            OracleDataReader dataReader;
            byte[] byteArray = new byte[1];
            try
            {
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    byteArray = (byte[])dataReader["BLOBFILE"];
                }
            }
            catch
            {

            }
            //stream = new MemoryStream(byteArray);
            return byteArray;
        }

        public bool AddVideoAsBlob(Video video, Stream fs)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            byte[] byteArray = null;
            BinaryReader reader = new BinaryReader(fs);
            byteArray = reader.ReadBytes((int)fs.Length);
            int isPrivate = 0;
            if (video.Private)
            {
                isPrivate = 1;
            }
            else if (!video.Private)
            {
                isPrivate = 0;
            }
            string query = "INSERT INTO SE_VIDEO (VideoID,Titel,Uploader,Beschrijving,KeerBekeken,Datum,Likes,Dislikes,Prive,Categorie,BlobFile) VALUES (" + video.VideoID + ",'" + video.Title + "','" + video.Uploader + "','" + video.Description + "'," + 0 + "," + video.UploadDate + "," + 0 + "," + 0 + "," + isPrivate + ",'" + string.Empty + "',"+":blobParameter )";
            OracleParameter blobParameter = new OracleParameter();
            blobParameter.OracleType = OracleType.Blob;
            blobParameter.ParameterName = "BlobParameter";
            blobParameter.Value = byteArray;

            OracleCommand command = new OracleCommand(query, connection);
            command.Parameters.Add(blobParameter);
            command.CommandType = CommandType.Text;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return false;
                // Catch if the command was not succesfully executed.
            }
            connection.Close();
            return true;
        }
        public bool AddPlaylist(Playlist playlist)
        {
            return true;
        }
        public List<Playlist> GetPlaylists (string username)
        {
            List<Playlist> PlaylistList = new List<Playlist>();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "SELECT afspeellijstID,NAAM FROM SE_Afspeellijst WHERE gebruikersnaam = '"+username+"'";

            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            OracleDataReader dataReader;
            int playlistID = 0;
            string name = string.Empty;
            try
            {
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    playlistID = Convert.ToInt32(dataReader["AFSPEELLIJSTID"]);
                    name = Convert.ToString(dataReader["NAAM"]);
                    Playlist playlist = new Playlist(playlistID,GetUser(username),name);
                    PlaylistList.Add(playlist);
                }
                
            }
            catch
            {
                // Catch if reading from the database doesn't work
            }
            connection.Close();
            return PlaylistList;
        }

        public List<Video> GetVideoFromPlaylist(int playlistId)
        {
            List<Video> videoList = new List<Video>();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "SELECT v.videoID,v.Titel,v.UPloader,v.Beschrijving,v.keerbekeken,v.datum,v.likes,v.dislikes,v.prive FROM SE_VIDEO v,SE_AFSPEELLIJST afsplt,SE_VIDEOPERAFSPEELLIJST vpal WHERE v.videoID = vpal.videoID AND afsplt.afspeellijstid = "+playlistId;
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
                    //location = Convert.ToString(dataReader["FILENAME"]);
                    //Comments
                    //location = Convert.ToString(dataReader["FILENAME"]);
                    if (isPrivate == 1)
                    {
                        isPrivateBool = true;
                    }
                    else if (isPrivate == 0)
                    {
                        isPrivateBool = false;
                    }
                    Video video = new Video(videoId, title, uploader, description, views, likes, disLikes, isPrivateBool, location);
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
                        //location = Convert.ToString(dataReader["FILENAME"]);
                        //Comments
                        //location = Convert.ToString(dataReader["FILENAME"]);
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
        public string AddComment(Video video, Comment comment)
        {
            //List<Comment> comments = GetComments(video);
            //int commentID = comments.Count() + 1;
            string query = "INSERT INTO SE_REACTIE (REACTIEID,VIDEOID,TEKST,GEBRUIKERSNAAM,DATUM,LIKES) VALUES ('" + comment.CommentID + "','" + video.VideoID + "','" +comment.Text+"','"+comment.Poster.Username+"','"+comment.PostedDate+"','"+0+"')";
            // Located lower than usual, because GetComments(video) would close the connection.
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.ToString();
                // Catch if the command was not succesfully executed.
            }
            connection.Close();
            return "Reactie succesvol toegevoegd";
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
                    // GetUser(username) will close the connection, so open it again.
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
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
            // GetUser() is only used for other queries, so the connection shouldn't be closed.
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
                    if (commentOnInt != 0)
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

        public bool DeleteVideo(Video video)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "DELETE FROM SE_VIDEO WHERE VIDEOID='"+video.VideoID+"' ";

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

        public bool DeleteComment (int commentID)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "DELETE FROM SE_REACTIE WHERE REACTIEID='" + commentID + "' ";

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

        public void AddView(Video video)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            int newViews = video.Views + 1;

               string  query = "UPDATE SE_VIDEO SET KEERBEKEKEN = '" + newViews + "' WHERE VIDEOID=" + video.VideoID;

            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {
                // Catch if reading from database was unsuccesful.
            }
            connection.Close();
        }
    }
}