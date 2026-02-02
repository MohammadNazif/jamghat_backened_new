namespace jamghat.FeatureServices.MailService
{
    public class MailModel
    {
        public class CommonMessage
        {
            public bool Status { get; set; }
            public string Message { get; set; }
        }

        public class MailRequest
        {
            public string ToEmail { get; set; }
            public string Subject { get; set; }
            public string BodyType { get; set; }

          
        }
    }
}
