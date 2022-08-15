

/* -- Pre-processor Directives. -- */

// Whether or not to actually send Emails, or a stub debug printing.
#define WITH_EMAIL
// Whether or not to show Email Send Completion Messages/Results.
#define WITH_EMAIL_SEND_RESULTS

using System;
using System.ComponentModel;
#if WITH_EMAIL
using System.Net;
using System.Net.Mail;
using System.Text;
#endif
#if WITH_EMAIL_SEND_RESULTS
using static BankManagementSystem.IO.OutputHelpers;
#endif

namespace BankManagementSystem.IO
{
	public static class Email
	{
#if WITH_EMAIL
		static int EmailsRemainingToBeSent = 0;

		/// <summary>Sends an Email to <paramref name="Receiver"/> saying <paramref name="Message"/>.</summary>
		/// <param name="Receiver">The Email Address that Message will be sent to.</param>
		/// <param name="Message">The contents that Receiver will read.</param>
		/// <param name="Subject">The Subject of this Email Message.</param>
		/// <param name="bBodyIsHTML"><see cref="true"/> if Message is HTML.</param>
		public static void Dispatch(string Receiver, string Message, string Subject = "Your Bank Account", bool bBodyIsHTML = true)
		{
			// Use Gmail as our Email Provider. Use Google's SMTP Settings.
			SmtpClient Client = new SmtpClient("smtp.gmail.com");
			Client.Port = 587;
			Client.UseDefaultCredentials = false;
			Client.Credentials = new NetworkCredential(EmailSecretConstants.kSendingEmailAddress, EmailSecretConstants.kSendingEmailPassword);
			Client.EnableSsl = true;

			// Define Sender/Receiver Addresses.
			MailAddress From = new MailAddress(EmailSecretConstants.kSendingEmailAddress, "Michael Wu", Encoding.UTF8);
			MailAddress To = new MailAddress(Receiver);

			// Declare the Email Message.
			MailMessage EmailMessage = new MailMessage(From, To)
			{
				Body = Message,
				BodyEncoding = Encoding.UTF8,
				Subject = Subject,
				SubjectEncoding = Encoding.UTF8,
				IsBodyHtml = bBodyIsHTML
			};

#if WITH_EMAIL_SEND_RESULTS
			// Notify on Email Sent.
			Client.SendCompleted += Client_SendCompleted;
#endif
			// Dispose the Email and SMTP Client once it has been sent.
			Client.SendCompleted += (object Sender, AsyncCompletedEventArgs E) =>
			{
				EmailMessage.Dispose();
				Client.Dispose();

				--EmailsRemainingToBeSent;
			};

			// Asynchronously send the Email with no Cancellation Token.
			Client.SendAsync(EmailMessage, null);
			++EmailsRemainingToBeSent;
		}
#else
		public static void Dispatch(string Receiver, string Message, string Subject = "Your Bank Account", bool bBodyIsHTML = true) { Print($"Send Email to {Receiver} with Subject: {Subject}"); }
#endif // WITH_EMAIL

#if WITH_EMAIL_SEND_RESULTS
		/// <summary>Called once when the corresponding Email Address is sent.</summary>
		/// <param name="Sender">The object that called this Event.</param>
		/// <param name="Args">Async Event Arguments.</param>
		static void Client_SendCompleted(object Sender, AsyncCompletedEventArgs Args)
		{
			string Token = (string)Args.UserState;

			// If there was an error while sending an Email...
			if (Args.Error != null)
			{
				SetColours(ConsoleColor.Red);
				Console.WriteLine("[{0}] {1}", Token, Args.Error.ToString());
				ResetColours();
			}
			// If the Email was successfully sent...
			else
			{
				Print("Message sent.", ConsoleColor.Green);
			}
		}
#endif // WITH_EMAIL_SEND_RESULTS

#if WITH_EMAIL
		public static bool IsAwaitingAsyncEmail()
		{
			return EmailsRemainingToBeSent != 0;
		}
#else
		public static bool IsAwaitingAsyncEmail() { return false; }
#endif // WITH_EMAIL
	}
}
