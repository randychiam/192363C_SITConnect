<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="_192363C_SITConnect.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SITConnect | Login</title>
    <script src="https://www.google.com/recaptcha/api.js?render=6Le2JzcaAAAAAHaeBWUmWy7rMKPvR_PxBTvFrxhP"></script>
</head>
<body style="font-family: Arial;">
    <form id="form1" runat="server">
        <h1 style="text-align: center;">SITConnect</h1>
        <div style="border-radius: 5px; background-color: #EBECF0; padding: 20px">
            <asp:Label ID="lbStatus" runat="server"></asp:Label>
            <h3>Login</h3>
            <p>Email: </p>
            <asp:TextBox ID="tbUserId" TextMode="Email" runat="server"></asp:TextBox>
            <p>Password: </p>
            <asp:TextBox ID="tbPassword" TextMode="Password" runat="server"></asp:TextBox>
            <div style="padding-bottom: 20px;">
                <br />
                <asp:Button ID="btnLogin" Text="Login" runat="server" OnClick="btnLogin_Click" />
            </div>
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
        </div>
    </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6Le2JzcaAAAAAHaeBWUmWy7rMKPvR_PxBTvFrxhP', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
</html>
