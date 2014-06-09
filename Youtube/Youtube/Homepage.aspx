<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="Youtube.Homepage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="tbSearchBar" runat="server" Width="881px"></asp:TextBox>
            <asp:Button ID="BtnSearch" runat="server" Text="Search" OnClick="BtnSearch_Click" />
            <asp:Button ID="BtnUpload" runat="server" OnClick="BtnUpload_Click" Text="Upload" />
            <br />
            <asp:Label ID="lblUserloginName" runat="server" Text="Username"></asp:Label>
            <asp:TextBox ID="tbLoginUsername" runat="server"></asp:TextBox>
            <asp:Label ID="lblPassword" runat="server" Text="Password:"></asp:Label>
            <asp:TextBox ID="tbPassword" runat="server" TextMode="Password"></asp:TextBox>
            <asp:Button ID="BtnLogIn" runat="server" OnClick="BtnLogIn_Click" Text="Log in" />
            <asp:Button ID="BtnRegister" runat="server" OnClick="BtnRegister_Click" Text="Register" style="width: 70px" />
            <asp:Button ID="BtnLogout" runat="server" OnClick="BtnLogout_Click" Text="Uitloggen" Visible="False" />
            <asp:Label ID="lblErrorMessages" runat="server" Font-Bold="True" Text="ErrorMessages" Visible="False"></asp:Label>
            <br />
                <div id="videoPlayer">
                    <iframe src="Video/TestVideo.mp4" id="VideoSource" runat="server" width ="420" height="315"></iframe> 
                </div>
            <asp:Button ID="BtnDeleteVideo" runat="server" Text="Verwijder video" OnClick="BtnDeleteVideo_Click" Visible="False"/>
            <br />
            <asp:Label ID="lblTitle" runat="server" Text="Title" Font-Bold="True" Font-Size="X-Large"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="lblViews" runat="server" Text="Views"></asp:Label>
            <br />
            &nbsp;&nbsp;<asp:Label ID="lblLikesDisLikes" runat="server" Text="Likes/Dislikes"></asp:Label>
&nbsp;<asp:Button ID="BtnLikes" runat="server" Text="Likes" BackColor="#009933" OnClick="BtnLikes_Click" />
            &nbsp;<asp:Button ID="BtnDislikes" runat="server" Text="Dislikes" BackColor="#FF3300" OnClick="BtnDislikes_Click" />
            <br />
            <asp:Label ID="lblDatePosted" runat="server" Text="DatePosted"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="lblUploaderName" runat="server" Text="Username" Font-Bold="True"></asp:Label>
            <br />
            <asp:Label ID="lblDescription" runat="server" Text="Description" Font-Bold="True" Font-Italic="True"></asp:Label>
            <br />
                <div id="CommentArea">
                    <asp:TextBox ID="tbAddComment" runat="server" Visible="False"></asp:TextBox>
                    <asp:Button ID="BtnAddComment" runat="server" Text="Reageer" OnClick="BtnAddComment_Click" Visible="False" />
                    <asp:Label ID="lblCommentInfo" runat="server" Font-Italic="True" Font-Size="Small" Text="Je moet ingelogd zijn om reactie's te plaatsen"></asp:Label>
                    <br />
                    <asp:Panel ID="PnlComments" runat="server">
                    </asp:Panel>
                </div>
            <br />
        </div>
    </form>
</body>
</html>
