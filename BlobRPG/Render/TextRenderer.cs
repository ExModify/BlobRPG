using BlobRPG.Font;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using BlobRPG.Shaders;
using BlobRPG.Models;
using BlobRPG.Entities;

namespace BlobRPG.Render
{
	public class TextRenderer
	{
		private readonly TextShader Shader;

		public TextRenderer(TextShader shader)
		{
			Shader = shader;
		}

		public void Clean()
		{
			Shader.CleanUp();
		}
		public void Render(Dictionary<FontType, List<GUIText>> texts)
		{
			Prepare();
			foreach (FontType font in texts.Keys)
			{
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D, font.TextureAtlas);
				foreach (GUIText text in texts[font])
				{
					RenderText(text);
				}
			}
			EndRendering();
		}


		private void Prepare()
		{
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.Disable(EnableCap.DepthTest);
			Shader.Start();
		}

		private void RenderText(GUIText text)
		{
			GL.BindVertexArray(text.TextMeshVao);
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);

			Shader.LoadText(text);
			GL.DrawArrays(PrimitiveType.Triangles, 0, text.VertexCount);


			GL.DisableVertexAttribArray(0);
			GL.DisableVertexAttribArray(1);
			GL.BindVertexArray(0);
		}

		private void EndRendering()
		{
			Shader.Stop();
			GL.Disable(EnableCap.Blend);
			GL.Enable(EnableCap.DepthTest);
		}
	}
}
