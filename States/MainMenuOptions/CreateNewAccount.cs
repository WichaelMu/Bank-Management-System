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
			if (ValidateNewAccount(NewAccount))
				ConfirmNewAccount(NewAccount);

			RunMainMenuSequence();
		}

		void PrintCreateNewAccount()
		{
			const string kCreateAccountTitle = "CREATE A NEW ACCOUNT";

			const string kFirstName = "First Name: ";
			const string kLastName = "Last Name: ";
			const string kAddress = "Address: ";
			const string kPhoneNumber = "Phone: ";
			const string kEmailAddress = "Email: ";

			PrintTitle(kCreateAccountTitle);

			AutoCentre(kPrompt, CharacterLimit, out int DetailPadding);

			PaddingUntilEnd(kFirstName, CharacterLimit, out int FNPadding);
			PaddingUntilEnd(kLastName, CharacterLimit, out int LNPadding);
			PaddingUntilEnd(kAddress, CharacterLimit, out int APadding);
			PaddingUntilEnd(kPhoneNumber, CharacterLimit, out int PNPadding);
			PaddingUntilEnd(kEmailAddress, CharacterLimit, out int EPadding);

			PrintWithBorder(kPrompt, DetailPadding);
			PrintNewLineWithBorder(CharacterLimit);

			PrintWithCustomPadding(kFirstName, kTabSize, FNPadding - 4, bWithBorder: true);
			PrintWithCustomPadding(kLastName, kTabSize, LNPadding - 4, bWithBorder: true);
			PrintWithCustomPadding(kAddress, kTabSize, APadding - 4, bWithBorder: true);
			PrintWithCustomPadding(kPhoneNumber, kTabSize, PNPadding - 4, bWithBorder: true);
			PrintWithCustomPadding(kEmailAddress, kTabSize, EPadding - 4, bWithBorder: true);

			PrintWithBorder(HorizontalBorder);
		}

		void ReceiveAccountCreationInput(out Account Account)
		{
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
					Console.WriteLine("Phone Numbers do not exceed 10 digits!");
				}
				else if (!string.IsNullOrEmpty(IntAsString))
				{
					Console.WriteLine("Phone Numbers can only have numbers!  ");
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

			Console.SetCursorPosition(12, 9);
			string Email = Input.String();

			Account = new Account(FirstName, LastName, Address, PhoneNumber, Email);
		}

		bool ValidateNewAccount(Account AccountToCheck)
		{
			Console.SetCursorPosition(0, 12);

			EValidationResult ValidationResult = AccountToCheck.Validate();

			if (ValidationResult != 0)
			{
				if ((ValidationResult & EValidationResult.FieldsEmpty) == EValidationResult.FieldsEmpty)
					Console.WriteLine("One or more fields were empty!              ");
				if ((ValidationResult & EValidationResult.NoAtSymbol) == EValidationResult.NoAtSymbol)
					Console.WriteLine("Invalid Email Address! No '@' symbol.       ");
				if ((ValidationResult & EValidationResult.TooManyAtSymbols) == EValidationResult.TooManyAtSymbols)
					Console.WriteLine("Invalid Email Address! Too many '@' symbols.");
				if ((ValidationResult & EValidationResult.InvalidDomain) == EValidationResult.InvalidDomain)
					Console.WriteLine("Invalid Email Address Domain!                     " +
								"\n\tValid Domains are:" +
								"\n\t- @gmail.com" +
								"\n\t- @outlook.com" +
								"\n\t- @student.uts.edu.au" +
								"\n\t- @uts.edu.au");
				if ((ValidationResult & EValidationResult.IllegalEmailAddress) == EValidationResult.IllegalEmailAddress)
					Console.WriteLine("Invalid Email Address!");

				Console.WriteLine("\nPress any key to retry...");
				Input.Any();
				RunAccountCreationSequence();
				return false;
			}
			else
			{
				Console.WriteLine("Is the above information correct (y/n)?");

				return true;
			}
		}

		async void ConfirmNewAccount(Account NewAccount)
		{
			Input.Char(out char Key);
			while (Key != 'Y' && Key != 'y' && Key != 'N' && Key != 'n')
			{
				Console.SetCursorPosition(0, 12);
				Console.Write("Invalid Response. Valid inputs are (Y/N) or (y/n)\nIs the above information correct? ");
				Input.Char(out Key);
			}

			if (Key == 'Y' || Key == 'y')
			{
				int UniqueAccountNumber = AccountParser.Unique;
				NewAccount.ID = UniqueAccountNumber;

				NewAccount.DispatchDetails();

				Console.WriteLine($"Account Created! Details will be provided via Email to {NewAccount.Email}");

				Console.WriteLine($"\n\nYour account number is: {UniqueAccountNumber}");

				string FirstName = $"First Name|{NewAccount.FirstName}";
				string LastName = $"Last Name|{NewAccount.LastName}";
				string Address = $"Address|{NewAccount.Address}";
				string Phone = $"Phone|{NewAccount.PhoneNumber}";
				string EmailAddress = $"Email|{NewAccount.Email}";
				string AccountNumber = $"AccountNo|{UniqueAccountNumber}";
				string Balance = "Balance|0"; // Begin is zero Balance.

				await FileSystem.WriteToFile(FileSystem.kDirectory, UniqueAccountNumber.ToString() + ".txt", EWriteMode.Overwrite, Encoding.UTF8,
					FirstName,
					LastName,
					Address,
					Phone,
					EmailAddress,
					AccountNumber,
					Balance
				);

				Console.WriteLine("\nPress any key to return to the Main Menu...");
				Input.Any();
			}
			else
			{
				RunAccountCreationSequence();
			}
		}
	}
}
