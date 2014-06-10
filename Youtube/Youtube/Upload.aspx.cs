// <copyright file="Upload.aspx.cs" company="Kai Corstjens">
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
    using System.IO;
    using System.Net;
    using System.Text;
    using Youtube.DataAccess;
    using Youtube.ServiceLayer;
    public partial class Upload : System.Web.UI.Page
    {
        private User currentUser;
        private DatabaseManager databaseManager = new DatabaseManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                currentUser = (User)Session["User"];
            }
            catch
            {
            }
            if (currentUser == null)
            {

                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Je bent niet ingelogd, ga terug naar de homepage.";
                BtnUpload.Enabled = false;
            }
        }
        protected void BtnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile && tbTitle.Text != string.Empty)
            {
                string appPath = HttpContext.Current.Request.ApplicationPath;
                string uploadFolder = HttpContext.Current.Request.MapPath(appPath);
                string filename;
                filename = fileUpload.FileName.Replace(" ", "+");
                string fileLocation = uploadFolder+ @"\Video\" + filename;
                int locationOfExtension = fileLocation.LastIndexOf('.');
                string locationNoExtension = fileLocation.Substring(0, locationOfExtension);
                if (fileLocation.Substring(locationOfExtension) == ".mp4")
                {
                    List<Video> videoList = databaseManager.GetAllVideos();
                    int highestVideoID = 0;
                    foreach (Video v in videoList)
                    {
                        if (v.VideoID >= highestVideoID)
                        {
                            highestVideoID = v.VideoID + 1;
                        }
                    }

                    Video newVideo = new Video(highestVideoID, tbTitle.Text, currentUser.Username, tbDescription.Text, 0, 0, 0, cbPrivé.Checked, string.Empty);
                    Stream stream = fileUpload.PostedFile.InputStream;
                    if (databaseManager.AddVideoAsBlob(newVideo,stream))
                    {
                        lblErrorMessages.Visible = true;
                        lblErrorMessages.ForeColor = System.Drawing.Color.Black;
                        lblErrorMessages.Text = "Video met succes toegevoegd.";
                        Page.Title = "Video geüpload";
                    }
                    else
                    {
                        lblErrorMessages.Visible = true;
                        lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                        lblErrorMessages.Text = "Video niet toegevoegd.";
                        Page.Title = "Video niet geüpload";
                    }
                }
                else
                {
                    lblErrorMessages.Visible = true;
                    lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                    lblErrorMessages.Text = "Selecteer een .mp4-bestand.";
                }
            }
            else
            {
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Geen bestand geselecteerd of geen titel ingevoerd.";
            }
        }

        protected void BtnBack_Click(object sender, EventArgs e)
        {
            string url = "Homepage.aspx";
            Response.Redirect(url);
        }
    }
}