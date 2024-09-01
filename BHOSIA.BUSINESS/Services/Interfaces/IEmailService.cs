namespace BHOSIA.BUSINESS.Services.Interfaces;

public interface IEmailService {
  void Send(string to, string subject, string body);
  void SendNotificationEmail();
}
