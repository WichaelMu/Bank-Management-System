using System;
using BankManagementSystem.Core;
using BankManagementSystem.IO;
using static BankManagementSystem.IO.OutputHelpers;

namespace BankManagementSystem
{
	public partial class BMS
	{
		void RunDeleteSequence()
		{
			Console.Clear();

			PrintDeletePrompt();
			ReceiveDeleteInput(out Account Account);

			// If Cancelled, don't do anything.
			if (Account)
				ConfirmDelete(Account);

			RunMainMenuSequence();
		}

		void PrintDeletePrompt()
		{
			const string kDeleteTitle = "DELETE";

			const string kAccountNumberPrompt = "Account Number: ";

			PrintTitle(kDeleteTitle);

			AutoCentre(kPrompt, CharacterLimit, out int PromptPadding);

			PrintWithBorder(kPrompt, PromptPadding);
			PrintNewLineWithBorder(CharacterLimit);

			PaddingUntilEnd(kAccountNumberPrompt, CharacterLimit, out int ANPadding);

			PrintWithCustomPadding(kAccountNumberPrompt, kTabSize, ANPadding - 4, bWithBorder: true);

			PrintWithBorder(HorizontalBorder);
		}

		void ReceiveDeleteInput(out Account Account)
		{
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

				// Re-enter Account Number.
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

			Console.Clear();
			PrintAccount(AccountNumber);
		}

		void ConfirmDelete(Account AccountToDelete)
		{
			Console.SetCursorPosition(0, 14);
			Print("Delete Account (y/n)?");

			string DecoratedName = AccountToDelete.GetDecoratedName();

			// Prompt Yes/No to Delete.
			Input.Char(out char Key);
			while (Key != 'Y' && Key != 'y' && Key != 'N' && Key != 'n')
			{
				Console.SetCursorPosition(0, 14);
				Print($"Invalid Response. Valid inputs are (Y/N) or (y/n)\nDo you want to delete {DecoratedName} Account? ", ConsoleColor.Red);
				Input.Char(out Key);
			}

			Console.SetCursorPosition(0, 14);

			// Confirm Delete.
			if (Key == 'Y' || Key == 'y')
			{
				FileSystem.DeleteAccount(AccountToDelete.ID);
				Print($"{DecoratedName} Account was Deleted!", ConsoleColor.Yellow);
			}
			// Cancel Delete.
			else
			{
				Print($"Did not Delete {DecoratedName} Account.", ConsoleColor.Yellow);
			}

			Print("Press any key to return to the Main Menu...");
			Input.Any();
		}
	}
}
