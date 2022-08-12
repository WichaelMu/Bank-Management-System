using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BankManagementSystem.Core
{
	public class Account
	{
		public string FirstName;
		public string LastName;
		public string Address;
		public int PhoneNumber;
		public string Email;

		public Account(string FirstName, string LastName, string Address, int PhoneNumber, string Email)
		{
			this.FirstName = FirstName;
			this.LastName = LastName;
			this.Address = Address;
			this.PhoneNumber = PhoneNumber;
			this.Email = Email;
		}

		public EValidationResult Validate()
		{
			if (FirstName.Length == 0 || LastName.Length == 0 || Address.Length == 0 || Email.Length == 0)
				return EValidationResult.FieldsEmpty;

			int NumberOfAtSymbols = Email.Count(Character => Character == '@');
			bool bValidEmailAddress = NumberOfAtSymbols == 1;

			bool bValidDomain = Email.EndsWith("@gmail.com");
			bValidDomain ^= Email.EndsWith("@outlook.com");
			bValidDomain ^= Email.EndsWith("@student.uts.edu.au");
			bValidDomain ^= Email.EndsWith("@uts.edu.au");

			bool bValidEmailPrefix = !Email.StartsWith('@') && char.IsLetter(Email[0]);

			if (bValidEmailAddress && bValidDomain && bValidEmailPrefix)
				return EValidationResult.Passed;

			byte Result = 0;

			if (!bValidDomain)
				Result |= (byte)EValidationResult.InvalidDomain;

			if (!bValidEmailAddress)
				Result |= NumberOfAtSymbols == 0
					? (byte)EValidationResult.NoAtSymbol
					: (byte)EValidationResult.TooManyAtSymbols;

			if (!bValidEmailPrefix)
				Result |= (byte)EValidationResult.IllegalEmailAddress;

			return (EValidationResult)Result;
		}
	}

	/// <summary>The result of an <see cref="Account"/>'s <see cref="Account.Validate"/>.</summary>
	public enum EValidationResult : byte
	{
		Passed = 0,
		NoAtSymbol = 1,
		TooManyAtSymbols = 2,
		InvalidDomain = 4,
		IllegalEmailAddress = 8,
		FieldsEmpty = 16
	}
}
