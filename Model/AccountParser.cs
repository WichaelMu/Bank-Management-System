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
