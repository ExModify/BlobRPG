using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.AnimationComponents.GLObjects
{
    public class Vbo
    {
        public int Id { get; private set; }
        public BufferTarget Target { get; private set; }

        public Vbo(int id, BufferTarget target)
        {
            Id = id;
            Target = target;
        }

        public static Vbo Create(BufferTarget type)
        {
            int id = GL.GenBuffer();
            return new Vbo(id, type);
        }

        public void Bind()
        {
            GL.BindBuffer(Target, Id);
        }
        public void Unbind()
        {
            GL.BindBuffer(Target, 0);
        }
        public void Store(float[] data)
        {
            GL.BufferData(Target, data.Length, data, BufferUsageHint.StaticDraw);
        }
        public void Store(int[] data)
        {
            GL.BufferData(Target, data.Length, data, BufferUsageHint.StaticDraw);
        }
        public void Delete()
        {
            GL.DeleteBuffer(Id);
        }
    }
}
