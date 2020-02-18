using Microsoft.Extensions.Configuration;
using SendingPushNotifications.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SendingPushNotifications
{

    public class Globals
    {
        private static string _connectionString;
        public Globals(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("SQLConnection");
        }

        private static List<Email_Template_Token_Instance> AllTokensForStringBody = new List<Email_Template_Token_Instance>();
        private static List<Device_ID> Device_ID_List = new List<Device_ID>();

        public static List<Email_Template_Token_Instance> ReturnALLBodyStringFromDb()
        {
            return AllTokensForStringBody;
        }

        public void GetALLBodyStringFromDb()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string cmdStr = string.Format("SELECT * FROM Email_Template_Token_Instance where Template_Instance_Id IN (SELECT Template_Instance_ID FROM Email_Template_Instance Where Is_Sent_Mobile = 0)");
                    SqlCommand cmd = new SqlCommand(cmdStr, con);
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        AllTokensForStringBody.Add(new Email_Template_Token_Instance
                        {
                            Template_Instance_Id = Convert.ToInt32(rdr["Template_Instance_Id"]),
                            Template_Token_Instance_Id = Convert.ToInt32(rdr["Template_Token_Instance_Id"]),
                            Token_Value = rdr["Token_Value"].ToString(),
                            Template_Token_Mapping_Id = Convert.ToInt32(rdr["Template_Token_Mapping_Id"]),
                        });
                    }
                    con.Close();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static List<Device_ID> ReturnDeviceID()
        {
            return Device_ID_List;
        }

        public List<Device_ID> GetDeviceID()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string cmdStr = string.Format("SELECT Device_ID,Email_Address FROM Mobile_Notification a INNER JOIN ADM_USERS b ON a.User_ID = b.User_ID");
                    SqlCommand cmd = new SqlCommand(cmdStr, con);
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Device_ID_List.Add(new Device_ID
                        {
                            Email = rdr["Email_Address"].ToString(),
                            DeviceId = rdr["Device_ID"].ToString()
                        });
                    }
                    con.Close();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Device_ID_List;
        }
    }
}
