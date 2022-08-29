using System;
using System.Text;
using BankManagementSystem.Core;
using BankManagementSystem.IO;
using static BankManagementSystem.IO.OutputHelpers;

namespace BankManagementSystem
{
	public partial class BMS
	{
		void RunAccountCreationSequence()
		{
			Console.Clear();

			PrintCreateNewAccount();
			ReceiveAccountCreationInput(out Account NewAccount);

			// If the account is Valid.
			if (ValidateNewAccount(NewAccount))
			{
				// If the user Confirms to create an Account.
				if (ConfirmNewAccount())
				{
					// Write to file.
					RegisterNewAccount(NewAccount);

					// Return to Main Menu.
					RunMainMenuSequence();
				}
				// Unconfirmed Account Creation. Retry...
				else
				{
					RunAccountCreationSequence();
				}
			} // Otherwise, handled in Validation.
		}

		void PrintCreateNewAccount()
		{
			// Define Title.
			const string kCreateAccountTitle = "CREATE A NEW ACCOUNT";

			// Define Prompts.
			const string kFirstName = "First Name: ";
			const string kLastName = "Last Name: ";
			const string kAddress = "Address: ";
			const string kPhoneNumber = "Phone: ";
			const string kEmailAddress = "Email: ";

			// Print title with borders.
			PrintTitle(kCreateAccountTitle);

			// Calculate centre-alignment padding spaces.
			AutoCentre(kPrompt, CharacterLimit, out int DetailPadding);

			// Calculate tabbed prompt padding spaces.
			PaddingUntilEnd(kFirstName, CharacterLimit, out int FNPadding);
			PaddingUntilEnd(kLastName, CharacterLimit, out int LNPadding);
			PaddingUntilEnd(kAddress, CharacterLimit, out int APadding);
			PaddingUntilEnd(kPhoneNumber, CharacterLimit, out int PNPadding);
			PaddingUntilEnd(kEmailAddress, CharacterLimit, out int EPadding);

			// Print prompt.
			PrintWithBorder(kPrompt, DetailPadding);
			PrintNewLineWithBorder(CharacterLimit);

			// Print prompt messages.
			PrintWithCustomPadding(kFirstName, kTabSize, FNPadding - 4);
			PrintWithCustomPadding(kLastName, kTabSize, LNPadding - 4);
			PrintWithCustomPadding(kAddress, kTabSize, APadding - 4);
			PrintWithCustomPadding(kPhoneNumber, kTabSize, PNPadding - 4);
			PrintWithCustomPadding(kEmailAddress, kTabSize, EPadding - 4);

			// Ending border.
			PrintWithBorder(HorizontalBorder);
		}

		/// <summary>Receives the details for a new Account.</summary>
		/// <param name="Account">Out the new Account.</param>
		void ReceiveAccountCreationInput(out Account Account)
		{
			// Prepare, position, and read Input.
			Console.SetCursorPosition(17, 5);
			string FirstName = Input.String();

			Console.SetCursorPosition(16, 6);
			string LastName = Input.String();

			Console.SetCursorPosition(14, 7);
			string Address = Input.String();

			// Protect the phone number from being an illegal value.
			int PhoneNumber;
			string IntAsString = string.Empty;

			do
			{
				Console.SetCursorPosition(0, 12);

				// Cannot exceed a length of 10.
				if (IntAsString.Length > 10)
				{
					Print("Phone Numbers do not exceed 10 digits!", ConsoleColor.Red);
				}
				// If IntAsString is null or Empty, and it has reached this point, then we know
				// this has looped more than once and IntAsString contains non-number characters.
				else if (!string.IsNullOrEmpty(IntAsString))
				{
					Print("Phone Numbers can only have numbers!", ConsoleColor.Red);
				}

				// Set the position to the end of the phone number.
				Console.SetCursorPosition(12 + IntAsString.Length, 8);

				// Backspace any illegal characters (non-number string)
				for (int i = 0; i < IntAsString.Length; ++i)
					Backspace();

				IntAsString = Input.String();
			}
			// If the Input is NaN or is > 10, loop.
			while (!int.TryParse(IntAsString, out PhoneNumber) || IntAsString.Length > 10);

			// Read the Email Address as is, we'll Validate it later.
			Console.SetCursorPosition(12, 9);
			string Email = Input.String();

			// Out new Account.
			Account = new Account(FirstName, LastName, Address, PhoneNumber, Email);
		}

		/// <summary>Perform validation checks.</summary>
		/// <returns>True if the AccountToCheck is valid.</returns>
		bool ValidateNewAccount(Account AccountToCheck)
		{
			Console.SetCursorPosition(0, 12);

			// Validate the inputs from before.
			EValidationResult ValidationResult = AccountToCheck.Validate();

			// If the Account has not passed Validation checks.
			if (ValidationResult != 0)
			{
				// Print failed messages. We can consider multiple fails with enums and bitwise operations.

				if ((ValidationResult & EValidationResult.FieldsEmpty) == EValidationResult.FieldsEmpty)
					Print("One or more fields were empty!", ConsoleColor.Red);
				if ((ValidationResult & EValidationResult.NoAtSymbol) == EValidationResult.NoAtSymbol)
					Print("Invalid Email Address! No '@' symbol.", ConsoleColor.Red);
				if ((ValidationResult & EValidationResult.TooManyAtSymbols) == EValidationResult.TooManyAtSymbols)
					Print("Invalid Email Address! Too many '@' symbols.", ConsoleColor.Red);
				if ((ValidationResult & EValidationResult.InvalidDomain) == EValidationResult.InvalidDomain)
					Print("Invalid Email Address Domain!" +
								"\n\tValid Domains are:" +
								"\n\t- @gmail.com" +
								"\n\t- @outlook.com" +
								"\n\t- @student.uts.edu.au" +
								"\n\t- @uts.edu.au", ConsoleColor.Red);
				if ((ValidationResult & EValidationResult.IllegalEmailAddress) == EValidationResult.IllegalEmailAddress)
					Print("Invalid Email Address!", ConsoleColor.Red);
				if ((ValidationResult & EValidationResult.IllegalCharacters) == EValidationResult.IllegalCharacters)
					Print("Illegal Characters in Field/s! Do not use '|'.", ConsoleColor.Red);

				// Prompt for retry or cancellation.
				Print("\nPress any key to retry or 'x' to cancel...");

				Input.Char(out char Keystroke);

				// No cancel; retry...
				if (Keystroke != 'x' && Keystroke != 'X')
				{
					RunAccountCreationSequence();
				}
				// Cancelled...
				else
				{
					RunMainMenuSequence();
				}

				return false;
			}

			return true;
		}

		/// <summary>Asks the user if the new Account's information is correct.</summary>
		/// <returns>True if the information is correct.</returns>
		bool ConfirmNewAccount()
		{
			Print("Is the above information correct (y/n)?");

			Input.Char(out char Key);
			while (Key != 'Y' && Key != 'y' && Key != 'N' && Key != 'n')
			{
				Console.SetCursorPosition(0, 12);
				Print("Invalid Response. Valid inputs are (Y/N) or (y/n)\nIs the above information correct? ", ConsoleColor.Red);
				Input.Char(out Key);
			}

			return Key == 'Y' || Key == 'y';
		}

		/// <summary>Writes a New Account into a new file.</summary>
		void RegisterNewAccount(Account NewAccount)
		{
			Console.SetCursorPosition(0, 12);

			// Get a unique ID.
			int UniqueAccountNumber = AccountParser.Unique;
			NewAccount.ID = UniqueAccountNumber;

			// Send an Email.
			NewAccount.Dispatch();

			Print($"Account Created! Details will be provided via Email to {NewAccount.Email}", ConsoleColor.Green);

			Print($"\n\nYour account number is: {UniqueAccountNumber}", ConsoleColor.Green);

			// Write this new Account to its respective file.
			NewAccount.Write();

			Console.WriteLine("\nPress any key to return to the Main Menu...");
			Input.Any();
		}
	}
}
