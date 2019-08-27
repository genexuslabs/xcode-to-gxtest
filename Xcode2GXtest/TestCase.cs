using System;
using System.Collections.Generic;
using System.Text;

namespace Xcode2GXtest
{
    public class TestCase
    {
		public string Name { get; private set; }

		public bool Status { get; private set; }

		public DateTime Started { get; private set; }

		public double Duration { get; private set; }

		public string Message { get; private set; }

		public string StatusString
		{
			get {
				return Status ? "passed" : "failed";
			}
		}

		public TestCase(string name, bool status, DateTime startedAt, double duration, string message)
		{
			Name = name;
			Status = status;
			Started = startedAt;
			Duration = duration;
			Message = message;
		}
    }
}
