using System;
using BankManagementSystem.Core;
using BankManagementSystem.IO;
using static BankManagementSystem.IO.OutputHelpers;

namespace BankManagementSystem
{
	public partial class BMS
	{
		void RunWithdrawSequence()
		{
			Console.Clear();

			PrintWithdrawPrompts();
			ReceiveWithdrawInputs();

			RunMainMenuSequence();
		}

		void PrintWithdrawPrompts()
		{
			const string kWithdrawTitle = "WITHDRAW";

			const string kAccountNumberPrompt = "Account Number: ";
			const string kAmount = "Amount: $";

			PrintTitle(kWithdrawTitle);

			AutoCentre(kPrompt, CharacterLimit, out int PromptPadding);

			PrintWithBorder(kPrompt, PromptPadding);
			PrintNewLineWithBorder(CharacterLimit);

			PaddingUntilEnd(kAccountNumberPrompt, CharacterLimit, out int ANPadding);
			PaddingUntilEnd(kAmount, CharacterLimit, out int APadding);

			PrintWithCustomPadding(kAccountNumberPrompt, kTabSize, ANPadding - 4);
			PrintWithCustomPadding(kAmount, kTabSize, APadding - 4);

			PrintWithBorder(HorizontalBorder);
		}

		void ReceiveWithdrawInputs()
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
					Print("Account Numbers do not exceed 8 digits!", ConsoleColor.Red);
				}
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

				if (IntAsString == "x" || IntAsString == "X")
				{
					return;
				}
			}
			// If the Input is NaN or is > 10, loop.
			while (!int.TryParse(IntAsString, out AccountNumber) || IntAsString.Length > 10 || !SearchAccountID(AccountNumber));

			Console.SetCursorPosition(14, 6);

			// Protect the Account Number from being an illegal value.
			int Amount;
			string AmountAsString = string.Empty;
			Account FromID = AccountParser.ConstructFromFile(AccountNumber);

			do
			{
				Console.SetCursorPosition(0, 9);
				ClearLine();

				// Cannot exceed a length of 10.
				if (AmountAsString.Length > 10)
				{
					// 1 << 31 is already 10 digits. Prevent overflow.
					Print("Account Numbers do not exceed 10 digits!", ConsoleColor.Red);
				}
				// If AmountAsString IS a number.
				else if (int.TryParse(AmountAsString, out int TriedAmount))
				{
					// If the Account doesn't have enough Balance.
					if (FromID.Balance < TriedAmount)
						Print($"{FromID.GetDecoratedName()} Account does not have enough balance!", ConsoleColor.Red);

					// If the Amount is negative.
					if (TriedAmount < 0)
						Print("The Withdraw Amount must be non-negative!", ConsoleColor.Red);
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
			// If the Input is NaN or the Account does not have enough Balance, loop.
			while (!int.TryParse(AmountAsString, out Amount) || Amount < 0 || FromID.Balance < Amount);

			Console.SetCursorPosition(0, 9);

			// Withdrawing Amount from Account.
			if (Amount != 0)
			{
				// Update and Write Account to file.
				FromID.Balance -= Amount;
				FromID.Transfers.Add(new Transfer(FormatDate(), ETransferType.Withdraw, Amount, FromID.Balance));
				FromID.Write();

				Print($"Successfully Withdrew ${Amount} from {FromID.GetDecoratedName()} Account!", ConsoleColor.Green);
			}
			// Withdrawing $0.00 does nothing.
			else
			{
				Print($"A Withdraw of $0.00 was attempted! A Withdraw into {FromID.GetDecoratedName()} Account has been cancelled.", ConsoleColor.Yellow);
			}

			Print("\nPress any key to return to the Main Menu...");
			Input.Any();
		}
	}
}
