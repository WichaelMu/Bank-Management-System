

/* --                                          Pre-processor Directives.                                      -- */
/* -- Used during debugging only, defines whether or not to disable sending Emails, to save Google API calls. -- */

// Whether or not to actually send Emails, or a stub debug printing.
#define WITH_EMAIL

#if WITH_EMAIL
// Whether or not to show Email Send Completion Messages/Results.
#define WITH_EMAIL_SEND_RESULTS
#endif

#if WITH_EMAIL
#if WITH_EMAIL_SEND_RESULTS
using System;
#endif // WITH_EMAIL_SEND_RESULTS
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;
#endif // WITH_EMAIL
#if WITH_EMAIL_SEND_RESULTS || !WITH_EMAIL
using static BankManagementSystem.IO.OutputHelpers;
#endif // WITH_EMAIL_SEND_RESULTS || !WITH_EMAIL

namespace BankManagementSystem.IO
{
	/// <summary>Utility class that dispatches e-mail messages.</summary>
	/// <remarks>Provides an API that abstracts the complexity of C# e-mail dispatchers.</remarks>
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
			Client.SendCompleted += Client_OnSendCompleted;
#endif // WITH_EMAIL_SEND_RESULTS

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
#else // !WITH_EMAIL
		public static void Dispatch(string Receiver, string Message, string Subject = "Your Bank Account", bool bBodyIsHTML = true) { Print($"Send Email to {Receiver} with Subject: {Subject}"); }
#endif // WITH_EMAIL

#if WITH_EMAIL_SEND_RESULTS

		static bool bShouldPrintResults = false;

		/// <summary>Called once when the corresponding Email Address is sent.</summary>
		/// <param name="Sender">The object that called this Event.</param>
		/// <param name="Args">Async Event Arguments.</param>
		static void Client_OnSendCompleted(object Sender, AsyncCompletedEventArgs Args)
		{
			if (bShouldPrintResults)
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
					Print("Email Message sent. You may now exit the Bank Management System.", ConsoleColor.Green);
				}
			}
		}

		public static void SetAllowEmailSendResultsPrinting(bool bInAllow)
		{
			bShouldPrintResults = bInAllow;
		}
#else
		public static void SetAllowEmailSendResultsPrinting(bool bInAllow) {  }
#endif // WITH_EMAIL_SEND_RESULTS

#if WITH_EMAIL
		/// <summary>
		/// Is this Program still waiting to either receive confirmation of a sent email?
		/// <br><b>OR</b></br><br></br>
		/// Is this Program still sending an Email?
		/// </summary>
		/// <returns><see langword="true"/> if this Program is waiting on, or still sending an Email.</returns>
		public static bool IsAwaitingAsyncEmail()
		{
			// We have pending Emails to be sent if this value is not zero.
			return EmailsRemainingToBeSent != 0;
		}
#else
		public static bool IsAwaitingAsyncEmail() { return false; }
#endif // WITH_EMAIL
	}
}
