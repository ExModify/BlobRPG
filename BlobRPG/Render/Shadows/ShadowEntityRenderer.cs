using BlobRPG.Entities;
using BlobRPG.Models;
using BlobRPG.Shaders;
using BlobRPG.Tools;
using GlmSharp;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render.Shadows
{
    class ShadowEntityRenderer
    {
		private ShadowShader Shader;

		public ShadowEntityRenderer(ShadowShader shader)
		{
			Shader = shader;
		}

		public void Render(Dictionary<TexturedModel, List<Entity>> entities, ref mat4 projectionViewMatrix)
		{
			foreach (TexturedModel model in entities.Keys)
			{
				RawModel rawModel = model.Model;
				BindModel(rawModel);

				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D, model.Texture.Id);

				if (model.Texture.HasTransparency)
                {
					Renderer.DisableAlphaBlend();
                }

				foreach (Entity entity in entities[model])
				{
					PrepareInstance(entity, ref projectionViewMatrix);
					GL.DrawElements(PrimitiveType.Triangles, rawModel.VertexCount, DrawElementsType.UnsignedInt, 0);
				}

				if (model.Texture.HasTransparency)
				{
					Renderer.EnableAlphaBlend();
				}
			}
			GL.DisableVertexAttribArray(0);
			GL.DisableVertexAttribArray(1);
			GL.BindVertexArray(0);
		}

		private void BindModel(RawModel rawModel)
		{
			GL.BindVertexArray(rawModel.VaoId);
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
		}

		private void PrepareInstance(Entity entity, ref mat4 projectionViewMatrix)
		{
			mat4 modelMatrix = MatrixMaths.CreateTransformationMatrix(entity.Position, entity.RotationX, entity.RotationY, entity.RotationZ, entity.Scale);
			Shader.LoadModelViewProjectionMatrix(projectionViewMatrix * modelMatrix);
		}

	}
}
