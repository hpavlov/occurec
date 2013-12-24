using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using OccuRec.Tracking;

namespace OccuRec.Helpers
{
    public static class Extensions
    {
		public static string GetFullStackTrace(this Exception ex)
		{
			var output = new StringBuilder();

			AddExceptionInfo(ex, ref output);

			return output.ToString();			
		}

		public static string GetFullStackTrace(this Exception ex, string message)
		{
			var output = new StringBuilder(message);
			output.Append("\r\n");

			AddExceptionInfo(ex, ref output);

			return output.ToString();
		}

		private static void AddExceptionInfo(Exception ex, ref StringBuilder output)
		{
			if (ex != null && output != null)
			{
				output.Append(ex.GetType().ToString());
				output.Append(" : ");
				output.Append(ex.Message);
				output.Append("\r\n");
				output.Append(ex.StackTrace);
				output.Append("\r\n");
				output.Append("--------------------------------------------------------------------------------------------------\r\n");

				if (ex.InnerException != null)
					AddExceptionInfo(ex.InnerException, ref output);
			}
		}

		public static void SetNUDValue(this NumericUpDown nud, double value)
		{
			if (!double.IsNaN(value))
				SetNUDValue(nud, (decimal)value);
		}

		public static void SetNUDValue(this NumericUpDown nud, int value)
		{
			SetNUDValue(nud, (decimal)value);
		}

		public static void SetNUDValue(this NumericUpDown nud, decimal value)
		{
			if (value < nud.Minimum)
				nud.Value = nud.Minimum;
			else if (value > nud.Maximum)
				nud.Value = nud.Maximum;
			else
				nud.Value = value;
		}

		public static void SetCBXIndex(this ComboBox cbx, int index)
		{
			if (cbx.Items.Count > 0)
				cbx.SelectedIndex = Math.Max(0, Math.Min(cbx.Items.Count - 1, index));
			else
				cbx.SelectedIndex = -1;
		}

		public static XmlNode Serialize(object instance)
		{
			var xmlSerializer = new XmlSerializer(instance.GetType());
			using (var stream = new MemoryStream())
			{
				xmlSerializer.Serialize(stream, instance);

				stream.Position = 0;	// reset position so we can start reading out of it

				var doc = new XmlDocument();
				doc.Load(stream);
				return doc.LastChild;
			}
		}

		public static void Serialize<T>(this object instance, Stream stream)
		{
			var xmlSerializer = new XmlSerializer(typeof(T));
			xmlSerializer.Serialize(stream, instance);
		}

		public static XmlNode Serialize<T>(this T instance)
		{
			string xmlString = instance.AsXmlString<T>();

			var doc = new XmlDocument();
			doc.LoadXml(xmlString);
			return doc.LastChild;
		}

		public static T Deserialize<T>(this string serializedXmlString)
		{
			using (var reader = new StringReader(serializedXmlString))
			{
				var serializer = new XmlSerializer(typeof(T));
				return (T)serializer.Deserialize(reader);
			}
		}

		public static T Deserialize<T>(this XmlNode rootNode)
		{
			using (var reader = new XmlNodeReader(rootNode))
			{
				var serializer = new XmlSerializer(typeof(T));
				return (T)serializer.Deserialize(reader);
			}
		}

		public static T Deserialize<T>(this Stream stream)
		{
			if (typeof(T) == typeof(string))
			{
				using (var reader = new StreamReader(stream))
				{
					return (T)((object)reader.ReadToEnd());
				}
			}
			var serializer = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(stream);
		}

		public static XmlNode AsSerialized(this object instance)
		{
			return Serialize(instance);
		}
		public static byte[] AsSerializedStream(this object instance)
		{
			if (instance is XmlNode)
			{
				using (var stream = new MemoryStream())
				{
					using (var writer = XmlWriter.Create(stream))
					{
						((XmlNode)instance).WriteTo(writer);

						stream.Position = 0; // reset position so we can start reading out of it

						return stream.ToArray();
					}
				}
			}

			var xmlSerializer = new XmlSerializer(instance.GetType());
			using (var stream = new MemoryStream())
			{
				xmlSerializer.Serialize(stream, instance);

				stream.Position = 0;	// reset position so we can start reading out of it

				return stream.ToArray();
			}
		}
		public static T AsObject<T>(this XmlNode xmlNode)
		{
			return Deserialize<T>(xmlNode);
		}

		public static string AsXmlString<T>(this T instance)
		{
			var stream = new StringWriter();
			var xml = new XmlSerializer(typeof(T));
			xml.Serialize(stream, instance);
			stream.Close();
			return stream.ToString();
		}

    }
}
