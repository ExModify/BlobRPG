using BlobRPG.Entities;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.Shaders;
using GlmSharp;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BlobRPG.Render
{
    public class SkyboxRenderer
    {
		private const float Size = 500;

		private readonly float[] Vertices = {
			-Size,  Size, -Size,
			-Size, -Size, -Size,
			Size, -Size, -Size,
			 Size, -Size, -Size,
			 Size,  Size, -Size,
			-Size,  Size, -Size,

			-Size, -Size,  Size,
			-Size, -Size, -Size,
			-Size,  Size, -Size,
			-Size,  Size, -Size,
			-Size,  Size,  Size,
			-Size, -Size,  Size,

			 Size, -Size, -Size,
			 Size, -Size,  Size,
			 Size,  Size,  Size,
			 Size,  Size,  Size,
			 Size,  Size, -Size,
			 Size, -Size, -Size,

			-Size, -Size,  Size,
			-Size,  Size,  Size,
			 Size,  Size,  Size,
			 Size,  Size,  Size,
			 Size, -Size,  Size,
			-Size, -Size,  Size,

			-Size,  Size, -Size,
			 Size,  Size, -Size,
			 Size,  Size,  Size,
			 Size,  Size,  Size,
			-Size,  Size,  Size,
			-Size,  Size, -Size,

			-Size, -Size, -Size,
			-Size, -Size,  Size,
			 Size, -Size, -Size,
			 Size, -Size, -Size,
			-Size, -Size,  Size,
			 Size, -Size,  Size
		};

		public RawModel Model { get; private set; }

		public int DayTextureId { get; private set; }
		public int NightTextureId { get; private set; }

		public float RotationSpeed { get; private set; }
		public float BlendFactor { get; private set; }

		public vec3 DayLight { get; private set; }
		public vec3 NightLight { get; private set; }

		public SkyboxShader Shader { get; private set; }

		private float Time;

		private int Texture1;
		private int Texture2;

		public SkyboxRenderer(SkyboxShader shader, ref mat4 projectionMatrix)
        {
			Shader = shader;

			Model = Loader.LoadToVao(Vertices, 3);

			DayLight = new vec3(0.7f);
			NightLight = new vec3(0f);

			shader.Start();
			shader.ConnectTextureUnits();
			shader.LoadProjectionMatrix(ref projectionMatrix);
			shader.Stop();
        }

		public void LoadTextures(Stream[] day, Stream[] night)
        {
			LoadDayTexture(day);
			LoadNightTexture(night);
        }
		public void LoadDayTexture(Stream[] streams)
        {
			DayTextureId = Loader.LoadCubeMap(streams);
		}
		public void LoadNightTexture(Stream[] streams)
		{
			NightTextureId = Loader.LoadCubeMap(streams);
		}

		public void Render(Camera camera, Fog fog, vec4 clipPlane)
		{
			Shader.Start();
			Shader.LoadViewMatrix(camera);
			Shader.LoadFog(fog);
			Shader.LoadClipPlane(clipPlane);
			GL.BindVertexArray(Model.VaoId);
			GL.EnableVertexAttribArray(0);
			BindTextures();
			GL.DrawArrays(PrimitiveType.Triangles, 0, Model.VertexCount);
			GL.DisableVertexAttribArray(0);
			GL.BindVertexArray(0);
			Shader.Stop();
		}
		public void Update()
		{
			Time += (float)Settings.DeltaTime * 1000;
			Time %= 24000;

			//float reflectivity;

			if (Time >= 0 && Time < 5000)
			{
				Texture1 = NightTextureId;
				Texture2 = NightTextureId;
				BlendFactor = (Time - 0) / (5000 - 0);

				//Settings.SkyColor = new vec3(0, 0, 0);
				Settings.SkyColor = NightLight;
				//reflectivity = NightReflectivity;
			}
			else if (Time >= 5000 && Time < 8000)
			{
				Texture1 = NightTextureId;
				Texture2 = DayTextureId;
				BlendFactor = (Time - 5000) / (8000 - 5000);

				//Settings.SkyColor = new Color4(DayColor.R * BlendFactor, DayColor.G * BlendFactor, DayColor.B * BlendFactor, 1.0f);

				Settings.SkyColor = new vec3(DayLight - (DayLight - NightLight) * (1 - BlendFactor));
				//reflectivity = DayReflectivity - (DayReflectivity - NightReflectivity) * (1 - BlendFactor);
			}
			else if (Time >= 8000 && Time < 21000)
			{
				Texture1 = DayTextureId;
				Texture2 = DayTextureId;
				BlendFactor = (Time - 8000) / (21000 - 8000);

				Settings.SkyColor = DayLight;
				//reflectivity = DayReflectivity;
			}
			else
			{
				Texture1 = DayTextureId;
				Texture2 = NightTextureId;
				BlendFactor = (Time - 21000) / (24000 - 21000);

				//Settings.SkyColor = new vec3(DayLight.x * (1 - BlendFactor), DayLight.y * (1 - BlendFactor), DayLight.z * (1 - BlendFactor));

				Settings.SkyColor = new vec3(DayLight - (DayLight - NightLight) * (BlendFactor));
				//reflectivity = DayReflectivity - (DayReflectivity - NightReflectivity) * BlendFactor;
			}

			//Program.Game.WaterRenderer.Reflectivity = reflectivity;
		}

		private void BindTextures()
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.TextureCubeMap, Texture1);
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.TextureCubeMap, Texture2);
			Shader.LoadBlendFactor(BlendFactor);
		}
	}
}
