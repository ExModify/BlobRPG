using BlobRPG.MainComponents;
using GlmSharp;
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
            Console.WriteLine("Compiling shader: " + name);
            VertexShaderId = LoadShader(name, ShaderType.VertexShader);
            FragmentShaderId = LoadShader(name, ShaderType.FragmentShader);
            ProgramId = GL.CreateProgram();

            GL.AttachShader(ProgramId, VertexShaderId);
            GL.AttachShader(ProgramId, FragmentShaderId);

            BindAttributes();

            GL.LinkProgram(ProgramId);
            GL.ValidateProgram(ProgramId);

            GetAllUniformLocations();
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

        protected void LoadFloat(int location, float value)
        {
            GL.Uniform1(location, value);
        }
        protected void LoadInt(int location, int value)
        {
            GL.Uniform1(location, value);
        }
        protected void LoadBool(int location, bool value)
        {
            GL.Uniform1(location, value ? 1 : 0);
        }
        protected void LoadVector(int location, vec2 vector)
        {
            GL.Uniform2(location, vector.x, vector.y);
        }
        protected void LoadVector(int location, vec3 vector)
        {
            GL.Uniform3(location, vector.x, vector.y, vector.z);
        }
        protected void LoadVector(int location, vec4 vector)
        {
            GL.Uniform4(location, vector.x, vector.y, vector.z, vector.w);
        }
        protected void LoadMatrix(int location, mat4 matrix)
        {
            GL.UniformMatrix4(location, 1, false, ref matrix.Values1D[0]);
            
        }

        protected abstract void BindAttributes();
        protected abstract void GetAllUniformLocations();

        protected void BindAttribute(int attrib, string name)
        {
            GL.BindAttribLocation(ProgramId, attrib, name);
        }
        protected int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(ProgramId, name);
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
