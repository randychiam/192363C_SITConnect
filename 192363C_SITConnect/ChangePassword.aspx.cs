using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace _192363C_SITConnect
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string SITConnectDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectDBConnection"].ConnectionString;

        static string finalHash;

        static string salt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("403.html", false);
                }

            }
            else
            {
                Response.Redirect("403.html", false);
            }

        }

        // Incrementing score based on strong password requirements met
        private int validatePassword(string pwd)
        {
            int score = 0;

            if (pwd.Length < 8)
            {
                return 1;
            }

            else
            {
                score = 1;
            }

            if (Regex.IsMatch(pwd, "[a-z]"))
            {
                score++;
            }

            if (Regex.IsMatch(pwd, "[A-Z]"))
            {
                score++;
            }

            if (Regex.IsMatch(pwd, "[0-9]"))
            {
                score++;
            }

            if (Regex.IsMatch(pwd, "[!@#$%^&*()_+]"))
            {
                score++;
            }

            return score;
        }

        // Retrieve password hash as string from database
        protected string getDBHash(string userid)
        {
            string h = null;

            SqlConnection con = new SqlConnection(SITConnectDBConnectionString);

            string sql = "select PasswordHash FROM Account WHERE Email=@USERID";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@USERID", userid);

            try
            {
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
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

            return h;
        }

        // Retrieve password salt as string from database
        protected string getDBSalt(string userid)
        {
            string s = null;

            SqlConnection con = new SqlConnection(SITConnectDBConnectionString);

            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE Email=@USERID";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@USERID", userid);

            try
            {
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
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

        protected void btnChangePwd_Click(object sender, EventArgs e)
        {
            int score = validatePassword(tbNewPassword.Text);

            string pwd = tbOldPassword.Text.ToString().Trim();

            string new_pwd = tbNewPassword.Text.ToString().Trim();

            string confirm_pwd = tbConfirmPassword.Text.ToString().Trim();

            // Compute SHA512 Algorithm for input data
            SHA512Managed hashing = new SHA512Managed();

            string dbHash = getDBHash(Session["LoggedIn"].ToString());

            string dbSalt = getDBSalt(Session["LoggedIn"].ToString());

            if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
            {
                string pwdWithSalt = pwd + dbSalt;

                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                string userHash = Convert.ToBase64String(hashWithSalt);

                if (userHash.Equals(dbHash))
                {
                    if (String.IsNullOrEmpty(tbOldPassword.Text.ToString().Trim()))
                    {
                        lbStatus.ForeColor = Color.Red;

                        lbStatus.Text = "Please enter your current password.";
                    }

                    else if (String.IsNullOrEmpty(tbNewPassword.Text.ToString().Trim()))
                    {
                        lbStatus.ForeColor = Color.Red;

                        lbStatus.Text = "Please enter your new password.";
                    }

                    else if (score < 5)
                    {
                        lbStatus.ForeColor = Color.Red;

                        lbStatus.Text = "Your new password must be at least 8 characters long and contain 1 uppercase, lowercase and special character and 1 number.";
                    }

                    else if (String.IsNullOrEmpty(tbConfirmPassword.Text.ToString().Trim()))
                    {
                        lbStatus.ForeColor = Color.Red;

                        lbStatus.Text = "Please re-enter your new password.";
                    }

                    else
                    {
                        if (new_pwd.Equals(confirm_pwd))
                        {
                            // Generate random salt 
                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                            byte[] saltByte = new byte[8];

                            // Fills array of bytes with a cryptographically strong sequence of random values.
                            rng.GetBytes(saltByte);

                            salt = Convert.ToBase64String(saltByte);

                            string new_pwdWithSalt = new_pwd + salt;

                            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(new_pwd));

                            byte[] new_hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(new_pwdWithSalt));

                            finalHash = Convert.ToBase64String(new_hashWithSalt);

                            SqlConnection con = new SqlConnection(SITConnectDBConnectionString);

                            string UpdatePassword = "update Account set PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt where Email = @Email";

                            SqlCommand UpdatePasswordCmd = new SqlCommand(UpdatePassword, con);

                            UpdatePasswordCmd.Parameters.AddWithValue("@PasswordHash", finalHash);

                            UpdatePasswordCmd.Parameters.AddWithValue("@PasswordSalt", salt);

                            UpdatePasswordCmd.Parameters.AddWithValue("@Email", Session["LoggedIn"].ToString());

                            con.Open();

                            UpdatePasswordCmd.ExecuteNonQuery();

                            con.Close();

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

                        else
                        {
                            lbStatus.ForeColor = Color.Red;

                            lbStatus.Text = "The new password and confirm password does not match. Please try again.";
                        }
                    }
                }

                else
                {
                    lbStatus.ForeColor = Color.Red;

                    lbStatus.Text = "The current password you entered is incorrect. Please try again.";
                }
            }

            else
            {
                Response.Redirect("403.html", false);
            }
        }
    }
}