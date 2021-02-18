using BlobRPG.MainComponents;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Shaders
{
    public abstract class ShaderCore
    {
        private int ProgramId { get; set; }
        private int VertexShaderId { get; set; }
        private int FragmentShaderId { get; set; }


        public ShaderCore(string name)
        {
            VertexShaderId = LoadShader(name, ShaderType.VertexShader);
            FragmentShaderId = LoadShader(name, ShaderType.FragmentShader);
            ProgramId = GL.CreateProgram();

            GL.AttachShader(ProgramId, VertexShaderId);
            GL.AttachShader(ProgramId, FragmentShaderId);

            GL.LinkProgram(ProgramId);
            GL.ValidateProgram(ProgramId);

            BindAttributes();
        }

        public void Start()
        {
            GL.UseProgram(ProgramId);
        }
        public void Stop()
        {
            GL.UseProgram(0);
        }
        public void CleanUp()
        {
            Stop();
            GL.DetachShader(ProgramId, VertexShaderId);
            GL.DetachShader(ProgramId, FragmentShaderId);

            GL.DeleteShader(VertexShaderId);
            GL.DeleteShader(FragmentShaderId);

            GL.DeleteProgram(ProgramId);
        }

        protected abstract void BindAttributes();

        protected void BindAttribute(int attrib, string name)
        {
            GL.BindAttribLocation(ProgramId, attrib, name);
        }

        private int LoadShader(string name, ShaderType type)
        {
            int shaderId = GL.CreateShader(type);

            string data = Loader.GetShader(name, type);

            GL.ShaderSource(shaderId, data);
            GL.CompileShader(shaderId);

            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int result);
            if (result == 0)
            {
                Console.WriteLine(GL.GetShaderInfoLog(shaderId));
                Environment.Exit(-1);
            }

            return shaderId;
        }
    }
}
