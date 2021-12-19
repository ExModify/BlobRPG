using BlobRPG.Font;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using GlmSharp;
using BlobRPG.MainComponents;
using BlobRPG.Render;

namespace BlobRPG.Entities
{
    public class GUIText
    {
		public string TextString { get; set; }
		public float FontSize { get; set; }

		public int TextMeshVao { get; private set; }
		public int VertexCount { get; private set; }
		public vec3 Color { get; set; }

		public vec2 Position { get; set; }
		public float MaxLineLength { get; set; }
		public int NumberOfLines { get; set; }

		public FontType Font { get; }

		public bool Centered { get; set; }

		public float Width { get; set; } = 0.5f;
		public float Edge { get; set; } = 0.1f;

		public float BorderWidth { get; set; } = 0.0f;
		/// <summary>
		/// Never put this to 0
		/// </summary>
		public float BorderEdge { get; set; } = 0.1f;
		public vec3 BorderColor { get; set; } = vec3.Zero;

		public vec2 DropShadow { get; set; } = vec2.Zero;

		public GUIText(string text, float fontSize, FontType font, vec2 position, float maxLineLength, bool centered)
		{
			TextString = text;
			FontSize = fontSize;
			Font = font;
			Position = position;
			MaxLineLength = maxLineLength;
			Centered = centered;
			Color = new vec3(0f, 0f, 0f);

			UpdateText(text);
		}

		public void Remove()
		{
			Loader.RemoveText(this);
		}
		public void SetMeshInfo(int VAO, int VerticesCount)
		{
			TextMeshVao = VAO;
			VertexCount = VerticesCount;
		}

		public void UpdateText(string Text)
		{
			GL.DeleteVertexArray(TextMeshVao);
			TextString = Text;
			TextMeshData data = Font.LoadText(this);
			int vao = Loader.LoadToVao(data.VertexPositions, data.TextureCoords);
			SetMeshInfo(vao, data.VertexPositions.Length / 2);
		}
	}
}
