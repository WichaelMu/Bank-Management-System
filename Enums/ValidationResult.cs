

namespace BankManagementSystem.Core
{
	/// <summary>The result of an <see cref="Account"/>'s <see cref="Account.Validate"/>.</summary>
	public enum EValidationResult : byte
	{
		/// <summary>The validation passed all tests.</summary>
		Passed = 0,
		/// <summary>The Email '@' symbol validation failed.</summary>
		NoAtSymbol = 1,
		/// <summary>The Email '@' symbol validation failed.</summary>
		TooManyAtSymbols = 2,
		/// <summary>The Email domain is not supported.</summary>
		InvalidDomain = 4,
		/// <summary>The Email is not valid.</summary>
		IllegalEmailAddress = 8,
		/// <summary>One or more fields are empty.</summary>
		FieldsEmpty = 16,
		/// <summary>A field contains the delimiter '|'.</summary>
		IllegalCharacters = 32
	}
}