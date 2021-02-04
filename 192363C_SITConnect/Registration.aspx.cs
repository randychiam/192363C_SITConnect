using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace _192363C_SITConnect
{
    public partial class Registration : System.Web.UI.Page
    {
        string SITConnectDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectDBConnection"].ConnectionString;

        static string finalHash;

        static string salt;

        byte[] Key;

        byte[] IV;

        // Keep password value after postback
        protected void Page_Load(object sender, EventArgs e)
        {
            string pwd = tbPassword.Text;

            tbPassword.Attributes.Add("value", pwd);
        }

        // Uses parameterized query to prevent SQL Injection
        public void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITConnectDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@FirstName, @LastName, @CreditCardInfo, @Email, " +
                        "@PasswordHash, @PasswordSalt, @DateOfBirth, @Status, @LoginAttempt)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.AddWithValue("@FirstName", tbFname.Text.Trim());

                            cmd.Parameters.AddWithValue("@LastName", tbLname.Text.Trim());

                            // Only credit card number is stored. Expiry date and CVV has to be entered by the user to authorize any payment process.
                            cmd.Parameters.AddWithValue("@CreditCardInfo", encryptData(tbCardInfo.Text.Trim()));

                            cmd.Parameters.AddWithValue("@Email", tbEmail.Text.Trim());

                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);

                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);

                            string dob = tbDob.Text;

                            //Parse date of birth independent of user's local settings
                            cmd.Parameters.AddWithValue("@DateOfBirth", DateTime.ParseExact(dob, "d/M/yyyy", CultureInfo.InvariantCulture));

                            cmd.Parameters.AddWithValue("@Status", "1");

                            cmd.Parameters.AddWithValue("@LoginAttempt", 0);

                            cmd.Connection = con;

                            con.Open();

                            cmd.ExecuteNonQuery();

                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
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

        // Encrypt data using the RjindaelManaged class (symmetric algorithm)
        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();

                cipher.IV = IV;

                cipher.Key = Key;

                ICryptoTransform encryptTransform = cipher.CreateEncryptor();

                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                byte[] plainText = Encoding.UTF8.GetBytes(data);

                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { }

            return cipherText;
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

        // Check password strength based on score obtained from password validation
        protected void btnPasswordStrength_Click(object sender, EventArgs e)
        {
            int scores = validatePassword(tbPassword.Text);

            string pwd_strength = "";

            switch (scores)
            {
                case 1:
                    pwd_strength = "Password is very weak";

                    break;

                case 2:
                    pwd_strength = "Password is weak";

                    break;

                case 3:
                    pwd_strength = "Password is medium";

                    break;

                case 4:
                    pwd_strength = "Password is strong";

                    break;

                case 5:
                    pwd_strength = "Password is excellent";

                    break;

                default:

                    break;
            }

            lblPasswordStrength.Text = pwd_strength;

            if (scores < 4)
            {
                lblPasswordStrength.ForeColor = Color.Red;

                return;
            }

            lblPasswordStrength.ForeColor = Color.Green;
        }

        // Register 
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string email = tbEmail.Text.ToString().Trim();

            string db_email = getEmail(email);

            int score = validatePassword(tbPassword.Text);

            if (String.IsNullOrEmpty(tbFname.Text.ToString().Trim()))
            {
                lbStatus.ForeColor = Color.Red;

                lbStatus.Text = "Please enter your first name.";
            }

            else if (String.IsNullOrEmpty(tbLname.Text.ToString().Trim()))
            {
                lbStatus.ForeColor = Color.Red;

                lbStatus.Text = "Please enter your last name.";
            }

            else if (String.IsNullOrEmpty(tbCardInfo.Text.ToString().Trim()))
            {
                lbStatus.ForeColor = Color.Red;

                lbStatus.Text = "Please enter your credit card number.";
            }

            else if (!(tbCardInfo.Text.ToString().Trim().Length == 16))
            {
                lbStatus.ForeColor = Color.Red;

                lbStatus.Text = "Credit card number must be 16 digits long.";
            }

            else if (String.IsNullOrEmpty(tbEmail.Text.ToString().Trim()))
            {
                lbStatus.ForeColor = Color.Red;

                lbStatus.Text = "Please enter your email.";
            }

            else if (email.Equals(db_email))
            {
                lbStatus.ForeColor = Color.Red;

                lbStatus.Text = "Email is already registered. Click <a href='https://localhost:44325/Login.aspx'>here</a> to login.";
            }

            else if (String.IsNullOrEmpty(tbPassword.Text.ToString().Trim()))
            {
                lbStatus.ForeColor = Color.Red;

                lbStatus.Text = "Please enter a password.";
            }

            else if (String.IsNullOrEmpty(tbDob.Text.ToString().Trim()))
            {
                lbStatus.ForeColor = Color.Red;

                lbStatus.Text = "Please enter your date of birth.";
            }

            else if (score < 5)
            {
                lbStatus.ForeColor = Color.Red;

                lbStatus.Text = "Your password must be at least 8 characters long and contain 1 uppercase, lowercase and special character and 1 number.";
            }

            else
            {
                string pwd = tbPassword.Text.ToString().Trim();

                // Generate random salt 
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                byte[] saltByte = new byte[8];

                // Fills array of bytes with a cryptographically strong sequence of random values.
                rng.GetBytes(saltByte);

                salt = Convert.ToBase64String(saltByte);

                SHA512Managed hashing = new SHA512Managed();

                string pwdWithSalt = pwd + salt;

                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));

                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                finalHash = Convert.ToBase64String(hashWithSalt);

                RijndaelManaged cipher = new RijndaelManaged();

                cipher.GenerateKey();

                Key = cipher.Key;

                IV = cipher.IV;

                createAccount();

                Response.Redirect("Login.aspx", false);
            }
        }

    }
}