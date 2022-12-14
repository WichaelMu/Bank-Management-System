using System;
using BankManagementSystem.Core;
using BankManagementSystem.IO;
using static BankManagementSystem.IO.Output;

namespace BankManagementSystem
{
	public partial class BMS
	{
		void RunDepositSequence()
		{
			Console.Clear();

			PrintDepositPrompts();
			ReceiveDepositInput();

			RunMainMenuSequence();
		}

		void PrintDepositPrompts()
		{
			const string kDepositTitle = "DEPOSIT";

			const string kAccountNumberPrompt = "Account Number: ";
			const string kAmount = "Amount: $";

			PrintTitle(kDepositTitle);

			AutoCentre(kPrompt, out int PromptPadding);

			PrintWithBorder(kPrompt, PromptPadding);
			PrintNewLineWithBorder();

			PaddingUntilEnd(kAccountNumberPrompt, out int ANPadding);
			PaddingUntilEnd(kAmount, out int APadding);

			PrintWithCustomPadding(kAccountNumberPrompt, kTabSize, ANPadding - 4);
			PrintWithCustomPadding(kAmount, kTabSize, APadding - 4);

			PrintWithBorder(HorizontalBorder);
		}

		void ReceiveDepositInput()
		{
			Console.SetCursorPosition(21, 5);

			// Protect the Account Number from being an illegal value.
			int AccountNumber;
			string IntAsString = string.Empty;

			do
			{
				Console.SetCursorPosition(0, 9);

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
				if (IntAsString == "x" || IntAsString == "X")
				{
					return;
				}
			}
			// If the Input is NaN or is > 10, loop.
			while (!int.TryParse(IntAsString, out AccountNumber) || IntAsString.Length > 10 || !SearchAccountID(AccountNumber));

			// Protect the Amount from being an illegal value.
			int Amount;
			string AmountAsString = string.Empty;
			Account FromID = AccountParser.ConstructFromFile(AccountNumber);

			do
			{
				Console.SetCursorPosition(0, 9);
				ClearLine();

				// Cannot exceed a length of 10.
				if (AmountAsString.Length >= 10)
				{
					// 1 << 31 is already 10 digits. Prevent overflow.
					Print("Invalid Deposit Amount! Amount was too large, please try again.", ConsoleColor.Red);
				}
				// If AmountAsString IS a number, but the Amount is negative.
				else if (int.TryParse(AmountAsString, out int TriedAmount) && TriedAmount < 0)
				{
					Print("A Deposit Amount must be non-negative!", ConsoleColor.Red);
				}
				// If AmountAsString is null or Empty, and it has reached this point, then we know
				// this has looped more than once and AmountAsString contains non-number characters.
				else if (!string.IsNullOrEmpty(AmountAsString))
				{
					Print("Enter the desired amount with numbers only! Use 'x' to Cancel.", ConsoleColor.Red);
				}

				// Set the position to the end of the Account Number.
				Console.SetCursorPosition(14 + AmountAsString.Length, 6);

				// Backspace any illegal characters (non-number string)
				for (int i = 0; i < AmountAsString.Length; ++i)
					Backspace();

				AmountAsString = Input.String();

				if (AmountAsString == "x" || AmountAsString == "X")
				{
					return;
				}
			}
			// If the Input is NaN, loop.
			while (!int.TryParse(AmountAsString, out Amount) || Amount < 0);

			Console.SetCursorPosition(0, 9);

			// Depositing Amount into Account.
			if (Amount != 0)
			{
				// Update and Write Account to file.
				FromID.Balance += Amount;
				FromID.Transfers.Add(new Transfer(FormatDate(), ETransferType.Deposit, Amount, FromID.Balance));
				FromID.Write();

				Print($"Successfully Deposited ${Amount} into {FromID.GetDecoratedName()} Account!", ConsoleColor.Green);
			}
			// Depositing $0.00 does nothing.
			else
			{
				Print($"A Deposit of $0.00 was attempted! A Deposit into {FromID.GetDecoratedName()} Account has been cancelled.", ConsoleColor.Yellow);
			}

			Print("\nPress any key to return to the Main Menu...");
			Input.Any();
		}
	}
}
