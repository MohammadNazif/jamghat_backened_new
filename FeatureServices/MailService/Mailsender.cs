
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using static jamghat.FeatureServices.MailService.MailModel;


public class MailSender
    {
        private readonly MailSettings _settings;

        public MailSender(IOptions<MailSettings> options)
        {
            _settings = options.Value;
        }

    public async Task SendAsync(
  string toEmail,
  string subject,
  string body,
  bool isHtml = true)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentException("Recipient email cannot be null or empty", nameof(toEmail));

        var mail = new MailMessage
        {
            From = new MailAddress(_settings.SenderId, "Jamghat"),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };

        mail.To.Add(toEmail);  // Will throw if toEmail is null or empty

        using var smtp = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.SenderId, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        await smtp.SendMailAsync(mail);
    }

}

