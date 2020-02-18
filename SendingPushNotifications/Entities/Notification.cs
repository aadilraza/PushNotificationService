namespace SendingPushNotifications.Entities
{
    public partial class Notification
    {
        public int Template_Instance_ID { get; set; }
        public int Template_Id { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public partial class NotifY
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Device_ID { get; set; }
        public int Template_Instance_ID { get; set; }
    }

}
