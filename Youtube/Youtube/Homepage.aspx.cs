// <copyright file="Homepage.aspx.cs" company="Kai Corstjens">
//     Copyright (c) Proftaak Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Web.UI.HtmlControls; 
    using System.IO;
    using System.Drawing;

    public partial class Homepage : System.Web.UI.Page
    {
        private DatabaseManager databaseManager;
        public List<Video> Videos {get; set;}
        public User CurrentUser { get; private set; }
        public Video CurrentVideo { get; private set; }
        public List<Comment> Comments { get; private set; }
        private int currentVideoID;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (databaseManager == null)
            {
                databaseManager = new DatabaseManager();
            }
            if (Videos == null)
            {
                Videos = databaseManager.GetAllVideos();
            }
            try
            {
                //CurrentUser = (User)Session["User"];
                string username = (string)(Session["Username"]);
                string password = (string)(Session["Password"]);
                if (username != string.Empty && username != null && password != null && password != string.Empty)
                {
                    CurrentUser = new User(username, password);
                }
                if(int.TryParse(Request.QueryString["video"],out currentVideoID))
                {
                    CurrentVideo = databaseManager.GetVideo(currentVideoID);
                }
                else
                {
                    if (Videos.Count != 0)
                    {
                        foreach (Video v in Videos)
                        {
                            if (CurrentVideo == null)
                            {
                                this.CurrentVideo = v;
                            }
                        }
                    }
                    else
                    {
                        this.CurrentVideo = new Video(0, "Geen video's gevonden", null, string.Empty, false, null, string.Empty);
                    }
                }
                if (CurrentUser != null)
                {
                    Login(CurrentUser);
                }
            }
            catch
            {
                // No session found.
            }
            
            string path = Server.MapPath("/");
            int filenameBeginInt = CurrentVideo.Location.LastIndexOf(@"\");
            string filename = CurrentVideo.Location.Substring(filenameBeginInt);
            string destinationPath = Server.MapPath("/") + @"Video\" + filename;
            if (!File.Exists(destinationPath))
            {
                File.Copy(CurrentVideo.Location, destinationPath);
            }
            HTMLVideo.Src = @"Video\"+filename;
            ChangeVideo(CurrentVideo);

            Comments = databaseManager.GetComments(CurrentVideo);
            AddComments();
            if (CurrentVideo != null && CurrentUser != null)
            {
                if (CurrentVideo.Uploader == CurrentUser.Username)
                {
                    BtnDeleteVideo.Visible = true;
                }
            }
            // HTMLVideo.Src = @"C:\SE22\KaiCorstjens\Videos\CurrentVideo4.mp4";
        }
        protected void BtnUpload_Click(object sender, EventArgs e)
        {
            if (CurrentUser != null)
            {
                string url = "Upload.aspx";
                Response.Redirect(url);
            }
            else
            {
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Je moet ingelogd zijn om video's te uploaden.";
            }
        }

        protected void BtnLogIn_Click(object sender, EventArgs e)
        {
            if (tbLoginUsername.Text != string.Empty && tbPassword.Text != string.Empty)
            {
                User loginUser = new User(tbLoginUsername.Text.ToLower(), tbPassword.Text);
                Login(loginUser);
            }
            else
            {
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Gebruikersnaam of wachtwoord niet ingevuld.";
            }
        }

        public void ChangeVideo(Video video)
        {
            Page.Title = CurrentVideo.Title;
            lblTitle.Text = CurrentVideo.Title;
            lblUploaderName.Text = CurrentVideo.Uploader;
            lblDescription.Text = CurrentVideo.Description;
            lblDatePosted.Text = CurrentVideo.UploadDate;
            lblViews.Text = Convert.ToString(CurrentVideo.Views);
            BtnLikes.Text = Convert.ToString(CurrentVideo.Likes);
            BtnDislikes.Text = Convert.ToString(CurrentVideo.DisLikes);
        }

        public void Login(User loginUser)
        {
            lblErrorMessages.Visible = true;
            if (databaseManager.Authenticate(loginUser))
            {
                lblUserloginName.Visible = false;
                lblPassword.Visible = false;
                tbLoginUsername.Visible = false;
                tbPassword.Visible = false;
                BtnLogIn.Visible = false;
                BtnRegister.Visible = false;
                tbAddComment.Visible = true;
                BtnAddComment.Visible = true;
                lblCommentInfo.Visible = false;
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Black;
                lblErrorMessages.Text = "Succesvol ingelogd als "+loginUser.Username+".";
                CurrentUser = loginUser;
                Session["Username"] = CurrentUser.Username;
                Session["Password"] = CurrentUser.Password;
                if (CurrentVideo.Uploader == CurrentUser.Username)
                {
                    BtnDeleteVideo.Visible = true;
                }
                //Session["User"] = CurrentUser;
            }
            else
            {
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Foute gebruikersnaam of wachtwoord.";
            }
        }

        protected void BtnLikes_Click(object sender, EventArgs e)
        {
            CurrentVideo.LikeVideo(true);
            BtnLikes.Text = Convert.ToString(CurrentVideo.Likes);
            if (!databaseManager.VideoLike(CurrentVideo,true))
            {
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Like niet toegevoegd, fout in de verbinding.";
            }
        }

        protected void BtnDislikes_Click(object sender, EventArgs e)
        {
            CurrentVideo.LikeVideo(false);
            BtnDislikes.Text = Convert.ToString(CurrentVideo.DisLikes);
            if (!databaseManager.VideoLike(CurrentVideo, false))
            {
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Dislike niet toegevoegd, fout in de verbinding.";
            }
        }

        protected void BtnRegister_Click(object sender, EventArgs e)
        {
            if (tbLoginUsername.Text != string.Empty && tbPassword.Text != string.Empty)
            {
                User newuser = new User(tbLoginUsername.Text.ToLower(), tbPassword.Text);
                if (databaseManager.AddUser(newuser))
                {
                    Login(newuser);
                }
                else
                {
                    lblErrorMessages.Visible = true;
                    lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                    lblErrorMessages.Text = "Gebruiker niet toegevoegd, deze bestaat mogelijk al.";
                }
            }
            else
            {
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Gebruikersnaam of wachtwoord niet ingevuld.";
            }
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            if (tbSearchBar.Text != string.Empty)
            {
                string search = tbSearchBar.Text.Replace(" ","+").ToLower();
                string url = "SearchREsults.aspx?search="+search;
                Response.Redirect(url);
            }
        }

        protected void BtnAddComment_Click(object sender, EventArgs e)
        {
            if (tbAddComment.Text != string.Empty)
            {
                int commentID = Comments.Count() + 1;
                Comment newcomment = new Comment(commentID,CurrentVideo, tbAddComment.Text, CurrentUser);
                databaseManager.AddComment(CurrentVideo, newcomment);
                AddComments();
            }
        }

        public void AddComments()
        {
            PnlComments.Controls.Clear();
            Comments = databaseManager.GetComments(CurrentVideo);
            foreach (Comment c in Comments)
            {
                Label lblComment = new Label();
                lblComment.Text = c.Text;
                lblComment.ID = "CommentTxt" + c.CommentID;
                Label lblCommentPoster = new Label();
                lblCommentPoster.Text = " --  posted by " + c.Poster.Username;
                lblCommentPoster.ID = "CommentPoster" + c.CommentID;
                lblCommentPoster.Font.Size = FontUnit.Small;
                PnlComments.Controls.Add(lblComment);
                PnlComments.Controls.Add(lblCommentPoster);
                PnlComments.Controls.Add(new LiteralControl("<br />"));
            }
        }

        protected void BtnDeleteVideo_Click(object sender, EventArgs e)
        {
            if (BtnDeleteVideo.Text == "Verwijder video")
            {
                BtnDeleteVideo.Text = "Weet je het zeker?";
            }
            else if (BtnDeleteVideo.Text == "Weet je het zeker?")
            {
                if (databaseManager.DeleteVideo(CurrentVideo))
                {
                    lblErrorMessages.Visible = true;
                    lblErrorMessages.ForeColor = System.Drawing.Color.Black;
                    lblErrorMessages.Text = "Video succesvol verwijderd";
                }
                else
                {
                    lblErrorMessages.Visible = true;
                    lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                    lblErrorMessages.Text = "Video niet verwijderd";
                }
            }
        }
    }
}