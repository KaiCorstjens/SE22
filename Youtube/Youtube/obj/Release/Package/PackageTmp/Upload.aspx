<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="Youtube.Upload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Video uploaden</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="lblUpload" runat="server" Font-Bold="True" Font-Size="XX-Large" Text="Upload"></asp:Label>
        <asp:Button ID="BtnBack" runat="server" Text="Terug" OnClick="BtnBack_Click" />
        <br />
        <table>
            <tr>
                <td>
                    <asp:Label ID="lblTitle" runat="server" Text="Titel"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbTitle" runat="server" DefaultButton="BtnUpload"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblFile" runat="server" Text="Bestand"></asp:Label>
                </td>
                <td>
                    <asp:FileUpload ID="fileUpload" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblDescription" runat="server" Text="Omschrijving"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbDescription" runat="server" DefaultButton="BtnUpload"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblPrivate" runat="server" Text="Privé"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="cbPrivé" runat="server" />
                </td>
            </tr>
        </table>
            <asp:Button ID="BtnUpload" runat="server" OnClick="BtnUpload_Click" Text="Upload!" />
        <asp:Label ID="lblErrorMessages" runat="server" Text="ErrorMessages" Visible="False"></asp:Label>
            <br />
    </div>
    </form>
</body>
</html>
