

namespace BankManagementSystem.Core
{
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