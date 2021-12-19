using BlobRPG.Entities;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Font
{
    public class FontType
    {
        public int TextureAtlas { get; private set; }
        private Creator Creator;
        public FontType(int textureAtlas, string fontFile, Window window)
        {
            TextureAtlas = textureAtlas;
            FileStream fs = File.OpenRead(fontFile);
            Creator = new Creator(fs, window);
            fs.Close();

        }
        public FontType(int textureAtlas, Stream fontFile, Window window)
        {
            TextureAtlas = textureAtlas;
            Creator = new Creator(fontFile, window);
        }

        public TextMeshData LoadText(GUIText Text)
        {
            return Creator.CreateTextMesh(Text);
        }
    }
}
