using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using BankManagementSystem.IO;

namespace BankManagementSystem.Core
{
	public class Account
	{
		public int ID;

		public string FirstName;
		public string LastName;
		public string Address;
		public int PhoneNumber;
		public string Email;

		public int Balance;

		public List<Transfer> Transfers;

		public Account(string FirstName, string LastName, string Address, int PhoneNumber, string Email)
		{
			this.FirstName = FirstName;
			this.LastName = LastName;
			this.Address = Address;
			this.PhoneNumber = PhoneNumber;
			this.Email = Email;

			Transfers = new List<Transfer>();
		}

		/// <summary>Validates the details of this Account.</summary>
		/// <returns>Issues regarding this Account's validity, if any.</returns>
		public EValidationResult Validate()
		{
			// Remove Leading and Trailing white-space.
			Input.Trim(ref FirstName);
			Input.Trim(ref LastName);
			Input.Trim(ref Address);
			Input.Trim(ref Email);

			// Check for empty inputs.
			if (FirstName.Length == 0 || LastName.Length == 0 || Address.Length == 0 || Email.Length == 0)
				return EValidationResult.FieldsEmpty;

			// Email Addresses only have one '@' symbol.
			int NumberOfAtSymbols = Email.Count(Character => Character == '@');
			bool bValidEmailAddress = NumberOfAtSymbols == 1;

			// That one '@' symbol must be immediately before an Email domain.
			// Use XOR. True if exactly one domain matches.
			bool bValidDomain = Email.EndsWith("@gmail.com");
			bValidDomain ^= Email.EndsWith("@outlook.com");
			bValidDomain ^= Email.EndsWith("@student.uts.edu.au");
			bValidDomain ^= Email.EndsWith("@uts.edu.au");

			// An Email Address cannot begin with the '@' symbol.
			// The first character of an Email Address must be a letter.
			bool bValidEmailPrefix = !Email.StartsWith('@') && char.IsLetter(Email[0]);

			// No field can have the delimiter '|'.
			bool bLegalCharacters = !FirstName.Contains('|');
			bLegalCharacters &= !LastName.Contains('|');
			bLegalCharacters &= !Address.Contains('|');
			bLegalCharacters &= !Email.Contains('|');

			// If the above three conditions are met, the Email Address is considered valid.
			if (bValidEmailAddress && bValidDomain && bValidEmailPrefix)
				return EValidationResult.Passed;

			// Byte gives us eight different errors.
			// We can consider multiple fails with enums and bitwise operations.
			byte Result = 0;

			// Bitwise operations to consider multiple errors.
			if (!bValidDomain)
				Result |= (byte)EValidationResult.InvalidDomain;

			// Check for either zero or too many '@' symbols.
			if (!bValidEmailAddress)
				Result |= NumberOfAtSymbols == 0
					? (byte)EValidationResult.NoAtSymbol
					: (byte)EValidationResult.TooManyAtSymbols;

			// Check for an illegal Email Address.
			if (!bValidEmailPrefix)
				Result |= (byte)EValidationResult.IllegalEmailAddress;

			if (!new EmailAddressAttribute().IsValid(Email))
				Result |= (byte)EValidationResult.IllegalEmailAddress;

			if (!bLegalCharacters)
				Result |= (byte)EValidationResult.IllegalCharacters;

			// The result can be thought of as a byte -> EValidationResult conversion.
			return (EValidationResult)Result;
		}

		/// <summary>Gets <see cref="FirstName"/> with the appropriate possessive apostrophe form.</summary>
		public string GetDecoratedName()
		{
			// Last element Index Operator.
			char LastChar = FirstName[^1];
			string ApostropheSuffix = LastChar == 'S' || LastChar == 's' ? "'" : "'s";

			return FirstName + ApostropheSuffix;
		}

		/// <summary>Gets the last five Transfers made to this Account.</summary>
		/// <param name="OutTransfers">The Transfers.</param>
		/// <returns>The number of Transfers. Maximum Five.</returns>
		public int GetLastFiveTransfers(out Transfer[] OutTransfers)
		{
			// Start from the fifth-last Transfer, or the beginning.
			int Last5Start = Math.Min(Transfers.Count, Math.Max(0, Transfers.Count - 5));

			// Malloc either 5, or the number of Transfers.
			OutTransfers = new Transfer[Transfers.Count - Last5Start];

			for (int i = Last5Start; i < Transfers.Count; ++i)
				OutTransfers[i - Last5Start] = Transfers[i];

			return Last5Start;
		}

		/// <summary>Updates this Account's file.</summary>
		public async void Write()
		{
			// Interpolate the lines in the file.
			string FirstName = $"First Name|{this.FirstName}";
			string LastName = $"Last Name|{this.LastName}";
			string Address = $"Address|{this.Address}";
			string Phone = $"Phone|{PhoneNumber}";
			string Email = $"Email|{this.Email}";
			string AccountNumber = $"AccountNo|{ID}";
			string Balance = $"Balance|{this.Balance}";

			// Write the above to a new file.
			await FileSystem.WriteToFile(FileSystem.kDirectory, ID.ToString() + ".txt", EWriteMode.Overwrite, Encoding.UTF8,
				FirstName,
				LastName,
				Address,
				Phone,
				Email,
				AccountNumber,
				Balance
			);

			// Preserve Transfer information. Append to the above file.
			foreach (Transfer T in Transfers)
				await FileSystem.WriteToFile(FileSystem.kDirectory, ID.ToString() + ".txt", EWriteMode.Append, Encoding.UTF8, T.ToString());
		}

		/// <summary>This Account as an Email Message.</summary>
		/// <param name="bIsStatement">Include Transfers?</param>
		public void Dispatch(bool bIsStatement = false)
		{
			// Appending multiple strings. Use StringBuilder.
			StringBuilder EmailMessage = new StringBuilder();

			// Alter the Subject of this Email based on whether this Account
			// is requesting an Account Statement.
			string Subject = bIsStatement
				? "Your Account Statement"
				: "Your New Bank Account Details";

			// HTML Heading 1 for the Subject.
			EmailMessage.Append($"<h1>{Subject}</h1>");

			// HTML Heading 2 for the Account Number.
			// Simple HTML Text for everything else.
			EmailMessage
			.Append($"<h2>Your Account Number: {ID}</h2>")
			.Append($"First Name: {FirstName}<br>")
			.Append($"Last Name: {LastName}<br>")
			.Append($"Address: {Address}<br>")
			.Append($"Phone Number: {PhoneNumber:D10}<br>")
			.Append($"Current Balance: {Balance:C0}");

			// If we are requesting an Account Statement, include the Transfers.
			// New Accounts will not request a Statement. New Accounts won't have
			// any Transfers.
			if (bIsStatement && Transfers.Count != 0)
			{
				// Table format the Transfers in the layout:
				// Date | Type | Amount.
				EmailMessage.Append("<br><br>");
				EmailMessage
				.Append("<table style=\"width:100%\">")
				.Append("<tr>")
				.Append("<th>Date</th>")
				.Append("<th>Type</th>")
				.Append("<th>Amount</th>")
				.Append("</tr>");

				// Write the Transfers to the Email Message.
				GetLastFiveTransfers(out Transfer[] LastTransfers);
				for (int i = 0; i < LastTransfers.Length; ++i)
				{
					Transfer Transfer = LastTransfers[i];

					EmailMessage
					.Append("<tr><td>")
					.Append(Transfer.Date)
					.Append("</td><td>")
					.Append(Transfer.Type)
					.Append("</td><td>")
					.Append(Transfer.Amount.ToString("C0"))
					.Append("</td></tr>");
				}

				EmailMessage.Append("</table>");
			}

			// Send this Email Message.
			IO.Email.Dispatch(Email, EmailMessage.ToString(), Subject);
		}

		/// <summary>Implicit bool operator checking whether or not a given Account is null.</summary>
		/// <param name="Account">The Account to check.</param>
		public static implicit operator bool(Account Account) => Account != null;
	}
}
