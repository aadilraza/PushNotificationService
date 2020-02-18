using Microsoft.Extensions.Configuration;
using SendingPushNotifications.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SendingPushNotifications
{
    public class AccessDb
    {
        private static string _connectionString;
        public AccessDb(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("SQLConnection");
        }

        public DataTable ReturnTable()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("Get_Mobile_Notification_Users", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dt;
        }

        public static List<Notification> FillList(DataTable dt)
        {
            var objList = new List<Notification>();
            try
            {
                objList = (from DataRow row in dt.Rows
                           select new Notification()
                           {
                               Template_Instance_ID = Convert.ToInt32(row["Template_Instance_ID"]),
                               Template_Id = Convert.ToInt32(row["Template_Id"]),
                               Email = row["To_Add"].ToString(),
                               Subject = row["Subject"].ToString(),
                               Body = row["Body"].ToString()
                           }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return objList;
        }

        public static List<NotifY> CreateNotification(List<Notification> not)
        {
            List<NotifY> NotifyList = new List<NotifY>();
            try
            {
                int count = 0;
                foreach (var k in not)
                {
                    var DeviceID = GetDeviceID(k.Email);
                    if (DeviceID != "") //If Device Does n't Exist.
                    {
                        var ListOfGettedParticularStrings = GetBodyStringFromDb(k.Template_Instance_ID);
                        string titleText = k.Subject, bodyText = "";
                        if (k.Body.Contains("{!Token.Break_Line}"))
                            bodyText = k.Body.Replace("{!Token.Break_Line}", "\n");
                        else
                            bodyText = k.Body;
                        MatchCollection mcol = Regex.Matches(bodyText, @"{!\b\S+?\b}");
                        StringBuilder sbody = new StringBuilder(bodyText);
                        for (int i = 0; i < mcol.Count; i++)
                        {
                            //Notification Title Starts.
                            if (k.Subject.Contains(mcol[i].Value))
                                titleText = k.Subject.Replace(mcol[i].Value, ListOfGettedParticularStrings[i].Token_Value);
                            //Notification Title Ends.
                            //Notification Body Start.
                            if (bodyText.Contains(mcol[i].Value))
                            {
                                sbody.Replace(mcol[i].Value, ListOfGettedParticularStrings[i].Token_Value);
                            }
                            //Notification Body Ends.
                        }
                        var notify = new NotifY()
                        {
                            Body = sbody.ToString(),
                            Device_ID = DeviceID,
                            Title = titleText,
                            Template_Instance_ID = k.Template_Instance_ID
                        };
                        NotifyList.Add(notify);
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return NotifyList;
        }

        private static List<Email_Template_Token_Instance> GetBodyStringFromDb(int Template_Instance_ID)
        {
            var ToReturn = new List<Email_Template_Token_Instance>();
            try
            {
                var List = Globals.ReturnALLBodyStringFromDb();
                ToReturn = List.Where(x => x.Template_Instance_Id == Template_Instance_ID).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ToReturn;
        }

        private static string GetDeviceID(string Email)
        {
            string ToReturn = "";
            try
            {
                var List = Globals.ReturnDeviceID();
                var NewRet = List.Where(x => x.Email == Email).ToList();
                if (NewRet.Count() > 0)
                {
                    ToReturn = NewRet.FirstOrDefault().DeviceId;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ToReturn;
        }

        public void Set_Is_Sent(string List)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("Update_Is_Sent_Mobile", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ARRAY_OF_Id", List));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
