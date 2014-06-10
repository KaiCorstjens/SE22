// <copyright file="Homepage.aspx.cs" company="Kai Corstjens">
//     Copyright (c) Individuele opdracht Kai Corstjens. All rights reserved.
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
            //lblErrorMessages.Visible = true;
            System.Web.HttpBrowserCapabilities browser = Request.Browser;
            if (browser.Browser != "Firefox")
            {
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Please use Firefox";
            }
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
            if (CurrentVideo != null)
            {
                ChangeVideo(CurrentVideo);
                databaseManager.AddView(CurrentVideo);
                Comments = databaseManager.GetComments(CurrentVideo);
                AddComments();
            }
            if (CurrentVideo != null && CurrentUser != null)
            {
                if (CurrentVideo.Uploader == CurrentUser.Username)
                {
                    BtnDeleteVideo.Visible = true;
                }
                AddComments();
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
            HTMLvideo.Src = "Handler.ashx?videoID=" +CurrentVideo.VideoID;
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
                BtnLogout.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Black;
                lblErrorMessages.Text = "Succesvol ingelogd als "+loginUser.Username+".";
                CurrentUser = loginUser;
                Session["Username"] = CurrentUser.Username;
                Session["Password"] = CurrentUser.Password;
                if (CurrentVideo != null)
                {
                    if (CurrentVideo.Uploader == CurrentUser.Username)
                    {
                        BtnDeleteVideo.Visible = true;
                    }
                }
                AddComments();
                Session["User"] = CurrentUser;
            }
            else
            {
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Foute gebruikersnaam of wachtwoord.";
            }
        }
        public void LogOut()
        {
            CurrentUser = null;
            BtnLogout.Visible = false;

            lblUserloginName.Visible = true;
            lblPassword.Visible = true;
            tbLoginUsername.Visible = true;
            tbPassword.Visible = true;
            BtnLogIn.Visible = true;
            BtnRegister.Visible = true;
            tbAddComment.Visible = false;
            BtnAddComment.Visible = false;
            lblCommentInfo.Visible = true;
            lblErrorMessages.Visible = true;
            lblErrorMessages.ForeColor = System.Drawing.Color.Black;
            lblErrorMessages.Text = "Succesvol uitgelogd.";
            Session["Username"] = string.Empty;
            Session["Password"] = string.Empty;
                BtnDeleteVideo.Visible = false;
            AddComments();
            Session["User"] = null;
        }
        public void AddComments()
        {
            PnlComments.Controls.Clear();
            if (CurrentVideo != null)
            {
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
                    Button btnDeleteComment = new Button();
                    btnDeleteComment.Text = "verwijder reactie";
                    btnDeleteComment.ID = "BtnDelete" + c.CommentID;
                    PnlComments.Controls.Add(lblComment);
                    PnlComments.Controls.Add(lblCommentPoster);
                    btnDeleteComment.Click += new EventHandler(DeleteComment);
                    if (CurrentUser != null && c != null)
                    {
                        if (CurrentUser.Username == c.Poster.Username)
                        {
                            PnlComments.Controls.Add(btnDeleteComment);
                        }
                    }
                    PnlComments.Controls.Add(new LiteralControl("<br />"));
                }
            }
        }
        protected void DeleteComment(object sender, EventArgs e)
        {
            int commentID = 0;
            Button clickedButton = (Button)sender;
            if (int.TryParse(clickedButton.ID.Substring(9), out commentID))
            {
                databaseManager.DeleteComment(commentID);
            }
            AddComments();
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
        protected void BtnLikes_Click(object sender, EventArgs e)
        {
            BtnLikes.Text = Convert.ToString(CurrentVideo.Likes);
            if (!databaseManager.VideoLike(CurrentVideo,true))
            {
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Like niet toegevoegd, fout met de verbinding.";
            }
            Page.Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
        }
        protected void BtnDislikes_Click(object sender, EventArgs e)
        {
            BtnDislikes.Text = Convert.ToString(CurrentVideo.DisLikes);
            if (!databaseManager.VideoLike(CurrentVideo, false))
            {
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Dislike niet toegevoegd, fout met de verbinding.";
            }
            Page.Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
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
                int highestCommentID = 0;
                foreach (Comment c in Comments)
                {
                    if (c.CommentID >= highestCommentID)
                    {
                        highestCommentID = c.CommentID +1;
                    }
                }
                Comment newcomment = new Comment(highestCommentID,CurrentVideo, tbAddComment.Text, CurrentUser);
                lblErrorMessages.Text= databaseManager.AddComment(CurrentVideo, newcomment);
                AddComments();
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

        protected void BtnLogout_Click(object sender, EventArgs e)
        {
            LogOut();
        }
    }
}