#if DEBUG
#define WITH_EMAIL_SEND_RESULTS
#endif

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BankManagementSystem.IO
{
	public static class Email
	{
		public static void Dispatch(string Receiver, string Message)
		{
			SmtpClient Client = new SmtpClient("smtp.gmail.com");
			Client.Port = 587;
			Client.UseDefaultCredentials = false;
			Client.Credentials = new NetworkCredential(EmailSecretConstants.kSendingEmailAddress, EmailSecretConstants.kSendingEmailPassword);
			Client.EnableSsl = true;

			MailAddress From = new MailAddress(EmailSecretConstants.kSendingEmailAddress, $"Michael {(char)0xD8} Wu", Encoding.UTF8);
			MailAddress To = new MailAddress(Receiver);

			MailMessage EmailMessage = new MailMessage(From, To)
			{
				Body = Message,
				BodyEncoding = Encoding.UTF8,
				Subject = "Your Bank Account",
				SubjectEncoding = Encoding.UTF8
			};

#if WITH_EMAIL_SEND_RESULTS
			Client.SendCompleted += Client_SendCompleted;
#endif

			Client.SendAsync(EmailMessage, null);

			EmailMessage.Dispose();
		}
#if WITH_EMAIL_SEND_RESULTS
		static void Client_SendCompleted(object sender, AsyncCompletedEventArgs e)
		{
			string token = (string)e.UserState;

			if (e.Error != null)
			{
				Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
			}
			else
			{
				Console.WriteLine("Message sent.");
			}
		}
#endif
	}
}
