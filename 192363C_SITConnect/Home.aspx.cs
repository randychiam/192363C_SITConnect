using System;

namespace _192363C_SITConnect
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    btnLogout.Visible = true;
                }
            }

            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Session.Clear() and Session.Abandon() removes the session variables but keep the current session in memory

            // Session.RemoveAll() ends the current session
            Session.Clear();

            Session.Abandon();

            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            // Expire ASP.NET_SessionId cookie
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;

                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            // Expire AuthToken cookie
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;

                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }
    }
}