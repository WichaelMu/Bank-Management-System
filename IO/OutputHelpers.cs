using System;
using System.Text;

namespace BankManagementSystem.IO
{
	/// <summary>Utility class that helps with printing to the console.</summary>
	public static class OutputHelpers
	{
		/// <summary>The character representing the vertical borders.</summary>
		const char kBorder = '|';
		const string kBackspace = "\b \b";
		public const string kPrompt = "ENTER THE DETAILS";

		/// <summary>
		/// Prints <paramref name="Content"/> with <paramref name="LeftRightPadding"/> on either side
		/// surrounded by <see cref="kBorder"/>.
		/// </summary>
		/// <param name="Content">The <see langword="string"/> to display.</param>
		/// <param name="LeftRightPadding">
		/// The padding on the left and right of Content in mono-spaced character sizes.
		/// </param>
		/// <param name="Border">The character that represents the vertical borders.</param>
		public static void PrintWithBorder(string Content, int LeftRightPadding = 0, char Border = kBorder)
		{
			if (LeftRightPadding == 0)
			{
				Print($"{Border}{Content}{Border}");
			}
			else
			{
				bool bContentIsOdd = Content.Length % 2 == 1;
				PrintWithCustomPadding(Content, LeftRightPadding, LeftRightPadding - (bContentIsOdd ? 1 : 0), bWithBorder: true, Border);
			}
		}

		/// <summary>
		/// Prints <paramref name="Content"/> with <paramref name="LeftPadding"/> and <paramref name="RightPadding"/>
		/// on its sides surrounded by <see cref="kBorder"/>.
		/// </summary>
		/// <param name="Content">The <see langword="string"/> to display.</param>
		/// <param name="LeftPadding">The padding on the left of Content in mono-spaced character size.</param>
		/// <param name="RightPadding">The padding on the right of Content in mono-spaced character size.</param>
		/// <param name="bWithBorder"><see langword="true"/> to surround Content with a vertical border.</param>
		/// <param name="Border">The character that represents vertical borders.</param>
		public static void PrintWithCustomPadding(string Content, int LeftPadding, int RightPadding, bool bWithBorder = true, char Border = kBorder)
		{
			string Left = new string(' ', LeftPadding);
			string Right = new string(' ', RightPadding);

			// Append Borders and/or Padding with StringBuilder.
			StringBuilder SB = new StringBuilder();
			if (bWithBorder)
			{
				SB
				.Append(Border)
				.Append(Left)
				.Append(Content)
				.Append(Right)
				.Append(Border);
			}
			else
			{
				SB
				.Append(Left)
				.Append(Content)
				.Append(Right);
			}

			/*
			 * let Left and Right Padding = 5
			 * let Border = '|'
			 * Result: '|     Content     |'
			 */

			Print(SB.ToString());
		}

		/// <summary>Prints a new line with borders.</summary>
		/// <param name="CharacterLimit">The width of the console where the borders should lie.</param>
		/// <param name="Border">The character that represents vertical borders.</param>
		public static void PrintNewLineWithBorder(int CharacterLimit, char Border = kBorder)
		{
			PrintWithCustomPadding("", 0, CharacterLimit, bWithBorder: true, Border: Border);
		}

		/// <summary>
		/// Calculates the centre alignment of <paramref name="Content"/>, given the width of the console.
		/// </summary>
		/// <param name="Content">The <see cref="string"/> to centre-align in the console.</param>
		/// <param name="CharacterLimit">The width of the console where the borders should lie.</param>
		/// <param name="Result">Outs the padding size of the left and right sides.</param>
		public static void AutoCentre(string Content, int CharacterLimit, out int Result)
		{
			int HalfLimit = CharacterLimit / 2;
			int HalfContent = Content.Length / 2;
			Result = HalfLimit - HalfContent;
		}

		/// <summary>
		/// Calculates the padding required to align a right-side border with <paramref name="CharacterLimit"/>.
		/// </summary>
		/// <param name="Content">The <see cref="string"/> to base the right-side padding off of.</param>
		/// <param name="CharacterLimit">The width of the console where the borders should lie.</param>
		/// <param name="Result">
		/// Outs the padding right-side padding size that aligns with the vertical borders.
		/// </param>
		public static void PaddingUntilEnd(string Content, int CharacterLimit, out int Result)
		{
			Result = CharacterLimit - Content.Length;
		}

		public static void SetColours(ConsoleColor FColour, ConsoleColor BColour = ConsoleColor.Black)
		{
			Console.ForegroundColor = FColour;
			Console.BackgroundColor = BColour;
		}

		public static void ResetColours()
		{
			SetColours(ConsoleColor.Gray);
		}

		public static void ClearLine()
		{
			Console.Write(new string(' ', BMS.CharacterLimit * 2));
			Console.CursorLeft = 0;
		}

		public static void Print(string Content, ConsoleColor FColour = ConsoleColor.Gray, ConsoleColor BColour = ConsoleColor.Black)
		{
			// Set Colours.
			SetColours(FColour, BColour);

			// Clear line and Write Content.
			ClearLine();
			Console.WriteLine(Content);

			// Revert to defaults.
			ResetColours();
		}

		/// <summary>Prints a title with borders.</summary>
		public static void PrintTitle(string Content)
		{
			AutoCentre(Content, BMS.CharacterLimit, out int Padding);

			PrintWithBorder(BMS.HorizontalBorder);
			PrintWithBorder(Content, Padding);
			PrintWithBorder(BMS.HorizontalBorder);
		}

		/// <summary>Removes the character from the console under the cursor.</summary>
		public static void Backspace() => Console.Write(kBackspace);

		/// <summary>
		/// A string date in the format:
		/// <br></br>
		/// DayOfWeek, Day of Month Year HH:mm:ss
		/// </summary>
		public static string FormatDate()
		{
			DateTime Now = DateTime.Now;
			TimeSpan Time = Now.TimeOfDay;
			StringBuilder TimeBuilder = new StringBuilder();
			TimeBuilder
			.Append(Now.ToString("D"))
			.Append(" ")
			.Append(Time.Hours.ToString("D2"))
			.Append(':')
			.Append(Time.Minutes.ToString("D2"))
			.Append(':')
			.Append(Time.Seconds.ToString("D2"));

			return TimeBuilder.ToString();
		}
	}
}
