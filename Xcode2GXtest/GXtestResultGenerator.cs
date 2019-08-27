using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Xcode2GXtest
{
    public class GXtestResultGenerator
    {
		public static string Generate(IList<TestCase> cases)
		{
			using (var sw = new StringWriter())
			{
				var xmlWriterSettings = new XmlWriterSettings() { Indent = true };
				using (XmlWriter writer = XmlWriter.Create(sw, xmlWriterSettings))
				{
					writer.WriteStartDocument();
					writer.WriteStartElement("GXtest.TestCaseCollection");

					XmlSerializer serializer = new XmlSerializer(typeof(TestCase));
					foreach (TestCase t in cases)
					{
						serializer.Serialize(writer, t);
					}

					writer.WriteEndElement();

				}
				return sw.ToString();
			}
		}
    }
}
