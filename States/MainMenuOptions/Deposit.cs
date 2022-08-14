using System;
using BankManagementSystem.Core;
using BankManagementSystem.IO;
using static BankManagementSystem.IO.OutputHelpers;

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

			AutoCentre(kPrompt, CharacterLimit, out int PromptPadding);

			PrintWithBorder(kPrompt, PromptPadding);
			PrintNewLineWithBorder(CharacterLimit);

			PaddingUntilEnd(kAccountNumberPrompt, CharacterLimit, out int ANPadding);
			PaddingUntilEnd(kAmount, CharacterLimit, out int APadding);

			PrintWithCustomPadding(kAccountNumberPrompt, kTabSize, ANPadding - 4, bWithBorder: true);
			PrintWithCustomPadding(kAmount, kTabSize, APadding - 4, bWithBorder: true);

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

				bool bInputWasEmpty = !string.IsNullOrEmpty(IntAsString);

				// Cannot exceed a length of 10.
				if (IntAsString.Length > 10)
				{
					Console.WriteLine("Account Numbers do not exceed 8 digits!                               ");
				}
				else if (int.TryParse(IntAsString, out _) && !SearchAccountID(IntAsString))
				{
					Console.WriteLine($"Account Number {IntAsString} does not exist!                         ");
				}
				// If not empty and is executed, then it has previously failed with letters.
				else if (bInputWasEmpty)
				{
					Console.WriteLine("Account Numbers can only have numbers! Use 'x' to Cancel.");
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

				// Cannot exceed a length of 10.
				if (AmountAsString.Length > 10)
				{
					// 1 << 31 is already 10 digits. Prevent overflow.
					Console.WriteLine("Account Numbers do not exceed 10 digits!");
				}
				else if (int.TryParse(AmountAsString, out int TriedAmount) && TriedAmount < 0)
				{
					Console.WriteLine("A Deposit Amount must be non-negative!");
				}

				// Set the position to the end of the Account Number.
				Console.SetCursorPosition(14 + AmountAsString.Length, 6);

				// Backspace any illegal characters (non-number string)
				for (int i = 0; i < AmountAsString.Length; ++i)
					Backspace();

				AmountAsString = Input.String();
			}
			// If the Input is NaN, loop.
			while (!int.TryParse(AmountAsString, out Amount) || Amount < 0);

			Console.SetCursorPosition(0, 9);

			if (Amount != 0)
			{
				// Update and Write Account to file.
				FromID.Balance += Amount;
				FromID.Transfers.Add(new Transfer(FormatDate(), ETransferType.Deposit, Amount, FromID.Balance));
				FromID.Write();

				Console.WriteLine($"Successfully Deposited ${Amount} into {FromID.GetDecoratedName()} Account!");
			}
			else
			{
				Console.WriteLine($"A Deposit of $0.00 was attempted! A Deposit into {FromID.GetDecoratedName()} Account has been cancelled.");
			}

			Console.WriteLine("\nPress any key to return to the Main Menu...");
			Input.Any();
		}
	}
}
