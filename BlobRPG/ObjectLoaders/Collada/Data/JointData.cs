using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Data
{
    public class JointData
	{
		public int Index { get; private set; }
		public string NameId { get; private set; }
		public mat4 BindLocalTransform { get; private set; }

		public List<JointData> Children { get; private set; }

		public JointData(int index, string nameId, mat4 bindLocalTransform)
		{
			Index = index;
			NameId = nameId;
			BindLocalTransform = bindLocalTransform;
			Children = new List<JointData>();
		}

		public void AddChild(JointData child)
		{
			Children.Add(child);
		}
	}
}
