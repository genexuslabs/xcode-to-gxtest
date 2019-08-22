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
			string s = "";
			while ((s = stream.ReadLine()) != null)
			{
				string testName;
				XcodeTestResult testResult;
				double testElapsed;
				if (ReadTestCaseResult(s, out testName, out testResult, out testElapsed)) {
					string testResultStr = testResult == XcodeTestResult.Passed ? "passed" : "failed";
					result.AppendLine($"Test '{testName}' {testResultStr} in {testElapsed} seconds.");
				}
			}

			return result.ToString();
		}

		enum XcodeTestResult
		{
			Passed,
			Failed
		}

		static bool ReadTestCaseResult(string line, out string testName, out XcodeTestResult result, out double elapsed)
		{
			// Test Case '-[WorkWithDevicesRecordUITests.WorkWithDevicesRecordUITests testExample]' passed (7.917 seconds).
			// Test Case '-[WorkWithDevicesRecordUITests.WorkWithDevicesRecordUITests testTestWorkWithDevicesRecordInsertButton]' failed (12.872 seconds).
			string regExPattern = @"Test Case '-\[[a-zA-Z0-9_]+\.[a-zA-Z0-9_]+ test(?<testname>[a-zA-Z0-9]+)\]' (?<result>(passed|failed)) \((?<elapsed>[0-9\.]+) seconds\)\.";
			Regex regEx = new Regex(regExPattern);
			Match m = regEx.Match(line);
			if (m.Success)
			{
				testName = m.Groups["testname"].ToString();
				result = m.Groups["result"].ToString() == "passed" ? XcodeTestResult.Passed : XcodeTestResult.Failed;
				Double.TryParse(m.Groups["elapsed"].ToString(), out elapsed);
				Double.TryParse(m.Groups["elapsed"].ToString(), NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out elapsed);
				return true;
			}
			else
			{
				testName = null;
				result = XcodeTestResult.Failed;
				elapsed = 0;
				return false;
			}
		}
    }
}
