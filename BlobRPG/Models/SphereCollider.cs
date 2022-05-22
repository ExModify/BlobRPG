using BlobRPG.Tools;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Models
{
    public class SphereCollider : ICollider
    {
        private float _Radius;

        public float Radius
        {
            get
            {
                return _Radius;
            }
            set
            {
                _Radius = value;
                RadiusSquared = value * value;
            }
        }
        public float RadiusSquared { get; private set; }
        public StructReferencer<vec3> Center { get; private set; }

        public SphereCollider(StructReferencer<vec3> center, float radius)
        {
            Center = center;
            Radius = radius;
        }
    }
}
