using Newtonsoft.Json;
using SendingPushNotifications.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SendingPushNotifications.Logics
{
    public static class NotifyAsyncTemplate
    {
        public static void PushNotification(NotifY notification)
        {
            var ListOfDeviceID = new List<string>();
            //var ListOfDeviceID = new List<string> { "fBeOH9X70Oo:APA91bFvYWWLBhQznlk9-yXsnB7Cl-ZPAYTqnoHTjxG7SsZvPVPCV9kdgKQdHr7KAXxK5FE26XuzJF0yIjP2jaYOW-68Sl7RsN7_-b3OFpulI-NWJ6txK7CpJUpWOozuEY8mN_rgZUqP" };
            ListOfDeviceID.Add(notification.Device_ID);
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAtj-2434:APA91bEZ3dLcqbQY2FLSRGAns24SbADvcIHtBi5asB6I2E3kvqdO7kPlcMUF9YXbhEOYo0QBhPIQjEmFUeGfT4JLvqlepKLvvA8aFAn_X2X0cjBIFrIhd6GPQc2DEcIo4pnkd8txt0x7"));
            tRequest.Headers.Add(string.Format("Sender: id={0}", "782752998270"));
            tRequest.ContentType = "application/json";

            var payload = new
            {
                registration_ids = ListOfDeviceID,
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = notification.Body,
                    title = notification.Title,
                    badge = 1
                }
            };

            string postbody = JsonConvert.SerializeObject(payload).ToString();
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //result.Response = sResponseFromServer;
                            }
                    }
                }
            }
        }
    }
}
