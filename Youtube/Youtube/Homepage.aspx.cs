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

    public partial class Homepage : System.Web.UI.Page
    {
        private DatabaseManager databaseManager;
        public List<Video> Videos {get; set;}
        public User CurrentUser { get; private set; }
        public Video CurrentVideo { get; private set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (databaseManager == null)
            {
                databaseManager = new DatabaseManager();
            }
            if (Videos == null)
            {
                Videos = new List<Video>();
            }
            try
            {
                CurrentUser = (User)Session["User"];
                CurrentVideo = (Video)Session["Video"];

                if (CurrentUser != null)
                {
                    if (databaseManager.Authenticate(CurrentUser))
                    {
                        lblUserloginName.Visible = false;
                        lblPassword.Visible = false;
                        tbLoginUsername.Visible = false;
                        tbPassword.Visible = false;
                        BtnLogIn.Visible = false;
                        lblErrorMessages.Visible = true;
                        lblErrorMessages.ForeColor = System.Drawing.Color.Black;
                        lblErrorMessages.Text = "Succesvol ingelogd.";
                    }
                    else
                    {
                        lblErrorMessages.Visible = true;
                        lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                        lblErrorMessages.Text = "Foute gebruikersnaam of wachtwoord in Session.";
                    }
                }
            }
            catch
            {
                // No session found.
            }
            Videos = databaseManager.GetAllVideos();
            foreach (Video v in Videos)
            {
                if (v.VideoID == 1)
                {
                    if (CurrentVideo == null)
                    {
                        this.CurrentVideo = v;
                    }
                }
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
                User loginUser = new User(tbLoginUsername.Text, tbPassword.Text);
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
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Black;
                lblErrorMessages.Text = "Succesvol ingelogd als "+loginUser.Username+".";
                CurrentUser = loginUser;
                Session["User"] = CurrentUser;
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
                User newuser = new User(tbLoginUsername.Text, tbPassword.Text);
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
    }
}