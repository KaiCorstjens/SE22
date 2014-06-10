<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.aspx.cs" Inherits="Youtube.SearchResults" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Resultaten</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:TextBox ID="tbSearchBar" runat="server" Width="881px"></asp:TextBox>
            <asp:Button ID="BtnSearch" runat="server" Text="Search" OnClick="BtnSearch_Click" />
            <asp:Button ID="BtnUpload" runat="server" OnClick="BtnUpload_Click" Text="Upload" />
            <asp:Button ID="BtnHomepage" runat="server" OnClick="BtnHomepage_Click" Text="Homepage" />
            <br />
            <asp:Label ID="lblUserloginName" runat="server" Text="Username"></asp:Label>
            <asp:TextBox ID="tbLoginUsername" runat="server"></asp:TextBox>
            <asp:Label ID="lblPassword" runat="server" Text="Password:"></asp:Label>
            <asp:TextBox ID="tbPassword" runat="server" TextMode="Password"></asp:TextBox>
            <asp:Button ID="BtnLogIn" runat="server" OnClick="BtnLogIn_Click" Text="Log in" />
            <asp:Button ID="BtnRegister" runat="server" OnClick="BtnRegister_Click" Text="Register" />
            <asp:Label ID="lblErrorMessages" runat="server" Font-Bold="True" Text="ErrorMessages" Visible="False"></asp:Label>
        <br />
        <div id="SearchResults">
            <asp:Panel ID="PnlSearchResults" runat="server">
            </asp:Panel>
        </div>
    </div>
    </form>
</body>
</html>
