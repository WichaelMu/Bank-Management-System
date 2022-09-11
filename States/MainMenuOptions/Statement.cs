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
				PrintLastFiveTransfers(Account, out int TotalLast);

				if (ReceiveEmailInput(TotalLast))
					SendEmail(Account);
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

				// Cannot exceed a length of 10.
				if (IntAsString.Length > 10)
				{
					Print("Account Numbers do not exceed 10 digits!", ConsoleColor.Red);
				}
				// If IntAsString IS a number, but we can't find a corresponding Account Number.
				else if (int.TryParse(IntAsString, out _) && !SearchAccountID(IntAsString))
				{
					Print($"Account Number {IntAsString} does not exist!", ConsoleColor.Red);
				}
				// If IntAsString is null or Empty, and it has reached this point, then we know
				// this has looped more than once and IntAsString contains non-number characters.
				else if (!string.IsNullOrEmpty(IntAsString))
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

		void PrintLastFiveTransfers(Account Account, out int TotalLast)
		{
			if (Account.Transfers.Count == 0)
			{
				TotalLast = 0;
				return;
			}

			int Last5 = Account.GetLastFiveTransfers(out Transfer[] LastTransfers);

			TotalLast = Account.Transfers.Count - Last5;
			string Last5Title = $"Your Last {TotalLast} Statements";

			// 2 Lines as Empty Space.
			PrintNewLineWithBorder(CharacterLimit, ' ');
			PrintNewLineWithBorder(CharacterLimit, ' ');

			// 3 Lines to Print the Title.
			PrintTitle(Last5Title);

			for (int i = 0; i < LastTransfers.Length; ++i)
			{
				string StatementSubtitle = $"Statement #{i + 1}";
				AutoCentre(StatementSubtitle, CharacterLimit, out int SPadding);

				Transfer T = LastTransfers[i];

				string Type = T.Type.ToString();
				string Date = T.Date;
				string Amount = $"Amount: {T.Amount:C0}";
				string Balance = $"Resulting Balance: {T.Balance:C0}";

				PaddingUntilEnd(Type, CharacterLimit, out int TPadding);
				PaddingUntilEnd(Date, CharacterLimit, out int DPadding);
				PaddingUntilEnd(Amount, CharacterLimit, out int APadding);
				PaddingUntilEnd(Balance, CharacterLimit, out int BPadding);

				// 8 Lines for each Transfer.

				PrintWithBorder(StatementSubtitle, SPadding);
				PrintNewLineWithBorder(CharacterLimit);

				PrintWithCustomPadding(Type, kTabSize, TPadding - 4);
				PrintWithCustomPadding(Date, kTabSize, DPadding - 4);
				PrintWithCustomPadding(Amount, kTabSize, APadding - 4);
				PrintWithCustomPadding(Balance, kTabSize, BPadding - 4);

				PrintNewLineWithBorder(CharacterLimit);
				PrintWithBorder(HorizontalBorder);
			}
		}

		bool ReceiveEmailInput(int TotalLast)
		{
			Console.SetCursorPosition(0, TotalLast != 0
				? TotalLast * 8 + 19 // 8 Lines are printed for each Transfer, and 14 + 5 for any the Last Statement Titles.
				: 14 // If there are no Transfers to print.
			);

			Print("Email Account Statment (y/n)?");

			Input.Char(out char Key);
			while (Key != 'Y' && Key != 'y' && Key != 'N' && Key != 'n')
			{
				Console.SetCursorPosition(0, 14);
				Print("Invalid Response. Valid inputs are (Y/N) or (y/n)\nIs the above information correct? ", ConsoleColor.Red);
				Input.Char(out Key);
			}

			// True if the User wants an Email of this Account's Statement.
			return Key == 'Y' || Key == 'y';
		}

		void SendEmail(Account Account)
		{
			Account.Dispatch(true);

			Print("Your Account Statement is being sent! Press any key to return to the Main Menu...", ConsoleColor.Green);

			Input.Any();
		}
	}
}
