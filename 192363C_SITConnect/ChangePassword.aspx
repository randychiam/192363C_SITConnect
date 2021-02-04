<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="_192363C_SITConnect.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SITConnect | Change Password</title>
</head>
<body style="font-family: Arial;">
    <form id="form1" runat="server">
        <h1 style="text-align: center;">SITConnect</h1>
        <div style="border-radius: 5px; background-color: #EBECF0; padding: 20px">
            <asp:Label ID="lbStatus" runat="server"></asp:Label>
            <h3>Change Password</h3>
            <p>Current Password: </p>
            <asp:TextBox ID="tbOldPassword" TextMode="Password" runat="server"></asp:TextBox>
            <p>New Password: </p>
            <asp:TextBox ID="tbNewPassword" TextMode="Password" runat="server"></asp:TextBox>
            <p>Confirm New Password: </p>
            <asp:TextBox ID="tbConfirmPassword" TextMode="Password" runat="server"></asp:TextBox>
            <div style="padding-bottom: 20px;">
                <br />
                <asp:Button ID="btnChangePwd" Text="Change Password" runat="server" OnClick="btnChangePwd_Click"/>
            </div>
        </div>
    </form>
</body>
</html>
