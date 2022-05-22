using BlobRPG.Tools;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Models
{
    public class AABBCollider : ICollider
    {
        public vec3 MinExtents { get; set; }
        public vec3 MaxExtents { get; set; }
        public StructReferencer<vec3> Center { get; set; }

        public vec3 OffsetMinExtents
        {
            get
            {
                return Center.Value + MinExtents;
            }
        }
        public vec3 OffsetMaxExtents
        {
            get
            {
                return Center.Value + MaxExtents;
            }
        }

        public AABBCollider(vec3 minExtents, vec3 maxExtents, StructReferencer<vec3> center)
        {
            MinExtents = minExtents;
            MaxExtents = maxExtents;
            Center = center;
        }
        public AABBCollider(float minX, float minY, float minZ, float maxX, float maxY, float maxZ, StructReferencer<vec3> center)
        {
            MinExtents = new(minX, minY, minZ);
            MaxExtents = new(maxX, maxY, maxZ);
            Center = center;
        }
    }
}
