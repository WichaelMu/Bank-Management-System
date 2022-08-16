using System;
using BankManagementSystem.IO;

namespace BankManagementSystem
{
	class Program
	{
		static void Main()
		{
			// Bank Management System begins execution here...
			new BMS().Execute();

			// Bank Management System terminates here...
			OutputHelpers.Print("\nPress Any Key to Terminate...", ConsoleColor.Cyan);
			Console.ReadKey();
		}
	}
}
