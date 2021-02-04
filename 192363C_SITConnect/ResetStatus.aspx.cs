using System;
using System.Data.SqlClient;

namespace _192363C_SITConnect
{
    public partial class ResetStatus : System.Web.UI.Page
    {

        string SITConnectDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["RequestUnlock"] != null && Session["ResetToken"] != null)
            {
                SqlConnection con = new SqlConnection(SITConnectDBConnectionString);

                // Reset account status to 1 (Not locked out)
                string ResetAccountStatus = "update Account set Status = @Status where Email = @Email";

                SqlCommand ResetAccountStatusCmd = new SqlCommand(ResetAccountStatus, con);

                ResetAccountStatusCmd.Parameters.AddWithValue("@Status", "1");

                ResetAccountStatusCmd.Parameters.AddWithValue("@Email", Session["RequestUnlock"].ToString());

                con.Open();

                ResetAccountStatusCmd.ExecuteNonQuery();

                con.Close();

                // Reset login attempts to 0
                string ResetLoginAttempt = "update Account set LoginAttempt = @LoginAttempt where Email = @Email";

                SqlCommand ResetLoginAttemptCmd = new SqlCommand(ResetLoginAttempt, con);

                ResetLoginAttemptCmd.Parameters.AddWithValue("@LoginAttempt", 0);

                ResetLoginAttemptCmd.Parameters.AddWithValue("@Email", Session["RequestUnlock"].ToString());

                con.Open();

                ResetLoginAttemptCmd.ExecuteNonQuery();

                con.Close();

                Session.Remove("RequestUnlock");

                Session.Remove("ResetToken");

                Response.Redirect("Login.aspx", false);
            }

            else
            {
                Response.Redirect("403.html", false);
            }
        }
    }
}