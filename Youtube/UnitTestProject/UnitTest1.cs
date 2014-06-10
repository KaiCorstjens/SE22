// <copyright file="UnitTest1.cs" company="Kai Corstjens">
//     Copyright (c) Individuele opdracht Kai Corstjens. All rights reserved.
// </copyright>
namespace UnitTestProject
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Youtube.DataAccess;
    using Youtube.ServiceLayer;
    [TestClass]
    public class UnitTest1
    {

        DatabaseManager databasemanager = new DatabaseManager();
        [TestMethod]
        public void TestGoodAuthentication()
        {
            User user = new User("kai","hallo123");
            Assert.AreEqual(true, databasemanager.Authenticate(user));
        }
        [TestMethod]
        public void TestBadAuthentication()
        {
            User user = new User("kai", "fout");
            Assert.AreEqual(false, databasemanager.Authenticate(user));
        }
        [TestMethod]
        public void TestGetPlaylist()
        {
            bool testPassed = false;
            if (databasemanager.GetAllPlaylists().Count >0)
            {
                testPassed = true;
            }
            Assert.AreEqual(true, testPassed);

        }
        [TestMethod]
        public void TestAddLike()
        {
            Video video = new Video(8,string.Empty,string.Empty,string.Empty,false,null,string.Empty);
            Stream stream = new MemoryStream();
            Assert.AreEqual(true, databasemanager.VideoLike(video, true));
        }
        [TestMethod]
        public void TestAddDisLike()
        {
            Video video = new Video(8, string.Empty, string.Empty, string.Empty, false, null, string.Empty);
            Stream stream = new MemoryStream();
            Assert.AreEqual(true, databasemanager.VideoLike(video, false));
        }
    }
}
