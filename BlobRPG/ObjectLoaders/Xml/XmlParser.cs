using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Xml
{
    class XmlParser
    {
		private const string DATA = ">(.+?)<";
		private const string START_TAG = "<(.+?)>";
		private const string ATTR_NAME = "(.+?)=";
		private const string ATTR_VAL = "\"(.+?)\"";
		private const string CLOSED = "(</|/>)";

		public static XmlNode LoadXML(string s)
		{
			FileStream fs = new(s, FileMode.Open);
			XmlNode node = LoadXML(fs);
			fs.Close();
			return node;
		}
		public static XmlNode LoadXML(Stream s)
        {
			StreamReader reader = new(s);
			reader.ReadLine(); // skipping the opening <?xml tag
			XmlNode node = LoadNode(reader);
			reader.Close();
			return node;
		}

		private static XmlNode LoadNode(StreamReader reader)
		{
			String line = reader.ReadLine().Trim();
			if (line.StartsWith("</"))
			{
				return null;
			}
			string[] startTagParts = GetStartTag(line).Split(" ");
			XmlNode node = new(startTagParts[0].Replace("/", ""));
			AddAttributes(startTagParts, node);
			AddData(line, node);
			if (Regex.IsMatch(line, CLOSED))
            {
				return node;
            }
			XmlNode child;
			while ((child = LoadNode(reader)) != null)
			{
				node.AddChild(child);
			}
			return node;
		}

		private static void AddData(string line, XmlNode node)
		{
			Match match = Regex.Match(line, DATA);
			if (match.Success)
			{
				node.Data = match.Groups[1].Value;
			}
		}

		private static void AddAttributes(string[] titleParts, XmlNode node)
		{
			for (int i = 1; i < titleParts.Length; i++)
			{
				if (titleParts[i].Contains("="))
				{
					AddAttribute(titleParts[i], node);
				}
			}
		}

		private static void AddAttribute(string attributeLine, XmlNode node)
		{
			Match nameMatch = Regex.Match(attributeLine, ATTR_NAME);
			Match valMatch = Regex.Match(attributeLine, ATTR_VAL);
			node.AddAttribute(nameMatch.Groups[1].Value, valMatch.Groups[1].Value);
		}

		private static string GetStartTag(string line)
		{
			Match match = Regex.Match(line, START_TAG);
			return match.Groups[1].Value;
		}
	}
}
