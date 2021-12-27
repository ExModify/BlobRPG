using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Data
{
    public class SkinningData
	{
		public List<string> JointOrder;
		public List<VertexSkinData> VerticesSkinData;

		public SkinningData(List<string> jointOrder, List<VertexSkinData> verticesSkinData)
		{
			JointOrder = jointOrder;
			VerticesSkinData = verticesSkinData;
		}
	}
}
