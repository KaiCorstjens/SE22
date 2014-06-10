// <copyright file="DatabaseManager.cs" company="Kai Corstjens">
//     Copyright (c) Individuele opdracht Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.IO;
    using System.Data.OracleClient;
    using System.Data;
    using Oracle.DataAccess.Types;
    using Youtube.ServiceLayer;

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
            string query = "SELECT * FROM SE_GEBRUIKER WHERE GEBRUIKERSNAAM = :userParameter";

            OracleCommand command = new OracleCommand(query, connection);
            command.Parameters.Add("userParameter", user.Username);
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
        public byte[] GetVideoAsBlob(int videoID)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "SELECT * FROM SE_VIDEO WHERE videoID =:videoID";
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("videoID", videoID);

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
            string query = "INSERT INTO SE_VIDEO (VideoID,Titel,Uploader,Beschrijving,KeerBekeken,Datum,Likes,Dislikes,Prive,BlobFile) VALUES (:videoID,:videoTitle,:videoUploader,:videoDescription," + 0 + ",:videoUploadDate," + 0 + "," + 0 + "," + isPrivate + ",:BlobParameter)";
            
            OracleParameter blobParameter = new OracleParameter();
            blobParameter.OracleType = OracleType.Blob;
            blobParameter.ParameterName = "BlobParameter";
            blobParameter.Value = byteArray;

            

            OracleCommand command = new OracleCommand(query, connection);
            command.Parameters.Add("videoID", video.VideoID);
            command.Parameters.Add("videoTitle", video.Title);
            command.Parameters.Add("videoUploader", video.Uploader);
            command.Parameters.Add("videoDescription", video.Description);
            command.Parameters.Add("videoUploadDate", video.UploadDate);
            command.Parameters.Add(blobParameter);
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
        public List<Playlist> GetPlaylists (string username)
        {
            List<Playlist> playlistList = new List<Playlist>();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "SELECT afspeellijstID,NAAM FROM SE_Afspeellijst WHERE gebruikersnaam = :userName";

            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;

            command.Parameters.Add("userName", username);
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
                    playlistList.Add(playlist);
                }
                
            }
            catch
            {
                // Catch if reading from the database doesn't work
            }
            connection.Close();
            return playlistList;
        }

        public bool AddVideoToPlaylist(int playlistID,int videoID)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "INSERT INTO SE_VIDEOPERAFSPEELLIJST (AFSPEELLIJSTID,VIDEOID) VALUES (:playlistID,:videoID)";

            OracleCommand command = new OracleCommand(query, connection);
            command.Parameters.Add("playlistID", playlistID);
            command.Parameters.Add("videoID", videoID);
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

        public List<Video> GetVideoFromPlaylist(int playlistId)
        {
            List<Video> videoList = new List<Video>();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "SELECT v.videoID,v.Titel,v.UPloader,v.Beschrijving,v.keerbekeken,v.datum,v.likes,v.dislikes,v.prive FROM SE_VIDEO v,SE_AFSPEELLIJST afsplt,SE_VIDEOPERAFSPEELLIJST vpal WHERE v.videoID = vpal.videoID AND afsplt.afspeellijstid = :playlistID";
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("playlistID", playlistId);
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
            // Parameter for new(dis)likes not needed, value calculated by code.
            if (likeDislike)
            {
                query = "UPDATE SE_VIDEO SET LIKES = '"+newlikes+"' WHERE VIDEOID= :videoID";
            }
            else if (!likeDislike)
            {
                query = "UPDATE SE_VIDEO SET DISLIKES = '"+newdislikes+"' WHERE VIDEOID= :videoID";
            }

            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("videoID", videoId);
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
            string query = "INSERT INTO SE_GEBRUIKER (GEBRUIKERSNAAM,WACHTWOORD) VALUES (:userName,:password)";

            OracleCommand command = new OracleCommand(query, connection);
            command.Parameters.Add(":userName", user.Username);
            command.Parameters.Add(":password", user.Password);
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
            string query = "SELECT * FROM SE_VIDEO WHERE VIDEOID=:videoID";
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("videoID", videoID);
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
            string query = "INSERT INTO SE_REACTIE (REACTIEID,VIDEOID,TEKST,GEBRUIKERSNAAM,DATUM,LIKES) VALUES (:commentID,:videoID,:commentText,:commentPoster,:commentPostedDate,'"+0+"')";
            // Located lower than usual, because GetComments(video) would close the connection.
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("commentID", comment.CommentID);
            command.Parameters.Add("videoID", video.VideoID);
            command.Parameters.Add("commentText", comment.Text);
            command.Parameters.Add("commentPoster", comment.Poster.Username);
            command.Parameters.Add("commentPostedDate", comment.PostedDate);

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
            string query = "SELECT * FROM SE_REACTIE WHERE VIDEOID=:videoID";
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("videoID", video.VideoID);
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
            string query = "SELECT * FROM SE_GEBRUIKER WHERE GEBRUIKERSNAAM= :username";
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("username", userName);
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
            string query = "SELECT * FROM SE_REACTIE WHERE REACTIEID=commentID";
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("commentID", commentID);
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
        public List<Playlist> GetAllPlaylists()
        {
            List<Playlist> PlaylistList = new List<Playlist>();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "SELECT * FROM SE_AFSPEELLIJST";
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            OracleDataReader dataReader;
            int playlistId = 0;
            string username = string.Empty;
            string name = string.Empty;
            try
            {
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    playlistId= Convert.ToInt32(dataReader["AFSPEELLIJSTID"]);
                    username = Convert.ToString(dataReader["GEBRUIKERSNAAM"]);
                    name = Convert.ToString(dataReader["NAAM"]);
                    Playlist playlist = new Playlist(playlistId, GetUser(username), name);
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
        public bool NewPlaylist(Playlist playlist)
        {
            //List<Comment> comments = GetComments(video);
            //int commentID = comments.Count() + 1;
            string query = "INSERT INTO SE_AFSPEELLIJST (AFSPEELLIJSTID,GEBRUIKERSNAAM,NAAM) VALUES (:playlistID,:username,:playlistName)";
            // Located lower than usual, because GetComments(video) would close the connection.
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("playlistID", playlist.PlaylistID);
            command.Parameters.Add("username", playlist.User.Username);
            command.Parameters.Add("playlistName", playlist.Name);

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
        public bool DeleteVideo(Video video)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string query = "DELETE FROM SE_VIDEO WHERE VIDEOID= :videoID";

            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("videoID", video.VideoID);

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
            string query = "DELETE FROM SE_REACTIE WHERE REACTIEID=:commentID";

            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("commentID", commentID);

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

               string  query = "UPDATE SE_VIDEO SET KEERBEKEKEN = '" + newViews + "' WHERE VIDEOID= :videoID";

            OracleCommand command = new OracleCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("videoID",video.VideoID);
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