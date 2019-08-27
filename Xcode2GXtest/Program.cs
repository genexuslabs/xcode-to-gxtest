using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

			Console.WriteLine();
			Console.Write("Press any key to continue...");
			Console.ReadKey();
        }

		static string TransformXcodeInput(StreamReader stream)
		{
			XcodeOutputProcessor processor = new XcodeOutputProcessor();
			List<TestCase> cases = processor.ProcessOutput(stream).ToList();
			return GXtestResultGenerator.Generate(cases);
		}
    }
}
