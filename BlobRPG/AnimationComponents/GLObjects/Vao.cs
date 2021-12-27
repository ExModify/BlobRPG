using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.AnimationComponents.GLObjects
{
    public class Vao
    {
        public int Id { get; private set; }
        public int IndexCount { get; private set; }
        public Vbo IndexVbo { get; private set; }
        public List<Vbo> DataVBOs { get; private set; }

        public static Vao Create()
        {
            return new Vao(GL.GenVertexArray());
        }
        public Vao(int id)
        {
            Id = id;
            DataVBOs = new List<Vbo>();
        }
        public void Bind(params int[] attributes)
        {
			Bind();
			for (int i = 0; i < attributes.Length; i++)
			{
				GL.EnableVertexAttribArray(attributes[i]);
			}

		}
		public void Unbind(params int[] attributes)
		{
			for (int i = 0; i < attributes.Length; i++)
			{
				GL.DisableVertexAttribArray(attributes[i]);
			}
			Unbind();
		}

		public void createIndexBuffer(int[] indices)
		{
			IndexVbo = Vbo.Create(BufferTarget.ElementArrayBuffer);
			IndexVbo.Bind();
			IndexVbo.Store(indices);
			IndexCount = indices.Length;
		}

		public void CreateAttribute(int attribute, float[] data, int attrSize)
		{
			Vbo dataVbo = Vbo.Create(BufferTarget.ArrayBuffer);
			dataVbo.Bind();
			dataVbo.Store(data);
			GL.VertexAttribPointer(attribute, attrSize, VertexAttribPointerType.Float, false, attrSize * 4, 0);
			dataVbo.Unbind();
			DataVBOs.Add(dataVbo);
		}
		public void CreateAttribute(int attribute, int[] data, int attrSize)
		{
			Vbo dataVbo = Vbo.Create(BufferTarget.ArrayBuffer);
			dataVbo.Bind();
			dataVbo.Store(data);
			GL.VertexAttribPointer(attribute, attrSize, VertexAttribPointerType.Int, false, attrSize * 4, 0);
			dataVbo.Unbind();
			DataVBOs.Add(dataVbo);
		}

		public void Delete()
		{
			GL.DeleteVertexArray(Id);
			for(int i = 0; i < DataVBOs.Count; i++)
            {
				DataVBOs[i].Delete();
            }
			IndexVbo.Delete();
		}

		private void Bind()
		{
			GL.BindVertexArray(Id);
		}

		private void Unbind()
		{
			GL.BindVertexArray(0);
		}
	}
}
