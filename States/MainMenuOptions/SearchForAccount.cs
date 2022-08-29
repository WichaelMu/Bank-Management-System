using System;
using BankManagementSystem.Core;
using BankManagementSystem.IO;
using static BankManagementSystem.IO.OutputHelpers;

namespace BankManagementSystem
{
	public partial class BMS
	{
		void RunAccountSearchSequence()
		{
			Console.Clear();

			PrintAccountSearch();
			if (ReceiveRequestedAccountNumber())
				RequestCheckAnother(true);
		}

		void PrintAccountSearch()
		{
			const string kSearchAccountTitle = "SEARCH AN ACCOUNT";
			const string kAccountNumber = "Account Number: ";

			PrintTitle(kSearchAccountTitle);

			AutoCentre(kPrompt, CharacterLimit, out int PromptPadding);
			PaddingUntilEnd(kAccountNumber, CharacterLimit, out int AccountNumberPadding);

			PrintWithBorder(kPrompt, PromptPadding);
			PrintNewLineWithBorder(CharacterLimit);

			PrintWithCustomPadding(kAccountNumber, kTabSize, AccountNumberPadding - 4);
			PrintWithBorder(HorizontalBorder);
		}

		bool ReceiveRequestedAccountNumber()
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
					return false;
				}
			}
			// If the Input is NaN or is > 10, loop.
			while (!int.TryParse(IntAsString, out AccountNumber) || IntAsString.Length > 10);

			Console.SetCursorPosition(0, 8);
			
			// Look for the Account and print the relevant messages or information.

			if (!SearchAccountID(IntAsString))
			{
				Print($"Account Number {IntAsString} does not exist!", ConsoleColor.Yellow);

				RequestCheckAnother(false);
				return false;
			}
			else
			{
				Console.Clear();
				Print("Account found!", ConsoleColor.Green);
				PrintAccount(AccountNumber);
				return true;
			}
		}

		/// <summary>Checks whether an <see cref="Account"/> with ID exists in the <see cref="FileSystem"/>.</summary>
		/// <param name="ID">The ID to check.</param>
		/// <returns><see langword="true"/> if an Account with ID exists.</returns>
		public bool SearchAccountID(int ID)
		{
			return SearchAccountID(ID.ToString());
		}

		/// <inheritdoc cref="SearchAccountID(int)"/>
		public bool SearchAccountID(string ID)
		{
			// An Account exists if we have a file associated with the given ID.
			return FileSystem.FileExists(FileSystem.kDirectory, ID + ".txt");
		}

		/// <summary>Prints the details of an Account of ID.</summary>
		public void PrintAccount(int ID)
		{
			// Title constant.
			const string kAccountTitle = "ACCOUNT DETAILS";

			// Field constants.
			const string kAccountNo = "Account No: ";
			const string kBalance = "Account Balance: ";
			const string kFirstName = "First Name: ";
			const string kLastName = "Last Name: ";
			const string kAddress = "Address: ";
			const string kPhone = "Phone: ";
			const string kEmail = "Email: ";

			Account FromID = AccountParser.ConstructFromFile(ID);

			// Build fields with information.
			string AccountNo = $"{kAccountNo}{ID}";
			string AccountBalance = $"{kBalance}{FromID.Balance:C0}";
			string FirstName = $"{kFirstName}{FromID.FirstName}";
			string LastName = $"{kLastName}{FromID.LastName}";
			string Address = $"{kAddress}{FromID.Address}";
			string Phone = $"{kPhone}{FromID.PhoneNumber:D10}";
			string Email = $"{kEmail}{FromID.Email}";

			// Align fields.
			PaddingUntilEnd(AccountNo, CharacterLimit, out int ANPadding);
			PaddingUntilEnd(AccountBalance, CharacterLimit, out int ABPadding);
			PaddingUntilEnd(FirstName, CharacterLimit, out int FNPadding);
			PaddingUntilEnd(LastName, CharacterLimit, out int LNPadding);
			PaddingUntilEnd(Address, CharacterLimit, out int APadding);
			PaddingUntilEnd(Phone, CharacterLimit, out int PPadding);
			PaddingUntilEnd(Email, CharacterLimit, out int EPadding);

			// Blank line.
			Console.WriteLine();

			// Write Title.
			PrintTitle(kAccountTitle);
			PrintNewLineWithBorder(CharacterLimit);

			// Write fields.
			PrintWithCustomPadding(AccountNo, kTabSize, ANPadding - 4);
			PrintWithCustomPadding(AccountBalance, kTabSize, ABPadding - 4);
			PrintWithCustomPadding(FirstName, kTabSize, FNPadding - 4);
			PrintWithCustomPadding(LastName, kTabSize, LNPadding - 4);
			PrintWithCustomPadding(Address, kTabSize, APadding - 4);
			PrintWithCustomPadding(Phone, kTabSize, PPadding - 4);
			PrintWithCustomPadding(Email, kTabSize, EPadding - 4);

			// Ending border.
			PrintWithBorder(HorizontalBorder);
		}

		/// <summary>Ask the user whether or not to search for another <see cref="Account"/>.</summary>
		void RequestCheckAnother(bool bHasAccountAlreadyOnScreen)
		{
			Print("\nCheck another account (y/n)? ");

			Input.Char(out char Key);
			while (Key != 'Y' && Key != 'y' && Key != 'N' && Key != 'n')
			{
				Console.SetCursorPosition(0, bHasAccountAlreadyOnScreen
					? 15
					: 8
				);

				Print("Invalid Response. Valid inputs are (Y/N) or (y/n)\nDo you want to check another account? ", ConsoleColor.Red);
				ClearLine();
				Input.Char(out Key);
			}

			// If the User wants to Search another Account, run this entire Sequence again.
			if (Key == 'Y' || Key == 'y')
			{
				RunAccountSearchSequence();
			}
			// Otherwise, go back to the Main Menu.
			else
			{
				RunMainMenuSequence();
			}
		}
	}
}
