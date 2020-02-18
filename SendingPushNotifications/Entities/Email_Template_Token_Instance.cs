namespace SendingPushNotifications.Entities
{
    public partial class Email_Template_Token_Instance
    {
        public int Template_Token_Instance_Id { get; set; }
        public int Template_Instance_Id{ get; set; }
        public string Token_Value { get; set; }
        public int Template_Token_Mapping_Id { get; set; }
    }
}
