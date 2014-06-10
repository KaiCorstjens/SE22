// <copyright file="SearchResults.aspx.cs" company="Kai Corstjens">
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

    public partial class SearchResults : System.Web.UI.Page
    {
        private List<Video> videos;
        private List<Video> selectedVideos;
        private List<Playlist> playlists;
        private DatabaseManager databaseManager = new DatabaseManager();
        private string search;
        private int playlistID;
        private User currentUser;
        protected void Page_Load(object sender, EventArgs e)
        {
            selectedVideos = new List<Video>();
            playlistID = 0;
            try
            {
                string playlistUser = Request.QueryString["ChooseplaylistUser"];
                int videoID = 0;
                int.TryParse(Request.QueryString["video"], out videoID);
                if (playlistUser != string.Empty)
                {
                    Page.Title = "Afspeellijsten";
                    playlists = databaseManager.GetPlaylists(playlistUser);
                    foreach (Playlist p in playlists)
                    {
                        Label myLabel = new Label();
                        myLabel.Text = p.Name;
                        myLabel.ID = "Label" + p.PlaylistID;
                        Button myButton = new Button();
                        myButton.Text = "Kies";
                        myButton.ID = "Play" + p.PlaylistID+"Vid"+videoID;
                        myButton.Click += new EventHandler(BtnChoosePlaylistClicked);
                        PnlSearchResults.Controls.Add(myLabel);
                        PnlSearchResults.Controls.Add(myButton);
                        PnlSearchResults.Controls.Add(new LiteralControl("<br />"));
                    }
                }
            }
            catch
            {

            }
            try
            {
                string playlistUser = Request.QueryString["playlistUser"];
                if (playlistUser != string.Empty)
                {

                    Page.Title = "Afspeellijsten";
                    playlists = databaseManager.GetPlaylists(playlistUser);
                    foreach (Playlist p in playlists)
                    {
                        Label myLabel = new Label();
                        myLabel.Text = p.Name;
                        myLabel.ID = "Label" + p.PlaylistID;
                        Button myButton = new Button();
                        myButton.Text = "Bekijken";
                        myButton.ID = "Button" + p.PlaylistID;
                        myButton.Click += new EventHandler(BtnPlaylistClicked);
                        PnlSearchResults.Controls.Add(myLabel);
                        PnlSearchResults.Controls.Add(myButton);
                        PnlSearchResults.Controls.Add(new LiteralControl("<br />"));
                    }
                }

            }
            catch
            {
                // No playlist, possibly search
            }
            try
            {
                int.TryParse(Request.QueryString["playlistID"], out playlistID);
                if (playlistID != 0)
                {
                    selectedVideos = databaseManager.GetVideoFromPlaylist(playlistID);
                }

                foreach (Video v in selectedVideos)
                {
                    Label myLabel = new Label();
                    myLabel.Text = v.Title;
                    myLabel.ID = "Label" + v.VideoID;
                    Button myButton = new Button();
                    myButton.Text = "Bekijken";
                    myButton.ID = "Button" + v.VideoID;
                    myButton.Click += new EventHandler(BtnClicked);
                    PnlSearchResults.Controls.Add(myLabel);
                    PnlSearchResults.Controls.Add(myButton);
                    PnlSearchResults.Controls.Add(new LiteralControl("<br />"));
                }
            }
            catch
            {
                // No playlist, possibly search
            }
            try
            {
                search = Request.QueryString["search"].Replace("+", " ");
                if (tbSearchBar.Text == string.Empty)
                {
                    tbSearchBar.Text = search;
                }
                Page.Title = search;
                videos = databaseManager.GetAllVideos();
                foreach (Video v in videos)
                {
                    if (!v.Private)
                    {
                        if (v.Title.ToLower().Contains(search))
                        {
                            selectedVideos.Add(v);
                        }
                    }
                }
                foreach (Video v in selectedVideos)
                {
                    Label myLabel = new Label();
                    myLabel.Text = v.Title;
                    myLabel.ID = "Label" + v.VideoID;
                    Button myButton = new Button();
                    myButton.Text = "Bekijken";
                    myButton.ID = "Button" + v.VideoID;
                    myButton.Click += new EventHandler(BtnClicked);
                    PnlSearchResults.Controls.Add(myLabel);
                    PnlSearchResults.Controls.Add(myButton);
                    PnlSearchResults.Controls.Add(new LiteralControl("<br />"));
                }
            }
            catch
            {
                // No search, possibly playlist
            }
            try
            {
                currentUser = (User)Session["User"];

                if (currentUser != null)
                {
                    Login(currentUser);
                }
            }
            catch
            {

            }
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
                lblErrorMessages.Text = "Succesvol ingelogd als " + loginUser.Username + ".";
                currentUser = loginUser;
                Session["User"] = currentUser;
            }
            else
            {
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Foute gebruikersnaam of wachtwoord.";
            }
        }
        protected void BtnClicked(object sender,EventArgs e)
        {
            int videoID = 0;
            Button clickedButton = (Button)sender;
            if (int.TryParse(clickedButton.ID.Substring(6), out videoID))
            {
                string url = "Homepage.aspx?video=" + videoID;
                Response.Redirect(url);
            }
        }
        protected void BtnPlaylistClicked(object sender, EventArgs e)
        {
            int playlistID = 0;
            Button clickedButton = (Button)sender;
            string plID = clickedButton.ID.ToString();
            if (int.TryParse(clickedButton.ID.Substring(6), out playlistID))
            {
                string url = "SearchResults.aspx?PlaylistID=" + playlistID;
                Response.Redirect(url);
            }
        }

        protected void BtnChoosePlaylistClicked(object sender, EventArgs e)
        {
            int playlistID = 0;
            Button clickedButton = (Button)sender;
            int videoID;
            string buttonID = clickedButton.ID.ToString();
            int beginOfVideo = buttonID.IndexOf("Vid");
            int.TryParse(buttonID.Substring(4,buttonID.Length-3-beginOfVideo), out playlistID);
            int.TryParse(buttonID.Substring(beginOfVideo+3),out videoID);
                databaseManager.AddVideoToPlaylist(playlistID,videoID);
        }
        protected void BtnUpload_Click(object sender, EventArgs e)
        {
            if (currentUser != null)
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
                string search = tbSearchBar.Text.Replace(" ", "+").ToLower();
                string url = "SearchREsults.aspx?search=" + search;
                Response.Redirect(url);
            }
        }

        protected void BtnHomepage_Click(object sender, EventArgs e)
        {
            string url = "Homepage.aspx";
            Response.Redirect(url);
        }
    }
}