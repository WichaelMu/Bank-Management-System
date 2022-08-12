

namespace BankManagementSystem
{
	public partial class BMS
	{
		// Define the padding size.
		const int kBorderLeftRightPadding = 5;
		// The tab size of content within borders.
		const int kTabSize = 4;
		// The character representing horizontal bar separators.
		const char kHorizontalBar = '=';

		const string kWelcomeMessage = "WELCOME TO SIMPLE BANKING SYSTEM";

		public static int CharacterLimit;
		public static string HorizontalBorder;

		public BMS()
		{
			Construct();
		}

		/// <summary>Defines class and application defaults.</summary>
		static void Construct()
		{
			// The width of the console; the border limits.
			CharacterLimit = kWelcomeMessage.Length + kBorderLeftRightPadding * 2;
			// The string holding an entire horizontal border separator.
			HorizontalBorder = new string(kHorizontalBar, CharacterLimit);
		}

		public void Execute()
		{
			RunLoginSequence();
		}
	}
}
