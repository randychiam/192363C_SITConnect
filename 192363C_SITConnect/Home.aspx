<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="_192363C_SITConnect.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SITConnect | Home</title>
</head>
<body style="font-family: Arial;">
    <form id="form1" runat="server">
        <h1 style="text-align: center;">SITConnect</h1>
        <div style="border-radius: 5px; background-color: #EBECF0; padding: 20px">
            <h3>Welcome to SITConnect!</h3>
            <a href="ChangePassword.aspx">Change Password.</a>
            <div style="padding-bottom: 20px;">
                <br />
                <asp:Button ID="btnLogout" Text="Logout" Visible="false" runat="server" OnClick="btnLogout_Click" />
            </div>
        </div>
    </form>
</body>
</html>

