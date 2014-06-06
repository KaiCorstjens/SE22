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
                lblErrorMessages.Visible = true;
                lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                lblErrorMessages.Text = "Je bent niet ingelogd, ga terug naar de homepage.";
            }
        }

        protected void BtnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                string uploadFolder = @"C:\SE22\KaiCorstjens\Videos\";
                string fileLocation = uploadFolder + fileUpload.FileName;
                string newFileLocation;
                int videoID = 0;
                List<Video> videoList = databaseManager.GetAllVideos();
                int highestVideoID = 0;
                foreach (Video v in videoList)
                {
                    if (v.VideoID > highestVideoID)
                    {
                        highestVideoID = v.VideoID + 1;
                    }
                }
                // Check if file already exists. If it exists, add a number to it.
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                if (File.Exists(fileLocation))
                {
                    for (int i = 0; i <= 999; i++)
                    {
                        int locationOfExtension = fileLocation.LastIndexOf('.');
                        string locationNoExtension = fileLocation.Substring(0, locationOfExtension);
                        string extension = fileLocation.Substring(locationOfExtension);
                        newFileLocation = locationNoExtension + i + extension;
                        if (!File.Exists(newFileLocation))
                        {
                            fileLocation = newFileLocation;
                            i = 1000;
                        }
                    }
                }
                fileUpload.SaveAs(fileLocation);
                Video newVideo = new Video(videoID,tbTitle.Text, currentUser.Username, tbDescription.Text,0,0,0,cbPrivé.Checked ,fileLocation);
                if (databaseManager.AddVideo(newVideo))
                {
                    lblErrorMessages.Visible = true;
                    lblErrorMessages.ForeColor = System.Drawing.Color.Black;
                    lblErrorMessages.Text = "Video succes toegevoegd.";
                }
                else
                {
                    lblErrorMessages.Visible = true;
                    lblErrorMessages.ForeColor = System.Drawing.Color.Red;
                    lblErrorMessages.Text = "Video niet toegevoegd.";
                }
            }
        }

        protected void BtnBack_Click(object sender, EventArgs e)
        {
            string url = "Homepage.aspx";
            Response.Redirect(url);
        }
    }
}