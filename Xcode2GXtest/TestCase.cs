using System;
using System.Xml;
using System.Xml.Serialization;

namespace Xcode2GXtest
{
    public class TestCase
    {
		public string Name { get; set; }

		public bool Status { get; set; }

		[XmlElement(ElementName = "StartedAt")]
		public DateTime Started { get; set; }

		[XmlElement(ElementName = "ElapsedTime")]
		public double Duration { get; set; }

		[XmlElement(ElementName = "ErrorMsg")]
		public string Message { get; set; }

		[XmlIgnore]
		public string StatusString
		{
			get {
				return Status ? "passed" : "failed";
			}
		}

		public TestCase()
		{
			Name = "";
			Status = false;
			Started = DateTime.Now;
			Duration = 0;
			Message = "";
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
