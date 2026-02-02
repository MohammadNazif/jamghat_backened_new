namespace jamghat.FeatureServices.MailService
{
    public class MailModel
    {
        public class MailSettings
        {
            public string SenderId { get; set; }
            public string Password { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
            public bool EnableSsl { get; set; }
        }

        public class MailRequest
        {
            public string ToEmail { get; set; }
            public string Subject { get; set; }
            public string BodyType { get; set; }

          
        }
    }
}
