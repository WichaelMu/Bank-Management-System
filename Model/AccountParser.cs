using System;
using System.Collections.Generic;
using BankManagementSystem.IO;

namespace BankManagementSystem.Core
{
	public static class AccountParser
	{
		public static Dictionary<string, string> ReadLogins()
		{
			if (FileSystem.ReadFromFile(FileSystem.kDirectory, FileSystem.kFileName, out List<string> LoginData))
			{
				Dictionary<string, string> Logins = new Dictionary<string, string>();

				foreach (string Credential in LoginData)
				{
#if DEBUG
					if (!Credential.Contains('|'))
						throw new ArgumentException("Login Details are not correctly configured for: " + Credential);
#endif
					string[] UsernameAndPassword = Credential.Split('|');
#if DEBUG
					if (UsernameAndPassword.Length != 2)
						throw new ArgumentException("Login details has no Username OR Password for " + Credential);

					if (Logins.ContainsKey(UsernameAndPassword[0]))
						throw new ArgumentException("Duplicate Key Found! Login failed checks for: " + Credential);
#endif
					Logins.Add(UsernameAndPassword[0], UsernameAndPassword[1]);
				}

				return Logins;
			}

			throw new System.IO.IOException("Unable to open ./login.txt");
		}

		/// <summary>Retrieves and Constructs an <see cref="Account"/> from a file with the corresponding <see cref="Account.ID"/>.</summary>
		/// <param name="ID">The <see cref="Account.ID"/>.</param>
		/// <returns>The <see cref="Account"/> associated with <paramref name="ID"/>.</returns>
		public static Account ConstructFromFile(int ID)
		{
			if (FileSystem.ReadFromFile(FileSystem.kDirectory, ID.ToString() + ".txt", out List<string> AccountDetails))
			{
				// Declare Account details.
				int AccountNumber = ID;
				string[] CoreDetails = new string[5];
				int Balance = 0;
				List<Transfer> Transfers = new List<Transfer>();

				for (int i = 0; i < AccountDetails.Count; ++i)
				{
					// The format in the Account files are <field>|<value>
					string[] Separate = AccountDetails[i].Split('|');

					// The F/L Names, Address, Phone, and Email are the first 4 lines in the file.
					if (i < 5)
					{
						CoreDetails[i] = Separate[1];
					}
					// The 5th line is the Account's Number.
					else if (i == 5)
					{
						AccountNumber = int.Parse(Separate[1]);
					}
					// The 6th line is the Account's Balance.
					else if (i == 6)
					{
						Balance = int.Parse(Separate[1]);
					}
					// Any subsequent line is a transfer (Withdraw | Deposit).
					else
					{
						string Date = Separate[0];
						ETransferType Type = Transfer.GetType(Separate[1]);
						int Amount = int.Parse(Separate[2]);
						int TBalance = int.Parse(Separate[3]);

						Transfers.Add(new Transfer(Date, Type, Amount, TBalance));
					}
				}

				// Construct the Core Details.
				string FirstName = CoreDetails[0];
				string LastName = CoreDetails[1];
				string Address = CoreDetails[2];
				int Phone = int.Parse(CoreDetails[3]);
				string Email = CoreDetails[4];

				// Construct a new Account with other miscellaneous information.
				Account FromFile = new Account(FirstName, LastName, Address, Phone, Email);
				FromFile.ID = AccountNumber;
				FromFile.Balance = Balance;
				FromFile.Transfers = Transfers;

				return FromFile;
			}

			// No file exists. This should never be seen. This edge case should be handled by the caller.
			return null;
		}

		/// <summary>The next Account Number.</summary>
		public static int Unique { get => GetNextAccountNumber(); }
		static int UniqueAccountNumber = 10000001;

		static int GetNextAccountNumber()
		{
			while (FileSystem.FileExists(FileSystem.kDirectory, UniqueAccountNumber.ToString() + ".txt"))
			{
				++UniqueAccountNumber;
			}

			return UniqueAccountNumber;
		}
	}
}
