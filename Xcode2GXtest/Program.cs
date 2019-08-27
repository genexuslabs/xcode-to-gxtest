using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Xcode2GXtest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
			{
				Console.WriteLine("Usage: dotnet Xcode2GXtest <file_name>");
				return;
			}

			string fileName = args[0];

			if (!File.Exists(fileName))
			{
				Console.WriteLine($"File not found: {fileName}");
				Console.ReadKey();
				return;
			}

			using (StreamReader stream = File.OpenText(fileName))
			{
				string transformed = TransformXcodeInput(stream);
				Console.Write(transformed);
			}

			Console.Write("Press any key to continue...");
			Console.ReadKey();
        }

		static string TransformXcodeInput(StreamReader stream)
		{
			StringBuilder result = new StringBuilder();

			XcodeOutputProcessor processor = new XcodeOutputProcessor();
			foreach (TestCase t in processor.ProcessOutput(stream))
			{
				string errMsg = t.Status ? "" : $" ({t.Message})";
				result.Append($"Test '{t.Name}' {t.StatusString} in {t.Duration} seconds{errMsg}.\n");
			}

			return result.ToString();
		}
    }
}
