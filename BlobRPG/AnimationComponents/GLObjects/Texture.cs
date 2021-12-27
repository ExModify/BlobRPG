using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.AnimationComponents.GLObjects
{
    public class Texture
    {
        public int TextureId { get; private set; }
        public int Size { get; private set; }
        public TextureTarget Target { get; private set; }

        public Texture(int textureId, int size, TextureTarget target = TextureTarget.Texture2D)
        {
            TextureId = textureId;
            Size = size;
            Target = target;
        }

        public void BindToUnit(int unit = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            GL.BindTexture(Target, TextureId);
        }
        public void Delete()
        {
            GL.DeleteTexture(TextureId);
        }
    }
}
