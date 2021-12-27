using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Xml
{
    public class XmlNode
	{
		public string Name { get; private set; }
		public Dictionary<string, string> Attributes { get; private set; }
		public string Data { get; set; }
		public Dictionary<string, List<XmlNode>> ChildNodes { get; private set; }

		public XmlNode(string name)
		{
			Name = name;
		}
		public string GetAttribute(string attr)
		{
			if (Attributes != null && Attributes.ContainsKey(attr))
			{
				return Attributes[attr];
			}
			else
			{
				return null;
			}
		}

		public XmlNode GetChild(string childName)
		{
			if (ChildNodes != null && ChildNodes.ContainsKey(childName))
			{
				List<XmlNode> nodes = ChildNodes[childName];
				if (nodes != null && nodes.Count != 0)
				{
					return nodes[0];
				}
			}
			return null;

		}
		public XmlNode GetChildWithAttribute(string childName, string attr, string value)
		{
			List<XmlNode> children = GetChildren(childName);
			if (children == null || children.Count == 0)
			{
				return null;
			}
			foreach (XmlNode child in children)
			{
				string val = child.GetAttribute(attr);
				if (value == val)
				{
					return child;
				}
			}
			return null;
		}

		public List<XmlNode> GetChildren(string name)
		{
			if (ChildNodes != null && ChildNodes.ContainsKey(name))
			{
				return ChildNodes[name];
			}
			return new List<XmlNode>();
		}
		public void AddAttribute(string attr, string value)
		{
			if (Attributes == null)
			{
				Attributes = new Dictionary<string, string>();
			}
			Attributes.Add(attr, value);
		}
		public void AddChild(XmlNode child)
		{
			if (ChildNodes == null)
			{
				ChildNodes = new Dictionary<string, List<XmlNode>>();
			}
			List<XmlNode> list;
			if (!ChildNodes.ContainsKey(child.Name))
			{
				list = new List<XmlNode>();
				ChildNodes.Add(child.Name, list);
			}
			else
			{
				list = ChildNodes[child.Name];
			}
			list.Add(child);
		}
	}
}
