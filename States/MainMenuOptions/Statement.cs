using System;
using BankManagementSystem.Core;
using BankManagementSystem.IO;
using static BankManagementSystem.IO.OutputHelpers;

namespace BankManagementSystem
{
	public partial class BMS
	{
		void RunStatmentSequence()
		{
			Console.Clear();

			PrintStatementPrompt();
			ReceiveStatementInput(out Account Account);
			if (Account)
			{
				PrintAccountStatement(Account);
				if (ReceiveEmailInput())
					Account.Dispatch(true);
			}

			RunMainMenuSequence();
		}

		void PrintStatementPrompt()
		{
			const string kStatementTitle = "STATEMENT";

			const string kAccountNumberPrompt = "Account Number: ";

			PrintTitle(kStatementTitle);

			AutoCentre(kPrompt, CharacterLimit, out int PromptPadding);

			PrintWithBorder(kPrompt, PromptPadding);
			PrintNewLineWithBorder(CharacterLimit);

			PaddingUntilEnd(kAccountNumberPrompt, CharacterLimit, out int ANPadding);

			PrintWithCustomPadding(kAccountNumberPrompt, kTabSize, ANPadding - 4);

			PrintWithBorder(HorizontalBorder);
		}

		void ReceiveStatementInput(out Account Account)
		{
			Console.SetCursorPosition(21, 5);

			// Protect the Account Number from being an illegal value.
			int AccountNumber;
			string IntAsString = string.Empty;

			do
			{
				Console.SetCursorPosition(0, 8);

				bool bInputWasEmpty = !string.IsNullOrEmpty(IntAsString);

				// Cannot exceed a length of 10.
				if (IntAsString.Length > 10)
				{
					Print("Account Numbers do not exceed 8 digits!", ConsoleColor.Red);
				}
				else if (int.TryParse(IntAsString, out _) && !SearchAccountID(IntAsString))
				{
					Print($"Account Number {IntAsString} does not exist!", ConsoleColor.Red);
				}
				// If not empty and is executed, then it has previously failed with letters.
				else if (bInputWasEmpty)
				{
					Print("Account Numbers can only have numbers! Use 'x' to Cancel.", ConsoleColor.Red);
				}

				// Set the position to the end of the Account Number.
				Console.SetCursorPosition(21 + IntAsString.Length, 5);

				// Backspace any illegal characters (non-number string)
				for (int i = 0; i < IntAsString.Length; ++i)
					Backspace();

				IntAsString = Input.String();

				// Cancel...
				if (IntAsString == "x")
				{
					Account = null;
					return;
				}
			}
			// If the Input is NaN or is > 10, loop.
			while (!int.TryParse(IntAsString, out AccountNumber) || IntAsString.Length > 10 || !SearchAccountID(AccountNumber));

			Account = AccountParser.ConstructFromFile(AccountNumber);
		}

		void PrintAccountStatement(Account Account)
		{
			Console.Clear();

			const string kStatementTitle = "SIMPLE BANKING SYSTEM";
			const string kStatementSubtitle = "Account Statement";

			string AccountNumber = $"Account Number: {Account.ID}";
			string AccountBalance = $"Account Balance: {Account.Balance:C0}";
			string FirstName = $"First Name: {Account.FirstName}";
			string LastName = $"Last Name: {Account.LastName}";
			string Address = $"Address: {Account.Address}";
			string PhoneNumber = $"Phone: {Account.PhoneNumber:D10}";
			string EmailAddress = $"Email: {Account.Email}";

			PrintTitle(kStatementTitle);

			AutoCentre(kStatementSubtitle, CharacterLimit, out int SubtitlePadding);

			PaddingUntilEnd(AccountNumber, CharacterLimit, out int ANPadding);
			PaddingUntilEnd(AccountBalance, CharacterLimit, out int ABPadding);
			PaddingUntilEnd(FirstName, CharacterLimit, out int FNPadding);
			PaddingUntilEnd(LastName, CharacterLimit, out int LNPadding);
			PaddingUntilEnd(Address, CharacterLimit, out int APadding);
			PaddingUntilEnd(PhoneNumber, CharacterLimit, out int PNPadding);
			PaddingUntilEnd(EmailAddress, CharacterLimit, out int EPadding);

			PrintWithBorder(kStatementSubtitle, SubtitlePadding);
			PrintNewLineWithBorder(CharacterLimit);

			PrintWithCustomPadding(AccountNumber, kTabSize, ANPadding - 4);
			PrintWithCustomPadding(AccountBalance, kTabSize, ABPadding - 4);
			PrintWithCustomPadding(FirstName, kTabSize, FNPadding - 4);
			PrintWithCustomPadding(LastName, kTabSize, LNPadding - 4);
			PrintWithCustomPadding(Address, kTabSize, APadding - 4);
			PrintWithCustomPadding(PhoneNumber, kTabSize, PNPadding - 4);
			PrintWithCustomPadding(EmailAddress, kTabSize, EPadding - 4);

			PrintWithBorder(HorizontalBorder);
		}

		bool ReceiveEmailInput()
		{
			Console.SetCursorPosition(0, 14);
			Print("Email Account Statment (y/n)?");

			Input.Char(out char Key);
			while (Key != 'Y' && Key != 'y' && Key != 'N' && Key != 'n')
			{
				Console.SetCursorPosition(0, 14);
				Print("Invalid Response. Valid inputs are (Y/N) or (y/n)\nIs the above information correct? ", ConsoleColor.Red);
				Input.Char(out Key);
			}

			return Key == 'Y' || Key == 'y';
		}
	}
}
