<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="_192363C_SITConnect.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SITConnect | Register</title>
    <style>
        input[type=number]::-webkit-inner-spin-button,
        input[type=number]::-webkit-outer-spin-button {
            -webkit-appearance: none;
            -moz-appearance: none;
            margin: 0;
        }
    </style>
    <script type="text/javascript">
        function validate_password() {

            var pwd = document.getElementById('<%=tbPassword.ClientID %>').value;

            if (pwd.length < 8) {
                document.getElementById("lblPassword").innerHTML = "Password must be at least 8 characters";
                document.getElementById("lblPassword").style.color = "Red";
                return ("too_short");
            }

            else if (pwd.search(/[0-9]/) == -1) {
                document.getElementById("lblPassword").innerHTML = "Password must contain at least 1 number";
                document.getElementById("lblPassword").style.color = "Red";
                return ("no_number");
            }

            else if (pwd.search(/[a-z]/) == -1) {
                document.getElementById("lblPassword").innerHTML = "Password must contain at least 1 lowercase character";
                document.getElementById("lblPassword").style.color = "Red";
                return ("no_lowercase");
            }

            else if (pwd.search(/[A-Z]/) == -1) {
                document.getElementById("lblPassword").innerHTML = "Password must contain at least 1 uppercase character";
                document.getElementById("lblPassword").style.color = "Red";
                return ("no_uppercase");
            }

            else if (pwd.search(/[!@#$%^&*()_+]/) == -1) {
                document.getElementById("lblPassword").innerHTML = "Password must contain at least 1 special character";
                document.getElementById("lblPassword").style.color = "Red";
                return ("no_special");
            }

            document.getElementById("lblPassword").innerHTML = "Password OK";
            document.getElementById("lblPassword").style.color = "Green";
        }
    </script>
</head>
<body style="font-family: Arial;">
    <div>
        <form id="form1" runat="server">
            <h1 style="text-align: center;">SITConnect</h1>
            <div style="border-radius: 5px; background-color: #EBECF0; padding: 20px">
                <asp:Label ID="lbStatus" runat="server"></asp:Label>
                <h3>Register</h3>
                <p>First Name: </p>
                <asp:TextBox ID="tbFname" runat="server"></asp:TextBox>
                <p>Last Name: </p>
                <asp:TextBox ID="tbLname" runat="server"></asp:TextBox>
                <p>Credit Card Info: </p>
                <asp:TextBox ID="tbCardInfo" runat="server" TextMode="Number"></asp:TextBox>
                <p>Email Address: </p>
                <asp:TextBox ID="tbEmail" TextMode="Email" runat="server"></asp:TextBox>
                <p>Password: </p>
                <asp:TextBox ID="tbPassword" TextMode="Password" OnKeyUp="javascript:validate_password()" runat="server"></asp:TextBox>
                <asp:Label ID="lblPassword" runat="server"></asp:Label><br />
                <br />
                <asp:Button ID="btnPasswordStrength" Text="Check Password Strength" runat="server" OnClick="btnPasswordStrength_Click" />
                <asp:Label ID="lblPasswordStrength" runat="server"></asp:Label>
                <p>Date of Birth: </p>
                <asp:TextBox ID="tbDob" runat="server"></asp:TextBox>
                <div style="padding-bottom: 20px;">
                    <br />
                    <asp:Button ID="btnRegister" Text="Register" runat="server" OnClick="btnRegister_Click" />
                </div>
            </div>
        </form>
    </div>
</body>
</html>
