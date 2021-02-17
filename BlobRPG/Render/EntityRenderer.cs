using BlobRPG.Models;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Render
{
    public class EntityRenderer
    {
        public void Prepare()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(1, 0, 0, 1);
        }

        public void Render(RawModel model)
        {
            GL.BindVertexArray(model.VaoId);
            GL.EnableVertexAttribArray(0);
            GL.DrawElements(PrimitiveType.Triangles, model.VertexCount, DrawElementsType.UnsignedInt, 0);
            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }
    }
}
