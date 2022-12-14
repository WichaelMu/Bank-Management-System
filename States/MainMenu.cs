using System;
using BankManagementSystem.IO;
using static System.ConsoleKey;
using static BankManagementSystem.IO.Output;

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
			// Map 0 to the prompt/choice input.
			// Map 1 - 7 = The available choices.
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

			// Print the title.
			PrintTitle(kWelcomeMessage);

			// Loop over and print the Prompts.
			for (int i = 1; i < 8; ++i)
			{
				PaddingUntilEnd(Prompts[i], out int Padding);
				PrintWithCustomPadding(Prompts[i], kTabSize, Padding - 4);
			}

			// Separating Border.
			PrintWithBorder(HorizontalBorder);

			// Print request for Input.
			PaddingUntilEnd(Prompts[0], out int PromptPadding);
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
					RunStatmentSequence();
					break;
				case D6: // Delete account.
					RunDeleteSequence();
					break;
				case D7: // Exit.
					// Do not exit the program until we have sent all pending Emails.
					if (Email.IsAwaitingAsyncEmail())
					{
						Email.SetAllowEmailSendResultsPrinting(true);

						Console.SetCursorPosition(0, 14);
						Print("Cannot exit right now, we're still sending Emails!", ConsoleColor.Yellow);
						ReceiveMainMenuInput();
					}

					MainProgram.Terminate();
					return;
				default: // Illegal Input.
					Console.SetCursorPosition(0, 11);

					const string kRetryInput = "Please Enter a Choice (1-7): ";

					PaddingUntilEnd(kRetryInput, out int PromptPadding);

					SetColours(ConsoleColor.Red);
					PrintWithCustomPadding(kRetryInput, kTabSize, PromptPadding - 4);
					ResetColours();

					ReceiveMainMenuInput();
					break;
			}
		}
	}
}
