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
			bool bValidPhoneNumber = PhoneNumber <= int.MaxValue;

			int NumberOfAtSymbols = Email.Count(Character => Character == '@');
			bool bValidEmailAddress = NumberOfAtSymbols == 1;

			bool bValidDomain = Email.EndsWith("@gmail.com");
			bValidDomain &= Email.EndsWith("@outlook.com");
			bValidDomain &= Email.EndsWith("@uts.edu.au");

			if (bValidPhoneNumber && bValidEmailAddress && bValidDomain)
				return EValidationResult.Passed;

			short Result = 0;

			if (!bValidPhoneNumber)
				Result |= (short)EValidationResult.InvalidPhone;

			if (!bValidDomain)
				Result |= (short)EValidationResult.InvalidDomain;

			if (!bValidEmailAddress)
				Result |= NumberOfAtSymbols == 0
					? (short)EValidationResult.NoAtSymbol
					: (short)EValidationResult.TooManyAtSymbols;

			return (EValidationResult)Result;
		}
	}

	public enum EValidationResult : short
	{
		Passed = 0,
		InvalidPhone = 1,
		NoAtSymbol = 2,
		TooManyAtSymbols = 4,
		InvalidDomain = 8
	}
}
