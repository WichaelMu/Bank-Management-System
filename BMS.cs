using System;
using System.Text;
using System.Collections.Generic;
using static BankManagementSystem.Helpers.OutputHelpers;
using BankManagementSystem.IO;
using BankManagementSystem.Core;

namespace BankManagementSystem
{
	public class BMS
	{
		// Define the padding size.
		const int kBorderLeftRightPadding = 5;
		// The tab size of content within borders.
		const int kTabSize = 4;
		// The character representing horizontal bar separators.
		const char kHorizontalBar = '=';

		const string kWelcomeMessage = "WELCOME TO SIMPLE BANKING SYSTEM";

		public static int CharacterLimit;
		public static string HorizontalBorder;

		public BMS()
		{
			Construct();
		}

		static void Construct()
		{
			// The width of the console; the border limits.
			CharacterLimit = kWelcomeMessage.Length + kBorderLeftRightPadding * 2;
			// The string holding an entire horizontal border separator.
			HorizontalBorder = new string(kHorizontalBar, CharacterLimit);
		}

		public void Execute()
		{
			RunLoginSequence();
		}

		#region Welcome/Login

		void RunLoginSequence()
		{
			PrintLoginScreen();
			ReceiveLoginInput(out string Username, out string Password);
			ValidateLoginCredentials(Username, Password);
		}

		void RunMainMenuSequence()
		{
			PrintMainMenu();
		}

		const string kUserNamePrompt = "User Name: ";
		const string kPasswordPrompt = "Password: ";

		/// <summary>Prints the login screen.</summary>
		void PrintLoginScreen()
		{
			const string kLoginMessage = "LOGIN TO START";

			// Welcome Title
			PrintTitle(kWelcomeMessage);

			// Calculate login alignments.

			// Centre-align the Login prompt.
			AutoCentre(kLoginMessage, CharacterLimit, out int LoginPromptPadding);

			// Neatly align the Username and Password Prompts.
			PaddingUntilEnd(kUserNamePrompt, CharacterLimit, out int UserNamePromptPadding);
			PaddingUntilEnd(kPasswordPrompt, CharacterLimit, out int PasswordPromptPadding);

			// Login Prompt
			PrintWithBorder(kLoginMessage, LoginPromptPadding);
			PrintNewLineWithBorder(CharacterLimit);

			// Prompt for Username and Password.
			PrintWithCustomPadding(kUserNamePrompt, kTabSize, UserNamePromptPadding - 4, bWithBorder: true);
			PrintWithCustomPadding(kPasswordPrompt, kTabSize, PasswordPromptPadding - 4, bWithBorder: true);

			// Close Welcome/Login prompt border.
			PrintWithBorder(HorizontalBorder);
		}

		/// <summary>Reads Username/Password inputs.</summary>
		/// <param name="Username">The Username input.</param>
		/// <param name="Password">The raw Password input.</param>
		void ReceiveLoginInput(out string Username, out string Password)
		{
			// Prompt.Length + TabSize gets the end of the prompt. + 1 to include the left vertical border.

			Console.SetCursorPosition(kUserNamePrompt.Length + kTabSize + 1, 5); // Position for Username input.
			Username = Console.ReadLine();                                           // Read username.
			Console.SetCursorPosition(kPasswordPrompt.Length + kTabSize + 1, 6); // Position for Password input.

			// Mask the Password input from the Console.
			ConsoleKeyInfo Key;
			StringBuilder PasswordBuilder = new StringBuilder();
			do
			{
				// Read input, but do not display it on the Console.
				Key = Console.ReadKey(intercept: true);

				// If a valid Password character, add it to the Password input.
				if (!char.IsControl(Key.KeyChar))
				{
					Console.Write('*');
					PasswordBuilder.Append(Key.KeyChar);
				}
				// Otherwise, remove one if Backspace was pressed.
				else if (Key.Key == ConsoleKey.Backspace && PasswordBuilder.Length > 0)
				{
					Console.Write("\b \b");
					PasswordBuilder.Remove(PasswordBuilder.Length - 1, 1);
				}

			}
			while (Key.Key != ConsoleKey.Enter); // Keep listening for Password input until 'Enter' is pressed.

			// Construct Password input.
			Password = PasswordBuilder.ToString();

			// Set Cursor to two spaces under the Welcome/Login Prompt.
			Console.SetCursorPosition(0, 9);
		}

		/// <summary>
		/// Checks whether <paramref name="Username"/> exists in a file with the
		/// corresponding <paramref name="Password"/>.
		/// </summary>
		async void ValidateLoginCredentials(string Username, string Password)
		{
			if (AccountParser.ReadLogins().ContainsKey(Username))
			{
				Console.Clear();

				PrintLoginScreen();
				Console.WriteLine();
				Console.WriteLine("That username already exists! Please try again...");

				ReceiveLoginInput(out Username, out Password);
				ValidateLoginCredentials(Username, Password);
			}
			else
			{
				await FileSystem.AddNewLogin(Username, Password);
				RunMainMenuSequence();
			}
		}

		#endregion

		void PrintMainMenu()
		{
			string[] Prompts = new string[]
			{
				"Enter your choice (1-7):",
				"1. Create a new account",
				"2. Search for an account",
				"3. Deposit",
				"4. Widthdraw",
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

			if (Console.ReadKey().Key == ConsoleKey.D1)
			{
				Console.Clear();
				RunLoginSequence();
			}
		}
	}
}
