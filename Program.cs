using System;

namespace BankManagementSystem
{
	class Program
	{
		static void Main(string[] args)
		{
			// Bank Management System begins execution here...
			BMS BankManagementSystem = new BMS();
			BankManagementSystem.Execute();

			// Bank Management System terminates here...
			Console.WriteLine("Press Any Key to Terminate...");
			Console.ReadKey();
		}
	}
}
