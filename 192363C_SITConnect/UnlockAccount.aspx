<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnlockAccount.aspx.cs" Inherits="_192363C_SITConnect.UnlockAccount" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SITConnect | Unlock Account</title>
</head>
<body style="font-family: Arial;">
    <form id="form1" runat="server">
        <h1 style="text-align: center;">SITConnect</h1>
        <div style="border-radius: 5px; background-color: #EBECF0; padding: 20px">
            <asp:Label ID="lbStatus" runat="server"></asp:Label>
            <h3>Unlock Account</h3>
            <p>Email: </p>
            <asp:TextBox ID="tbUserId" TextMode="Email" runat="server"></asp:TextBox>
            <div style="padding-bottom: 20px;">
                <br />
                <asp:Button ID="btnUnlock" Text="Send Email" runat="server" OnClick="btnUnlock_Click"/>
            </div>
        </div>
    </form>
</body>
</html>
