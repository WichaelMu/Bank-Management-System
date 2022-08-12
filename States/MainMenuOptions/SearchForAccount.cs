﻿using System;
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

			PrintWithCustomPadding(kAccountNumber, kTabSize, AccountNumberPadding - 4, bWithBorder: true);
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
					Console.WriteLine("Account Numbers do not exceed 8 digits!");
				}
				// If not empty and is executed, then it has previously failed with letters.
				else if (!string.IsNullOrEmpty(IntAsString))
				{
					Console.WriteLine("Account Numbers can only have numbers! ");
				}

				// Set the position to the end of the Account Number.
				Console.SetCursorPosition(21 + IntAsString.Length, 5);

				// Backspace any illegal characters (non-number string)
				for (int i = 0; i < IntAsString.Length; ++i)
					Backspace();

				IntAsString = Input.String();
			}
			// If the Input is NaN or is > 10, loop.
			while (!int.TryParse(IntAsString, out AccountNumber) || IntAsString.Length > 10);

			Console.SetCursorPosition(0, 8);

			if (!SearchAccountID(IntAsString))
			{
				Console.WriteLine($"Account Number {IntAsString} does not exist!        ");

				RequestCheckAnother(false);
				return false;
			}
			else
			{
				Console.Clear();
				Console.WriteLine("Account found!                         ");
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
			return FileSystem.FileExists(FileSystem.kDirectory, ID + ".txt");
		}

		/// <summary>Prints the details of an Account of ID.</summary>
		public void PrintAccount(int ID)
		{
			// Title constant.
			const string kAccountTitle = "ACCOUNT DETAILS";

			// Field constants.
			const string kAccountNo = "Account No: ";
			const string kBalance = "Account Balance: $";
			const string kFirstName = "First Name: ";
			const string kLastName = "Last Name: ";
			const string kAddress = "Address: ";
			const string kPhone = "Phone: ";
			const string kEmail = "Email: ";

			Account FromID = AccountParser.ConstructFromFile(ID);

			// Build fields with information.
			string AccountNo = $"{kAccountNo}{ID}";
			string AccountBalance = $"{kBalance}{FromID.Balance}";
			string FirstName = $"{kFirstName}{FromID.FirstName}";
			string LastName = $"{kLastName}{FromID.LastName}";
			string Address = $"{kAddress}{FromID.Address}";
			string Phone = $"{kPhone}{FromID.PhoneNumber}";
			string Email = $"{kEmail}{FromID.Email}";

			// Align fields.
			PaddingUntilEnd(AccountNo, CharacterLimit, out int ANPadding);
			PaddingUntilEnd(AccountBalance, CharacterLimit, out int ABPadding);
			PaddingUntilEnd(FirstName, CharacterLimit, out int FNPadding);
			PaddingUntilEnd(LastName, CharacterLimit, out int LNPadding);
			PaddingUntilEnd(Address, CharacterLimit, out int APadding);
			PaddingUntilEnd(Phone, CharacterLimit, out int PPadding);
			PaddingUntilEnd(Email, CharacterLimit, out int EPadding);

			Console.WriteLine();

			PrintTitle(kAccountTitle);
			PrintNewLineWithBorder(CharacterLimit);

			PrintWithCustomPadding(AccountNo, kTabSize, ANPadding - 4, bWithBorder: true);
			PrintWithCustomPadding(AccountBalance, kTabSize, ABPadding - 4, bWithBorder: true);
			PrintWithCustomPadding(FirstName, kTabSize, FNPadding - 4, bWithBorder: true);
			PrintWithCustomPadding(LastName, kTabSize, LNPadding - 4, bWithBorder: true);
			PrintWithCustomPadding(Address, kTabSize, APadding - 4, bWithBorder: true);
			PrintWithCustomPadding(Phone, kTabSize, PPadding - 4, bWithBorder: true);
			PrintWithCustomPadding(Email, kTabSize, EPadding - 4, bWithBorder: true);

			PrintWithBorder(HorizontalBorder);
		}

		/// <summary>Ask the user whether or not to search for another <see cref="Account"/>.</summary>
		void RequestCheckAnother(bool bHasAccountAlreadyOnScreen)
		{
			Console.Write("\nCheck another account (y/n)? ");

			Input.Char(out char Key);
			while (Key != 'Y' && Key != 'y' && Key != 'N' && Key != 'n')
			{
				Console.SetCursorPosition(0, bHasAccountAlreadyOnScreen
					? 15
					: 8
				);

				Console.Write("Invalid Response. Valid inputs are (Y/N) or (y/n)\nDo you want to check another account? ");
				Input.Char(out Key);
			}

			if (Key == 'Y' || Key == 'y')
			{
				RunAccountSearchSequence();
			}
			else
			{
				RunMainMenuSequence();
			}
		}
	}
}