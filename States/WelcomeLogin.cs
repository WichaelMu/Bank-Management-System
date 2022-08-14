//#define WITH_CREDITS

using System;
using System.Collections.Generic;
using System.Text;
using BankManagementSystem.Core;
using static BankManagementSystem.IO.OutputHelpers;

namespace BankManagementSystem
{
	public partial class BMS
	{
		void RunLoginSequence()
		{
			Console.Clear();

			PrintLoginScreen();
			ReceiveLoginInput(out string Username, out string Password);
			ValidateLoginCredentials(Username, Password);

			RunMainMenuSequence();
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

#if WITH_CREDITS
			Console.WriteLine("\n\n");

			const string kAuthorTitle = "Application Development with .NET";
			const string kName = "Written by: Michael Wu";

			AutoCentre(kAuthorTitle, CharacterLimit, out int ADNETPadding);
			AutoCentre(kName, CharacterLimit, out int MWPadding);

			PrintWithBorder(kAuthorTitle, ADNETPadding, Border: ' ');
			PrintWithBorder(kName, MWPadding, Border: ' ');
#endif
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
					Backspace();
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
		void ValidateLoginCredentials(string Username, string Password)
		{
			Dictionary<string, string> Logins = AccountParser.ReadLogins();
			bool bUsernameExists = Logins.ContainsKey(Username);
			bool bLoginMatches = bUsernameExists && Logins[Username] == Password;

			if (!bLoginMatches)
			{
				Console.Clear();

				PrintLoginScreen();
				Console.WriteLine();
				Console.WriteLine("Incorrect Username or Password! Please try again...");

				ReceiveLoginInput(out Username, out Password);
				ValidateLoginCredentials(Username, Password);
			}
		}
	}
}
