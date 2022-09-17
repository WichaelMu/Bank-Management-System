using System;
using BankManagementSystem.IO;

namespace BankManagementSystem
{
	class MainProgram
	{
		static void Main()
		{
			// Bank Management System begins execution here...
			new BMS().Execute();

			// Bank Management System terminates here...
			Terminate();
		}

		public static void Terminate()
		{
			Output.Print("\nPress Any Key to Terminate...", ConsoleColor.Cyan);
			Input.Any();

			// Terminates this Process with exit code: 0.
			Environment.Exit(0);
		}
	}
}
