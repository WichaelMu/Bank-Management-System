using System;
using System.Collections.Generic;
using System.Text;

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

		public static ETransferType GetType(string Type)
		{
			if (Type == "Deposit")
				return ETransferType.Deposit;
			return ETransferType.Withdraw;
		}

		public ETransferType TypeFromString(string Type)
		{
			this.Type = GetType(Type);

			return this.Type;
		}
	}

	public enum ETransferType
	{
		Deposit, Withdraw
	}
}
