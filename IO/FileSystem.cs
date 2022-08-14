using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementSystem.IO
{
	public static class FileSystem
	{
		public const string kDirectory = "./";
		public const string kFileName = "login.txt";

		/// <summary>Writes string lines to a file.</summary>
		/// <param name="Path">The path of the file to write to.</param>
		/// <param name="NameOfFile">The name of the file to write to, including it's extension.</param>
		/// <param name="Mode"><see cref="EWriteMode"/> append to the file (if it exists), or overwrite the file regardless of it's existing contents.</param>
		/// <param name="Encoding">The type of <see cref="Encoding"/> to write as.</param>
		/// <param name="Lines">The lines to write.</param>
		public static async Task WriteToFile(string Path, string NameOfFile, EWriteMode Mode, Encoding Encoding, params string[] Lines)
		{
			// Construct the file.
			string PathAndName = Path + NameOfFile;

			if (Mode == EWriteMode.Overwrite)
			{
				await File.WriteAllLinesAsync(PathAndName, Lines, Encoding);
			}
			else
			{
				using StreamWriter File = new StreamWriter(PathAndName, true, Encoding);
				foreach (string Line in Lines)
				{
					File.WriteLine(Line);
				}
			}
		}

		/// <summary>Writes string lines to a file.</summary>
		/// <param name="Path">The path of the file to write to.</param>
		/// <param name="NameOfFile">The name of the file to write to, including it's extension.</param>
		/// <param name="Mode"><see cref="EWriteMode"/> append to the file (if it exists), or overwrite the file regardless of it's existing contents.</param>
		/// <param name="Encoding">The type of <see cref="Encoding"/> to write as.</param>
		/// <param name="Lines">The lines to write.</param>
		public static async Task WriteToFileAsync(string Path, string NameOfFile, EWriteMode Mode, Encoding Encoding, string[] Lines)
		{
			await WriteToFile(Path, NameOfFile, Mode, Encoding, Lines);
		}

		/// <summary>Reads contents from a file into a <see cref="List{T}"/> of <see cref="string"/>s.</summary>
		/// <param name="Path">The path of the file to read from.</param>
		/// <param name="NameOfFile">The name of the file to read from, including it's extension.</param>
		/// <param name="ContentsInFile">The out <see cref="List{T}"/> of <see cref="string"/>s of the contents from the file at path.</param>
		/// <returns>True if NameOfFile at Path was read with no errors. False if NameOfFile at Path does not exist, or is unable to be read.</returns>
		public static bool ReadFromFile(string Path, string NameOfFile, out List<string> ContentsInFile)
		{
			ContentsInFile = new List<string>();

			try
			{
				using StreamReader StreamReader = new StreamReader(Path + NameOfFile);

				string Line;

				while ((Line = StreamReader.ReadLine()) != null)
				{
					ContentsInFile.Add(Line);
				}

				StreamReader.Close();
			}
			catch (IOException)
			{
				// In case NameOfFile doesn't exist, just create it.
				FileStream NewFile = File.Create(Path + NameOfFile);
				NewFile.Close(); // Make sure it is closed.

				// Recursively retry reading from this file after creating.
				return ReadFromFile(Path, NameOfFile, out ContentsInFile);
			}
			catch (Exception e)
			{
				Console.WriteLine("File could not be read!\n" + e);
				return false;
			}

			return true;
		}

		public static bool FileExists(string Path, string NameOfFile)
		{
			return File.Exists(Path + NameOfFile);
		}

		public static void DeleteAccount(int ID)
		{
			if (FileExists(kDirectory, ID + ".txt"))
				File.Delete(kDirectory + ID + ".txt");
		}
	}


	/// <summary>The behaviour in which to write to a file.</summary>
	public enum EWriteMode
	{
		/// <summary>Append to the end of a file.</summary>
		Append,
		/// <summary>Make or write to a file, regardless of it's existing contents.</summary>
		Overwrite
	}
}
