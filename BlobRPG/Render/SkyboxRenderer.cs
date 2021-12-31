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

		public SkyboxShader Shader { get; private set; }

		private int Texture1;
		private int Texture2;

		public SkyboxRenderer(SkyboxShader shader, ref mat4 projectionMatrix)
        {
			Shader = shader;

			Model = Loader.LoadToVao(Vertices, 3);

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
			if (Settings.IngameTime >= Settings.DayPhaseStart && Settings.IngameTime < Settings.DayPhaseEnd)
			{
				Texture1 = NightTextureId;
				Texture2 = DayTextureId;

				BlendFactor = (Settings.IngameTime - Settings.DayPhaseStart) / Settings.PhaseTime;
			}
			else if (Settings.IngameTime >= Settings.DayPhaseEnd && Settings.IngameTime < Settings.NightPhaseStart)
			{
				Texture1 = DayTextureId;
				Texture2 = DayTextureId;
				BlendFactor = 1;
			}
			else if (Settings.IngameTime >= Settings.NightPhaseStart && Settings.IngameTime < Settings.NightPhaseEnd)
			{
				Texture1 = DayTextureId;
				Texture2 = NightTextureId;

				BlendFactor = (Settings.IngameTime - Settings.NightPhaseStart) / Settings.PhaseTime;
			}
			else
			{
				Texture1 = NightTextureId;
				Texture2 = NightTextureId;
				BlendFactor = 1;
			}
			

			//float reflectivity;
			
			if (Settings.IngameTime >= 0 && Settings.IngameTime < 5000)
			{
				Texture1 = NightTextureId;
				Texture2 = NightTextureId;
				BlendFactor = (Settings.IngameTime - 0) / (5000 - 0);

				//Settings.SkyColor = NightLight;
			}
			else if (Settings.IngameTime >= 5000 && Settings.IngameTime < 8000)
			{
				Texture1 = NightTextureId;
				Texture2 = DayTextureId;
				BlendFactor = (Settings.IngameTime - 5000) / (8000 - 5000);

				//Settings.SkyColor = new vec3(DayLight - (DayLight - NightLight) * (1 - BlendFactor));
			}
			else if (Settings.IngameTime >= 8000 && Settings.IngameTime < 21000)
			{
				Texture1 = DayTextureId;
				Texture2 = DayTextureId;
				BlendFactor = (Settings.IngameTime - 8000) / (21000 - 8000);

				//Settings.SkyColor = DayLight;
			}
			else
			{
				Texture1 = DayTextureId;
				Texture2 = NightTextureId;
				BlendFactor = (Settings.IngameTime - 21000) / (24000 - 21000);

				//Settings.SkyColor = new vec3(DayLight - (DayLight - NightLight) * (BlendFactor));
			}
			
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
