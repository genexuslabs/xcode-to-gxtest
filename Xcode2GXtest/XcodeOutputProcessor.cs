using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Xcode2GXtest
{
    public class XcodeOutputProcessor
    {
		public IEnumerable<TestCase> ProcessOutput(StreamReader stream)
		{
			string s = "";
			ReadState state = ReadState.StartTest;

			string testName;
			XcodeTestResult testResult;
			double testElapsed;
			DateTime startTime = new DateTime();
			string message = "";
			string errMessage = "";

			while ((s = stream.ReadLine()) != null)
			{
				if (state == ReadState.StartTest && ReadTestCaseStart(s, out testName))
				{
					state = ReadState.StartTime;
				}
				else if (state == ReadState.StartTime && ReadTestCaseStartTime(s, out startTime))
				{
					state = ReadState.EndTest;
				}
				else if (state == ReadState.EndTest && ReadTestCaseError(s, out message))
				{
					errMessage = message;
				}
				else if (state == ReadState.EndTest && ReadTestCaseResult(s, out testName, out testResult, out testElapsed))
				{
					state = ReadState.StartTest;
					yield return new TestCase(testName, testResult == XcodeTestResult.Passed, startTime, testElapsed, errMessage);
					startTime = new DateTime();
					errMessage = "";
				}
			}
		}

		enum XcodeTestResult
		{
			Passed,
			Failed
		}

		enum ReadState
		{
			StartTest,
			StartTime,
			EndTest
		}

		static Regex testStartedRegEx = new Regex(@"Test Case '-\[[a-zA-Z0-9_]+\.[a-zA-Z0-9_]+ test(?<testname>[a-zA-Z0-9]+)\]' started\.");

		static bool ReadTestCaseStart(string line, out string testName)
		{
			// Test Case '-[WorkWithDevicesRecordUITests.WorkWithDevicesRecordUITests testExample]' started.
			testName = "";
			Match m = testStartedRegEx.Match(line);
			var success = m.Success;
			if (success)
			{
				testName = m.Groups["testname"].Value;
			}
			return success;
		}

		static Regex testStartTimeRegEx = new Regex(@"Start Test at (?<year>[0-9]{4})-(?<month>[0-9]{2})-(?<day>[0-9]{2}) (?<hour>[0-9]{2}):(?<minute>[0-9]{2}):(?<second>[0-9]{2})\.(?<milli>[0-9]{3})");

		static bool ReadTestCaseStartTime(string line, out DateTime start)
		{
			//     t =     0.00s Start Test at 2019-08-22 13:35:08.332
			start = new DateTime();
			Match m = testStartTimeRegEx.Match(line);
			var success = m.Success;
			if (success)
			{
				int year = int.Parse(m.Groups["year"].Value);
				int month = int.Parse(m.Groups["month"].Value);
				int day = int.Parse(m.Groups["day"].Value);
				int hour = int.Parse(m.Groups["hour"].Value);
				int minute = int.Parse(m.Groups["minute"].Value);
				int second = int.Parse(m.Groups["second"].Value);
				int milli = int.Parse(m.Groups["milli"].Value);

				start = new DateTime(year, month, day, hour, minute, second, milli);
			}
			return success;
		}

		static Regex testAssertionFailedRegEx = new Regex(@"Assertion Failure: [a-zA-Z0-9\.: ]*- (?<errMsg>[a-zA-Z0-9' ]*)$");

		static bool ReadTestCaseError(string line, out string errorMessage)
		{
			//     t =    12.86s Assertion Failure: GXUITestingAPI.swift:366: XCTAssertTrue failed - Could not find control with name 'ButtonInsert'
			errorMessage = "";
			Match m = testAssertionFailedRegEx.Match(line);
			var success = m.Success;
			if (success)
			{
				errorMessage = m.Groups["errMsg"].Value;
			}
			return success;
		}

		static Regex testEndedRegEx = new Regex(@"Test Case '-\[[a-zA-Z0-9_]+\.[a-zA-Z0-9_]+ test(?<testname>[a-zA-Z0-9]+)\]' (?<result>(passed|failed)) \((?<elapsed>[0-9\.]+) seconds\)\.");

		static bool ReadTestCaseResult(string line, out string testName, out XcodeTestResult result, out double elapsed)
		{
			// Test Case '-[WorkWithDevicesRecordUITests.WorkWithDevicesRecordUITests testExample]' passed (7.917 seconds).
			// Test Case '-[WorkWithDevicesRecordUITests.WorkWithDevicesRecordUITests testTestWorkWithDevicesRecordInsertButton]' failed (12.872 seconds).
			testName = "";
			result = XcodeTestResult.Failed;
			elapsed = 0;
			Match m = testEndedRegEx.Match(line);
			var success = m.Success;
			if (success)
			{
				testName = m.Groups["testname"].Value;
				result = m.Groups["result"].Value == "passed" ? XcodeTestResult.Passed : XcodeTestResult.Failed;
				Double.TryParse(m.Groups["elapsed"].Value, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out elapsed);
			}
			return success;
		}
	}
}
