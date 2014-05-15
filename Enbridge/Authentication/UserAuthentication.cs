using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.Authentication
{
    public static class UserAuthentication
    {
        public static string userCheck(string userName)
        {
            string viewerSettings = "";

            using (SqlConnection conn = new SqlConnection(Enbridge.AppConstants.CONN_STRING_AUTH))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                //comm.CommandText = "Select Count(*) from users where username = '@user




                using (SqlCommand command = new SqlCommand(null, conn))
                {
                    //command.CommandText = "if ((SELECT COUNT(*) from users where USERNAME =  @user) = 0) ";
                    command.CommandText = "DECLARE @maxId INT; ";
                    command.CommandText += "SELECT @maxId = max(userid) FROM users; ";
                    command.CommandText += "Set @maxId = @maxId + 1;";
                    command.CommandText += "if ((SELECT COUNT(*) from users where USERNAME =  @user) = 0) ";
                    command.CommandText += "    INSERT INTO users (userid, USERNAME, Created, Modified) values (@maxId, @user, GETDATE(), GETDATE()); ";
                    command.CommandText += "DECLARE @visits INT; ";
                    command.CommandText += "SELECT @visits = viewerVisits FROM users WHERE USERNAME =  @user; ";
                    command.CommandText += "IF (@visits is NULL) ";
                    command.CommandText += "    Set @visits = 1; ";
                    command.CommandText += "ELSE ";
                    command.CommandText += "    Set @visits = @visits + 1; ";
                    command.CommandText += "Update users set viewerVisits = @visits, lastvisit = GETDATE() where USERNAME =  @user;";                
                    command.Parameters.AddWithValue("@user", userName);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                using (SqlCommand command = new SqlCommand(null, conn))
                {
                    try
                    {
                        command.CommandText = "SELECT viewerSettings from users where USERNAME = @inputUser";
                        command.Parameters.AddWithValue("@inputUser", userName);


                        if (command.ExecuteScalar() != null)
                        {
                            viewerSettings = command.ExecuteScalar().ToString();
                        }

                    }
                    catch (SqlException exc)
                    {
                        Console.WriteLine(exc.Message);
                        //viewerSettings.Value = exc.Message;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return viewerSettings;
        }
    }
}
