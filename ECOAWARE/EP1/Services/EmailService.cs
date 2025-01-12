using MailKit.Net.Smtp;
using MimeKit;

namespace EP1.Services
{
	public class EmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string message)
		{
			var emailSettings = _configuration.GetSection("EmailSettings");

			// Lấy giá trị cấu hình
			var smtpServer = emailSettings["SMTPServer"];
			var smtpPort = emailSettings["Port"];
			var senderEmail = emailSettings["SenderEmail"];
			var appPassword = emailSettings["AppPassword"];

			// Kiểm tra cấu hình hợp lệ
			if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(smtpPort) ||
				string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(appPassword))
			{
				throw new InvalidOperationException("Email settings are not properly configured.");
			}

			var email = new MimeMessage();
			email.From.Add(new MailboxAddress("Your App Name", senderEmail));
			email.To.Add(new MailboxAddress("", toEmail));
			email.Subject = subject;

			var bodyBuilder = new BodyBuilder { HtmlBody = message };
			email.Body = bodyBuilder.ToMessageBody();

			using var smtp = new SmtpClient();
			try
			{
				Console.WriteLine("Connecting to SMTP server...");
				await smtp.ConnectAsync(smtpServer, int.Parse(smtpPort), MailKit.Security.SecureSocketOptions.StartTls);
				Console.WriteLine("Authenticating...");
				await smtp.AuthenticateAsync(senderEmail, appPassword);

				Console.WriteLine("Sending email...");
				await smtp.SendAsync(email);
				Console.WriteLine("Email sent successfully!");

				await smtp.DisconnectAsync(true);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to send email: {ex.Message}");
				throw new InvalidOperationException("Could not send email. Please check SMTP settings.", ex);
			}
		}
	}
}
