using Microsoft.Extensions.Configuration;
using SendingPushNotifications.Logics;
using System;
using System.IO;
using System.Reflection;

namespace SendingPushNotifications
{
    class Program
    {
        private static IConfiguration _iconfiguration;
        static void Main(string[] args)
        {
            Console.WriteLine("Push Notification Is Running.");
            GetAppSettingsFile();
            GetAllGlobalRecords();

            var _AccessDb = new AccessDb(_iconfiguration);
            var objList = AccessDb.FillList(_AccessDb.ReturnTable());
            var List = AccessDb.CreateNotification(objList);

            string ListIsSend = "";
            foreach (var item in List)
            {
                NotifyAsyncTemplate.PushNotification(item);
                ListIsSend += item.Template_Instance_ID.ToString() + ',';
                //For Debugging.
                //if (ListIsSend.Split(',').Count() == 100)
                //    break;
            }
            //_AccessDb.Set_Is_Sent(ListIsSend);
        }

        private static void GetAllGlobalRecords()
        {
            var _Globals = new Globals(_iconfiguration);
            _Globals.GetALLBodyStringFromDb();
            _Globals.GetDeviceID();
        }

        private static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }
    }
}
