// <copyright file="PlainComment.cs" company="Kai Corstjens">
//     Copyright (c) Individuele opdracht Kai Corstjens. All rights reserved.
// </copyright>
namespace Youtube.ServiceLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    public class PlainComment
    {
        public string Text { get; set; }
        public string Poster { get; set; }
        public Comment OriginalComment { get; set;}

        public PlainComment(Comment comment)
        {
            this.Text = comment.Text;
            this.Poster = comment.Poster.Username;
            this.OriginalComment = comment;
        }
    }
}