using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace _192363C_SITConnect
{
    public partial class Login : System.Web.UI.Page
    {
        string SITConnectDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public class MyObject
        {
            public string success { get; set; }

            public List<string> ErrorMessage { get; set; }
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

        // Retrieve account status as string from database 
        protected string getAccountStatus(string userid)
        {
            string s = null;

            SqlConnection con = new SqlConnection(SITConnectDBConnectionString);

            string sql = "select Status FROM ACCOUNT WHERE Email=@USERID";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@USERID", userid);

            try
            {
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Status"] != null)
                        {
                            if (reader["Status"] != DBNull.Value)
                            {
                                s = reader["Status"].ToString();
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

        // Retrieve login attempts as string from database 
        protected int getLoginAttempts(string userid)
        {
            int s = 0;

            SqlConnection con = new SqlConnection(SITConnectDBConnectionString);

            string sql = "select LoginAttempt FROM ACCOUNT WHERE Email=@USERID";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@USERID", userid);

            try
            {
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LoginAttempt"] != null)
                        {
                            if (reader["LoginAttempt"] != DBNull.Value)
                            {
                                s = Convert.ToInt32(reader["LoginAttempt"]);
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

        // Google reCaptcha v3 service implementation
        public bool validateCaptcha()
        {
            bool result = true;

            // User gets response POST parameter after submitting the captcha form
            string captchaResponse = Request.Form["g-recaptcha-response"];

            // Sends a GET request to Google with secret key and response
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                ("https://www.google.com/recaptcha/api/siteverify?secret=6Le2JzcaAAAAAOVXFnDv_lNIQZOP4XIMFUgO5rth &response=" + captchaResponse);

            try
            {
                using (WebResponse res = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(res.GetResponseStream()))
                    {
                        // JSON response
                        string jsonRes = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        // jsonObject handles the response and JSON is deserialized
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonRes);

                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }

                return result;
            }

            catch (WebException ex)
            {
                throw ex;
            }
        }

        // Login
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // HtmlEncode to prevent XSS attacks however, ValidateRequest in Login.aspx is not set to false for security purposes
            string pwd = HttpUtility.HtmlEncode(tbPassword.Text.ToString().Trim());

            string user_id = HttpUtility.HtmlEncode(tbUserId.Text.ToString().Trim());

            string account_status = getAccountStatus(user_id);

            int login_attempt = getLoginAttempts(user_id);

            // Compute SHA512 Algorithm for input data
            SHA512Managed hashing = new SHA512Managed();

            string dbHash = getDBHash(user_id);

            string dbSalt = getDBSalt(user_id);

            SqlConnection con = new SqlConnection(SITConnectDBConnectionString);

            string sql = "select Email from Account where Email = @UserId";

            string db_user_id;

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@UserId", user_id);

            con.Open();

            db_user_id = (string)cmd.ExecuteScalar();

            con.Close();

            try
            {
                // Check if user is a human or robot
                if (validateCaptcha())
                {
                    // Check if user is registered and dbHash and dbSalt is not null or empty
                    if (user_id.Equals(db_user_id) && dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string pwdWithSalt = pwd + dbSalt;

                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                        string userHash = Convert.ToBase64String(hashWithSalt);

                        // Check if password entered matches password in database and if account status is 1 (Not locked out)
                        if (userHash.Equals(dbHash) && account_status == "1")
                        {
                            Session["LoggedIn"] = tbUserId.Text.Trim();

                            // Create a new GUID to save into session
                            string guid = Guid.NewGuid().ToString();

                            Session["AuthToken"] = guid;

                            // Create a new cookie and assigns GUID value to it
                            Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                            // Reset login attempts when user successfully logs in
                            string ResetLoginAttempt = "update Account set LoginAttempt = @LoginAttempt where Email = @Email";

                            SqlCommand ResetLoginAttemptCmd = new SqlCommand(ResetLoginAttempt, con);

                            ResetLoginAttemptCmd.Parameters.AddWithValue("@LoginAttempt", 0);

                            ResetLoginAttemptCmd.Parameters.AddWithValue("@Email", tbUserId.Text);

                            con.Open();

                            ResetLoginAttemptCmd.ExecuteNonQuery();

                            con.Close();

                            // Redirect user to homepage 
                            Response.Redirect("Home.aspx", false);
                        }

                        // If password entered does not matche password in database and if account status is 1 (Not locked out)
                        else if (!userHash.Equals(dbHash) && account_status == "1")
                        {
                            // Increments login attempts when password is wrong 
                            string AddLoginAttempt = "update Account set LoginAttempt = @LoginAttempt where Email = @Email";

                            SqlCommand AddLoginAttemptCmd = new SqlCommand(AddLoginAttempt, con);

                            AddLoginAttemptCmd.Parameters.AddWithValue("@LoginAttempt", login_attempt + 1);

                            AddLoginAttemptCmd.Parameters.AddWithValue("@Email", tbUserId.Text);

                            con.Open();

                            AddLoginAttemptCmd.ExecuteNonQuery();

                            con.Close();

                            // If login attempt exceeds 3
                            if (login_attempt == 2)
                            {
                                // Update account status to 0 (Locked out)
                                string updateStatus = "update Account set Status = @Status where Email = @Email";

                                SqlCommand updateStatusCmd = new SqlCommand(updateStatus, con);

                                updateStatusCmd.Parameters.AddWithValue("@Status", "0");

                                updateStatusCmd.Parameters.AddWithValue("@Email", tbUserId.Text);

                                con.Open();

                                updateStatusCmd.ExecuteNonQuery();

                                con.Close();

                                lbStatus.ForeColor = Color.Red;

                                lbStatus.Text = "Your account is locked due to too many login attempts. Click <a href='https://localhost:44325/UnlockAccount.aspx'>here</a> to unlock your account.";
                            }

                            else
                            {
                                lbStatus.ForeColor = Color.Red;

                                lbStatus.Text = "Userid or password is incorrect. Please try again.";
                            }
                        }

                        // If account status is 0 (Locked out)
                        else
                        {
                            lbStatus.ForeColor = Color.Red;

                            lbStatus.Text = "Your account is locked due to too many login attempts. Click <a href='https://localhost:44325/UnlockAccount.aspx'>here</a> to unlock your account.";
                        }
                    }

                    // If user is not registered
                    else if (!user_id.Equals(db_user_id))
                    {
                        Response.Redirect("Registration.aspx", false);
                    }

                    else
                    {
                        // Increments login attempts when password is wrong 
                        string AddLoginAttempt = "update Account set LoginAttempt = @LoginAttempt where Email = @Email";

                        SqlCommand AddLoginAttemptCmd = new SqlCommand(AddLoginAttempt, con);

                        AddLoginAttemptCmd.Parameters.AddWithValue("@LoginAttempt", login_attempt + 1);

                        AddLoginAttemptCmd.Parameters.AddWithValue("@Email", tbUserId.Text);

                        con.Open();

                        AddLoginAttemptCmd.ExecuteNonQuery();

                        con.Close();

                        if (login_attempt == 2)
                        {
                            // Update account status to 0 (Locked out)
                            string updateStatus = "update Account set Status = @Status where Email = @Email";

                            SqlCommand updateStatusCmd = new SqlCommand(updateStatus, con);

                            updateStatusCmd.Parameters.AddWithValue("@Status", "0");

                            updateStatusCmd.Parameters.AddWithValue("@Email", tbUserId.Text);

                            con.Open();

                            updateStatusCmd.ExecuteNonQuery();

                            con.Close();

                            lbStatus.ForeColor = Color.Red;

                            lbStatus.Text = "Your account is locked due to too many login attempts. Click <a href='https://localhost:44325/UnlockAccount.aspx'>here</a> to unlock your account.";
                        }

                        else
                        {
                            lbStatus.ForeColor = Color.Red;

                            lbStatus.Text = "Userid or password is incorrect. Please try again.";
                        }
                    }
                }

                // Captcha fail message
                else
                {
                    lbStatus.ForeColor = Color.Red;

                    lbStatus.Text = "Captcha verification failed.";
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { }
        }

    }
}