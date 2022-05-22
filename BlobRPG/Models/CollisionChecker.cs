using BlobRPG.Tools;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Models
{
    public static class CollisionChecker
    {
        public static bool IsColliding(this ICollider collider, ICollider other)
        {
            if (collider is SphereCollider sphere)
            {
                if (other is SphereCollider sphere2)
                {
                    return SphereSphereCollision(sphere, sphere2);
                }
                else if (other is AABBCollider aabb)
                {
                    return AABBSphereCollision(sphere, aabb);
                }
            }
            else if (collider is AABBCollider aabb)
            {
                if (other is SphereCollider sphere2)
                {
                    return AABBSphereCollision(sphere2, aabb);
                }
                else if (other is AABBCollider aabb2)
                {
                    return AABBAABBCollision(aabb, aabb2);
                }
            }

            return false;
        }

        private static bool AABBSphereCollision(SphereCollider sphere, AABBCollider aabb)
        {
            vec3 closestPoint = ClosestPoint(aabb, sphere.Center);
            vec3 differenceVec = sphere.Center.Value - closestPoint;

            return differenceVec.LengthSqr < sphere.RadiusSquared;
        }
        private static bool SphereSphereCollision(SphereCollider sphere, SphereCollider sphere2)
        {
            vec3 d = sphere.Center.Value - sphere2.Center.Value;
            float radiusSum = sphere.Radius + sphere2.Radius;

            return vec3.Dot(d, d) <= radiusSum * radiusSum;
        }
        private static bool AABBAABBCollision(AABBCollider a, AABBCollider b)
        {
            vec3 amin = a.OffsetMinExtents;
            vec3 amax = a.OffsetMaxExtents;

            vec3 bmin = b.OffsetMinExtents;
            vec3 bmax = b.OffsetMaxExtents;

            return amin.x <= bmax.x && amax.x >= bmin.x &&
                   amin.y <= bmax.y && amax.y >= bmin.y &&
                   amin.z <= bmax.z && amax.z >= bmin.z;
        }
        private static vec3 ClosestPoint(AABBCollider aabb, StructReferencer<vec3> position)
        {
            vec3 point = position.Value;
            vec3 max = aabb.OffsetMaxExtents;
            vec3 min = aabb.OffsetMinExtents;
            float x, y, z;

            if (point.x > max.x)
            {
                x = max.x;
            }
            else if (point.x < min.x)
            {
                x = min.x;
            }
            else
            {
                x = point.x;
            }

            if (point.y > max.y)
            {
                y = max.y;
            }
            else if (point.y < min.y)
            {
                y = min.y;
            }
            else
            {
                y = point.y;
            }

            if (point.z > max.z)
            {
                z = max.z;
            }
            else if (point.z < min.z)
            {
                z = min.z;
            }
            else
            {
                z = point.z;
            }

            return new vec3(x, y, z);
        }
    }
}
