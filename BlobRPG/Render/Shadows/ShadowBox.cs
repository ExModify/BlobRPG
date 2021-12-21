using BlobRPG.Entities;
using BlobRPG.Tools;
using GlmSharp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render.Shadows
{
    public class ShadowBox
	{
		private vec4 Up = new(0, 1, 0, 0);
		private vec4 Forward = new(0, 0, -1, 0);

		private float MinX, MaxX;
		private float MinY, MaxY;
		private float MinZ, MaxZ;
		private readonly Camera Camera;

		private float FarHeight, FarWidth, NearHeight, NearWidth;

		public vec3 GetCenter(ref mat4 lightViewMatrix)
        {
			float x = (MinX + MaxX) / 2f;
			float y = (MinY + MaxY) / 2f;
			float z = (MinZ + MaxZ) / 2f;
			vec4 cen = new(x, y, z, 1);
			mat4 invertedLight = lightViewMatrix.Inverse;
			return new vec3(invertedLight * cen);
		}
		public float Width
        {
            get
            {
				return MaxX - MinX;
			}
		}
		public float Height
		{
			get
			{
				return MaxY - MinY;
			}
		}
		public float Length
		{
			get
			{
				return MaxZ - MinZ;
			}
		}

		public ShadowBox(Camera camera)
		{
			Camera = camera;
			CalculateWidthsAndHeights();
		}

		public void Update(ref mat4 lightViewMatrix)
		{
			mat4 rotation = CalculateCameraRotationMatrix();
			vec3 forwardVector = new vec3(rotation * Forward);

			vec3 toFar = new vec3(forwardVector);
			toFar *= Settings.ShadowDistance;
			vec3 toNear = new vec3(forwardVector);
			toNear *= Settings.NEAR;
			vec3 centerNear = toNear + Camera.Position;
			vec3 centerFar = toFar + Camera.Position;

			vec4[] points = CalculateFrustumVertices(rotation, forwardVector, centerNear, centerFar, ref lightViewMatrix);

            for (int i = 0; i < points.Length; i++)
			{
				if (i == 0)
				{
					MinX = points[i].x;
					MaxX = points[i].x;
					MinY = points[i].y;
					MaxY = points[i].y;
					MinZ = points[i].z;
					MaxZ = points[i].z;
					continue;
				}
				if (points[i].x > MaxX)
				{
					MaxX = points[i].x;
				}
				else if (points[i].x < MinX)
				{
					MinX = points[i].x;
				}
				if (points[i].y > MaxY)
				{
					MaxY = points[i].y;
				}
				else if (points[i].y < MinY)
				{
					MinY = points[i].y;
				}
				if (points[i].z > MaxZ)
				{
					MaxZ = points[i].z;
				}
				else if (points[i].z < MinZ)
				{
					MinZ = points[i].z;
				}
			}
			MaxZ += Settings.ShadowOffset;
		}

		private vec4[] CalculateFrustumVertices(mat4 rotation, vec3 forwardVector, vec3 centerNear, vec3 centerFar, ref mat4 lightViewMatrix)
		{
			vec3 upVector = new vec3(rotation * Up);
			vec3 rightVector = vec3.Cross(forwardVector, upVector);
			vec3 downVector = new vec3(-upVector.x, -upVector.y, -upVector.z);
			vec3 leftVector = new vec3(-rightVector.x, -rightVector.y, -rightVector.z);

			vec3 farTop = centerFar + new vec3(upVector.x * FarHeight, upVector.y * FarHeight, upVector.z * FarHeight);
			vec3 farBottom = centerFar + new vec3(downVector.x * FarHeight, downVector.y * FarHeight, downVector.z * FarHeight);
			vec3 nearTop = centerNear + new vec3(upVector.x * NearHeight, upVector.y * NearHeight, upVector.z * NearHeight);
			vec3 nearBottom = centerNear + new vec3(downVector.x * NearHeight, downVector.y * NearHeight, downVector.z * NearHeight);

			vec4[] points = new vec4[8];
			points[0] = CalculateLightSpaceFrustumCorner(ref farTop, ref rightVector, FarWidth, ref lightViewMatrix);
			points[1] = CalculateLightSpaceFrustumCorner(ref farTop, ref leftVector, FarWidth, ref lightViewMatrix);
			points[2] = CalculateLightSpaceFrustumCorner(ref farBottom, ref rightVector, FarWidth, ref lightViewMatrix);
			points[3] = CalculateLightSpaceFrustumCorner(ref farBottom, ref leftVector, FarWidth, ref lightViewMatrix);
			points[4] = CalculateLightSpaceFrustumCorner(ref nearTop, ref rightVector, NearWidth, ref lightViewMatrix);
			points[5] = CalculateLightSpaceFrustumCorner(ref nearTop, ref leftVector, NearWidth, ref lightViewMatrix);
			points[6] = CalculateLightSpaceFrustumCorner(ref nearBottom, ref rightVector, NearWidth, ref lightViewMatrix);
			points[7] = CalculateLightSpaceFrustumCorner(ref nearBottom, ref leftVector, NearWidth, ref lightViewMatrix);
			return points;
		}
		private vec4 CalculateLightSpaceFrustumCorner(ref vec3 startPoint, ref vec3 direction, float width, ref mat4 lightViewMatrix)
		{
			vec3 point = startPoint + new vec3(direction.x * width, direction.y * width, direction.z * width);
			vec4 point4f = lightViewMatrix * new vec4(point.x, point.y, point.z, 1f);
			return point4f;
		}

		private mat4 CalculateCameraRotationMatrix()
		{
			return mat4.Identity
					* mat4.RotateY((float)MathHelper.DegreesToRadians(-Camera.Yaw))
					* mat4.RotateX((float)MathHelper.DegreesToRadians(-Camera.Pitch));
		}

		private void CalculateWidthsAndHeights()
		{
			FarWidth = (float)(Settings.ShadowDistance * Math.Tan(MathHelper.DegreesToRadians(Settings.FieldOfView)));
			NearWidth = (float)(Settings.NEAR * Math.Tan(MathHelper.DegreesToRadians(Settings.FieldOfView)));
			FarHeight = FarWidth / Settings.AspectRatio;
			NearHeight = NearWidth / Settings.AspectRatio;
		}
	}
}
