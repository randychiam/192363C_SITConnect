using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace _192363C_SITConnect
{
    public partial class UnlockAccount : System.Web.UI.Page
    {
        string SITConnectDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected string getEmail(string userid)
        {
            string s = null;

            SqlConnection con = new SqlConnection(SITConnectDBConnectionString);

            string sql = "select Email FROM ACCOUNT WHERE Email=@USERID";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@USERID", userid);

            try
            {
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != null)
                        {
                            if (reader["Email"] != DBNull.Value)
                            {
                                s = reader["Email"].ToString();
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { con.Close(); }

            return s;
        }

        protected string getFname(string userid)
        {
            string s = null;

            SqlConnection con = new SqlConnection(SITConnectDBConnectionString);

            string sql = "select Fname FROM ACCOUNT WHERE Email=@USERID";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@USERID", userid);

            try
            {
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Fname"] != null)
                        {
                            if (reader["Fname"] != DBNull.Value)
                            {
                                s = reader["Fname"].ToString();
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { con.Close(); }

            return s;
        }

        protected string getLname(string userid)
        {
            string s = null;

            SqlConnection con = new SqlConnection(SITConnectDBConnectionString);

            string sql = "select Lname FROM ACCOUNT WHERE Email=@USERID";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@USERID", userid);

            try
            {
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Lname"] != null)
                        {
                            if (reader["Lname"] != DBNull.Value)
                            {
                                s = reader["Lname"].ToString();
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { con.Close(); }

            return s;
        }

        protected async Task Execute()
        {
            string fname = getFname(tbUserId.Text.Trim());

            string lname = getLname(tbUserId.Text.Trim());

            string guid = Guid.NewGuid().ToString();

            Session["ResetToken"] = guid;

            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("192363C@mymail.nyp.edu.sg", "SITConnect Support"),
                Subject = "Unlock Account",
                HtmlContent = "<p>Click <a href='https://localhost:44325/ResetStatus.aspx'>here</a> to unlock your account.</p>"
            };

            msg.AddTo(new EmailAddress(tbUserId.Text.Trim(), fname + " " + lname));

            _ = await client.SendEmailAsync(msg);
        }

        protected void btnUnlock_Click(object sender, EventArgs e)
        {
            string email = tbUserId.Text.ToString().Trim();

            string db_email = getEmail(email);

            if (String.IsNullOrEmpty(tbUserId.Text.ToString().Trim()))
            {
                lbStatus.ForeColor = Color.Red;

                lbStatus.Text = "Please enter your email.";
            }

            else if (!email.Equals(db_email))
            {
                lbStatus.ForeColor = Color.Red;

                lbStatus.Text = "Email is not registered to any account.";
            }

            else
            {
                Session["RequestUnlock"] = tbUserId.Text.ToString().Trim();

                Execute().ConfigureAwait(false);

                Response.Redirect("Login.aspx", false);
            }
        }
    }
}