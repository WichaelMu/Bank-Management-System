using System;
using static BankManagementSystem.Helpers.OutputHelpers;

namespace BankManagementSystem.IO
{
	class Input
	{
		/// <summary>Reads a line from the <see cref="Console"/>.</summary>
		/// <returns>The line that was read.</returns>
		public static string String() { return Console.ReadLine(); }

		/// <summary>Displays a message before reading a line from the <see cref="Console"/>.</summary>
		/// <param name="message">The message to display before reading a line.</param>
		/// <returns>The line that was read.</returns>
		public static string String(string message)
		{
			Print(message);

			return String();
		}

		/// <summary>Reads an <see cref="int"/> from the <see cref="Console"/>.</summary>
		/// <remarks><see cref="FormatException"/> if the input is malinformed.</remarks>
		/// <returns>The <see cref="int"/> that was read.</returns>
		public static int Int() { return Convert.ToInt32(String()); }

		/// <summary>Displays a message before reading an <see cref="int"/> from the <see cref="Console"/>.</summary>
		/// <remarks><see cref="FormatException"/> if the input is malinformed.</remarks>
		/// <param name="message">The message to display before reading a line.</param>
		/// <returns>The <see cref="int"/> that was read.</returns>
		public static int Int(string message)
		{
			Print(message);

			return Int();
		}

		/// <summary>Reads a <see cref="float"/> from the <see cref="Console"/>.</summary>
		/// <remarks><see cref="FormatException"/> if the input is malinformed.</remarks>
		/// <returns>The <see cref="float"/> that was read.</returns>
		public static float Float() { return float.Parse(String()); }

		/// <summary>Displays a message before reading a <see cref="float"/> from the <see cref="Console"/>.</summary>
		/// <remarks><see cref="FormatException"/> if the input is malinformed.</remarks>
		/// <param name="message">The message to display before reading a line.</param>
		/// <returns>The <see cref="float"/> that was read.</returns>
		public static float Float(string message)
		{
			Print(message);

			return Float();
		}

		/// <summary>Reads a <see cref="double"/> from the <see cref="Console"/>.</summary>
		/// <remarks><see cref="FormatException"/> if the input is malinformed.</remarks>
		/// <returns>The <see cref="double"/> that was read.</returns>
		public static double Double() { return double.Parse(String()); }

		/// <summary>Displays a message before reading a <see cref="double"/> from the <see cref="Console"/>.</summary>
		/// <remarks><see cref="FormatException"/> if the input is malinformed.</remarks>
		/// <param name="message">The message to display before reading a line.</param>
		/// <returns>The <see cref="double"/> that was read.</returns>
		public static double Double(string message)
		{
			Print(message);

			return Double();
		}

		/// <summary>Reads any key as a stub input.</summary>
		public static void Any() => Console.ReadKey();

		public static ConsoleKey Key() => Console.ReadKey().Key;

		public static char Char(out char C) => C = Console.ReadKey().KeyChar;
	}
}
