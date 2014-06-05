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
        private DatabaseManager databaseManager = new DatabaseManager();
        private string search; 
        private User currentUser;
        protected void Page_Load(object sender, EventArgs e)
        {
            search = Request.QueryString["search"].Replace("+"," ");
            if (tbSearchBar.Text == string.Empty)
            {
                tbSearchBar.Text = search;
            }
            videos = databaseManager.GetAllVideos();
            selectedVideos = new List<Video>();
            foreach (Video v in videos)
            {
                if (v.Title.ToLower().Contains(search))
                {
                    selectedVideos.Add(v);
                }
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
    }
}