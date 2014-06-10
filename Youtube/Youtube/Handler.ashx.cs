// <copyright file="Handler.aspx.cs" company="Kai Corstjens">
//     Copyright (c) Individuele opdracht Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.IO;

    /// <summary>
    /// Summary description for Handler
    /// </summary>
    public class Handler : IHttpHandler
    {
        private DatabaseManager databaseManager = new DatabaseManager();
        public void ProcessRequest(HttpContext context)
        {
            int id = 6;

            if (context.Request.QueryString["VideoID"] == null || int.TryParse(context.Request.QueryString["VideoID"], out id))
            { }
            
              byte[]  video = databaseManager.GetVideoAsBlob(id);
            

            context.Response.ContentType = "video/mp4";
            context.Response.BinaryWrite(video);
            context.Response.End();
            context.Response.Flush();

            }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}