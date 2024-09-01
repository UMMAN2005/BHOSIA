using BHOSIA.DATA;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace BHOSIA.BUSINESS.Services.Implementations;

public class EmailService(IConfiguration configuration)
  : IEmailService {
  // ReSharper disable once UnusedMember.Local
  public static string Me => "ummanmemmedov2005@gmail.com";

  public void Send(string to, string subject, string body) {
    var email = new MimeMessage();

    email.From.Add(MailboxAddress.Parse(configuration.GetSection("EmailSettings:From").Value));
    email.To.Add(MailboxAddress.Parse(to));
    email.Subject = subject;
    email.Body = new TextPart(TextFormat.Html) { Text = body };

    using var smtp = new SmtpClient();
    smtp.Connect(configuration.GetSection("EmailSettings:Provider").Value, Convert.ToInt32(configuration.GetSection("EmailSettings:Port").Value), true);
    smtp.Authenticate(configuration.GetSection("EmailSettings:UserName").Value, configuration.GetSection("EmailSettings:Password").Value);
    smtp.Send(email);
    smtp.Disconnect(true);
  }

  public void SendNotificationEmail() {
  }
}