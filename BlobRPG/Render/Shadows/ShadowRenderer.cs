using BlobRPG.Entities;
using BlobRPG.Models;
using BlobRPG.Shaders;
using GlmSharp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render.Shadows
{
    class ShadowRenderer
    {
		private readonly ShadowFrameBuffer ShadowFbo;
		private readonly ShadowShader Shader;
		private readonly ShadowBox ShadowBox;
		private mat4 ProjectionMatrix = mat4.Identity;
		private mat4 LightViewMatrix = mat4.Identity;
		private mat4 ProjectionViewMatrix = mat4.Identity;
		private mat4 Offset = CreateOffset();

		private readonly ShadowEntityRenderer EntityRenderer;

		public mat4 ToShadowMapSpaceMatrix
		{
            get
            {
				return Offset * ProjectionViewMatrix;
			}
		}
		public int ShadowMap
		{
            get
			{
				return ShadowFbo.ShadowMap;
			}
		}
		public mat4 LightSpaceTransform
        {
            get
            {
				return LightViewMatrix;
			}
        }

		public ShadowRenderer(Camera camera)
		{
			Shader = new ShadowShader();
			ShadowBox = new ShadowBox(camera);
			ShadowFbo = new ShadowFrameBuffer(Settings.ShadowMapSize, Settings.ShadowMapSize);
			EntityRenderer = new ShadowEntityRenderer(Shader);
		}

		public void Render(Dictionary<TexturedModel, List<Entity>> entities, Light sun)
		{
			ShadowBox.Update(ref LightViewMatrix);
			vec3 sunPosition = sun.Position;
			vec3 lightDirection = new vec3(-sunPosition.x, -sunPosition.y, -sunPosition.z);
			Prepare(lightDirection, ShadowBox);
			EntityRenderer.Render(entities, ref ProjectionViewMatrix);
			Finish();
		}

		
		public void CleanUp()
		{
			Shader.CleanUp();
			ShadowFbo.CleanUp();
		}
	
		private void Prepare(vec3 lightDirection, ShadowBox box)
		{
			UpdateOrthoProjectionMatrix(box.Width, box.Height, box.Length);
			UpdateLightViewMatrix(lightDirection, box.GetCenter(ref LightViewMatrix));
			ProjectionViewMatrix = ProjectionMatrix * LightViewMatrix;
			ShadowFbo.BindFrameBuffer();
			GL.Enable(EnableCap.DepthTest);
			GL.Clear(ClearBufferMask.DepthBufferBit);
			Shader.Start();
		}

		private void Finish()
		{
			Shader.Stop();
			ShadowFbo.UnbindFrameBuffer();
		}

		private void UpdateLightViewMatrix(vec3 direction, vec3 center)
		{
			direction = direction.Normalized;
			center *= -1;
			LightViewMatrix = mat4.Identity;

			float pitch = (float)Math.Acos(new vec2(direction.x, direction.z).Length);
			float yaw = (float)MathHelper.RadiansToDegrees(Math.Atan(direction.x / direction.z));
			yaw = direction.z > 0 ? yaw - 180 : yaw;

			LightViewMatrix *= mat4.RotateX(pitch);
			LightViewMatrix *= mat4.RotateY(-MathHelper.DegreesToRadians(yaw));
			LightViewMatrix *= mat4.Translate(center);
		}

		private void UpdateOrthoProjectionMatrix(float width, float height, float length)
		{
			ProjectionMatrix = mat4.Identity;
			ProjectionMatrix.m00 = 2f / width;
			ProjectionMatrix.m11 = 2f / height;
			ProjectionMatrix.m22 = -2f / length;
			ProjectionMatrix.m33 = 1;
		}

		private static mat4 CreateOffset()
		{
			mat4 offset = mat4.Identity;
			offset *= mat4.Translate(new vec3(0.5f, 0.5f, 0.5f));
			offset *= mat4.Scale(new vec3(0.5f, 0.5f, 0.5f));
			return offset;
		}
	}
}
