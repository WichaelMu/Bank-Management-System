using System.Collections.Generic;
using System.Linq;
using System.Text;
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

			// If the above three conditions are met, the Email Address is considered valid.
			if (bValidEmailAddress && bValidDomain && bValidEmailPrefix)
				return EValidationResult.Passed;
			
			// Byte gives us eight different errors.
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

			// The result can be thought of as a byte -> EValidationResult conversion.
			return (EValidationResult)Result;
		}

		/// <summary>Get's <see cref="FirstName"/> with the appropriate possessive apostrophe form.</summary>
		public string GetDecoratedName()
		{
			char LastChar = FirstName[FirstName.Length - 1];
			string ApostropheSuffix = LastChar == 'S' || LastChar == 's' ? "'" : "'s";
			return FirstName + ApostropheSuffix;
		}

		/// <summary>Updates this Account's file.</summary>
		public async void Write()
		{
			string FirstName = $"First Name|{this.FirstName}";
			string LastName = $"Last Name|{this.LastName}";
			string Address = $"Address|{this.Address}";
			string Phone = $"Phone|{PhoneNumber}";
			string Email = $"Email|{this.Email}";
			string AccountNumber = $"AccountNo|{ID}";
			string Balance = $"Balance|{this.Balance}";

			await FileSystem.WriteToFile(FileSystem.kDirectory, ID.ToString() + ".txt", EWriteMode.Overwrite, Encoding.UTF8,
				FirstName,
				LastName,
				Address,
				Phone,
				Email,
				AccountNumber,
				Balance
			);

			// Preserve Transfer information.
			foreach (Transfer T in Transfers)
				await FileSystem.WriteToFile(FileSystem.kDirectory, ID.ToString() + ".txt", EWriteMode.Append, Encoding.UTF8, T.ToString());
		}

		/// <summary>This Account as an Email Message.</summary>
		/// <param name="bIsStatement">Include Transfers?</param>
		public void Dispatch(bool bIsStatement = false)
		{
			StringBuilder EmailMessage = new StringBuilder();

			string Subject = bIsStatement
				? "Your Account Statement"
				: "Your New Bank Account Details";

			EmailMessage.Append($"<h1>{Subject}</h1>");

			EmailMessage
			.Append($"<h2>Your Account Number: {ID}</h2>")
			.Append($"First Name: {FirstName}<br>")
			.Append($"Last Name: {LastName}<br>")
			.Append($"Address: {Address}<br>")
			.Append($"Current Balance: {Balance:C0}");

			if (bIsStatement && Transfers.Count != 0)
			{
				EmailMessage.Append("<br><br>");
				EmailMessage
				.Append("<table style=\"width:100%\">")
				.Append("<tr>")
				.Append("<th>Date</th>")
				.Append("<th>Type</th>")
				.Append("<th>Amount</th>")
				.Append("</tr>");

				foreach (Transfer Transfer in Transfers)
					EmailMessage
					.Append("<tr><td>")
					.Append(Transfer.Date)
					.Append("</td><td>")
					.Append(Transfer.Type)
					.Append("</td><td>")
					.Append(Transfer.Amount.ToString("C0"))
					.Append("</td></tr>");

				EmailMessage.Append("</table>");
			}

			IO.Email.Dispatch(Email, EmailMessage.ToString(), Subject);
		}

		public static implicit operator bool (Account Account) => Account != null;
	}
}
