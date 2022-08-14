using System;
using BankManagementSystem.IO;
using static System.ConsoleKey;
using static BankManagementSystem.IO.OutputHelpers;

namespace BankManagementSystem
{
	public partial class BMS
	{
		void RunMainMenuSequence()
		{
			Console.Clear();

			PrintMainMenu();
			ReceiveMainMenuInput();
		}

		void PrintMainMenu()
		{
			string[] Prompts = new string[]
			{
				"Enter your choice (1-7):",
				"1. Create a new account",
				"2. Search for an account",
				"3. Deposit",
				"4. Withdraw",
				"5. A/C statement",
				"6. Delete account",
				"7. Exit"
			};

			PrintTitle(kWelcomeMessage);

			for (int i = 1; i < 8; ++i)
			{
				PaddingUntilEnd(Prompts[i], CharacterLimit, out int Padding);
				PrintWithCustomPadding(Prompts[i], kTabSize, Padding - 4);
			}

			PrintWithBorder(HorizontalBorder);
			PaddingUntilEnd(Prompts[0], CharacterLimit, out int PromptPadding);
			PrintWithCustomPadding(Prompts[0], kTabSize, PromptPadding - 4);
			PrintWithBorder(HorizontalBorder);
		}

		void ReceiveMainMenuInput()
		{
			ConsoleKey Choice = Input.Key();

			switch (Choice)
			{
				case D1: // Create a new account.
					RunAccountCreationSequence();
					break;
				case D2: // Search for an account.
					RunAccountSearchSequence();
					break;
				case D3: // Deposit.
					RunDepositSequence();
					break;
				case D4: // Withdraw.
					RunWithdrawSequence();
					break;
				case D5: // A/C Statement.
					break;
				case D6: // Delete account.
					break;
				case D7: // Exit.
					break;
				default: // Illegal Input.
					break;
			}
		}
	}
}
