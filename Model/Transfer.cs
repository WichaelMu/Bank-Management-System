

namespace BankManagementSystem.Core
{
	public class Transfer
	{
		public string Date;
		public ETransferType Type;
		public int Amount;
		public int Balance;

		public Transfer(string Date, ETransferType Type, int Amount, int Balance)
		{
			this.Date = Date;
			this.Type = Type;
			this.Amount = Amount;
			this.Balance = Balance;
		}

		/// <summary>Gets the corresponding <see cref="ETransferType"/> from a string.</summary>
		public static ETransferType GetType(string Type)
		{
			if (Type == "Deposit")
				return ETransferType.Deposit;
			return ETransferType.Withdraw;
		}

		/// <summary>Sets this type from a string.</summary>
		public ETransferType TypeFromString(string Type)
		{
			this.Type = GetType(Type);

			return this.Type;
		}

		/// <summary>Construct a file-parse-able string from this Transfer.</summary>
		public override string ToString()
		{
			return $"{Date}|{Type}|{Amount}|{Balance}";
		}
	}
}
