using System;
using static BankManagementSystem.IO.OutputHelpers;

namespace BankManagementSystem.IO
{
	public static class Input
	{
		/// <summary>Reads a line from the <see cref="Console"/>.</summary>
		/// <returns>The line that was read.</returns>
		public static string String() { return Console.ReadLine(); }

		/// <summary>Displays a message before reading a line from the <see cref="Console"/>.</summary>
		/// <param name="Message">The message to display before reading a line.</param>
		/// <returns>The line that was read.</returns>
		public static string String(string Message)
		{
			Print(Message);

			return String();
		}

		/// <summary>Reads an <see cref="int"/> from the <see cref="Console"/>.</summary>
		/// <remarks><see cref="FormatException"/> if the input is malinformed.</remarks>
		/// <returns>The <see cref="int"/> that was read.</returns>
		public static int Int() { return Convert.ToInt32(String()); }

		/// <summary>Displays a message before reading an <see cref="int"/> from the <see cref="Console"/>.</summary>
		/// <remarks><see cref="FormatException"/> if the input is malinformed.</remarks>
		/// <param name="Message">The message to display before reading a line.</param>
		/// <returns>The <see cref="int"/> that was read.</returns>
		public static int Int(string Message)
		{
			Print(Message);

			return Int();
		}

		/// <summary>Reads a <see cref="float"/> from the <see cref="Console"/>.</summary>
		/// <remarks><see cref="FormatException"/> if the input is malinformed.</remarks>
		/// <returns>The <see cref="float"/> that was read.</returns>
		public static float Float() { return float.Parse(String()); }

		/// <summary>Displays a message before reading a <see cref="float"/> from the <see cref="Console"/>.</summary>
		/// <remarks><see cref="FormatException"/> if the input is malinformed.</remarks>
		/// <param name="Message">The message to display before reading a line.</param>
		/// <returns>The <see cref="float"/> that was read.</returns>
		public static float Float(string Message)
		{
			Print(Message);

			return Float();
		}

		/// <summary>Reads a <see cref="double"/> from the <see cref="Console"/>.</summary>
		/// <remarks><see cref="FormatException"/> if the input is malinformed.</remarks>
		/// <returns>The <see cref="double"/> that was read.</returns>
		public static double Double() { return double.Parse(String()); }

		/// <summary>Displays a message before reading a <see cref="double"/> from the <see cref="Console"/>.</summary>
		/// <remarks><see cref="FormatException"/> if the input is malinformed.</remarks>
		/// <param name="Message">The message to display before reading a line.</param>
		/// <returns>The <see cref="double"/> that was read.</returns>
		public static double Double(string Message)
		{
			Print(Message);

			return Double();
		}

		/// <summary>Removes any leading and trailing white-space from <paramref name="ToTrim"/>.</summary>
		/// <param name="ToTrim">The string to trim.</param>
		public static void Trim(ref string ToTrim)
		{
			ToTrim = ToTrim.Trim();
		}

		/// <summary>Reads any key as a stub input.</summary>
		public static void Any() => Console.ReadKey();

		/// <summary>Reads the next <see cref="ConsoleKey"/>.</summary>
		public static ConsoleKey Key() => Console.ReadKey(intercept: true).Key;

		/// <summary>Reads the next <see cref="char"/>.</summary>
		public static char Char(out char C) => C = Console.ReadKey(intercept: true).KeyChar;
	}
}
