using BlobRPG.Models;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Shaders
{
    public class TextShader : ShaderCore
    {
        private int ColorLocation;
        private int TranslationLocation;

        private int WidthLocation;
        private int EdgeLocation;

        private int BorderWidthLocation;
        private int BorderEdgeLocation;
        private int OutlineColorLocation;

        private int OffsetLocation;

        public TextShader() : base("font")
        {
        }

        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
            BindAttribute(1, "textureCoords");
        }

        protected override void GetAllUniformLocations()
        {
            ColorLocation = GetUniformLocation("color");
            TranslationLocation = GetUniformLocation("translation");

            WidthLocation = GetUniformLocation("width");
            EdgeLocation = GetUniformLocation("edge");

            BorderWidthLocation = GetUniformLocation("borderWidth");
            BorderEdgeLocation = GetUniformLocation("borderEdge");
            OutlineColorLocation = GetUniformLocation("outlineColor");

            OffsetLocation = GetUniformLocation("offset");
        }
        public void LoadText(GUIText text)
        {
            LoadVector(ColorLocation, text.Color);
            LoadVector(TranslationLocation, text.Position);

            LoadFloat(WidthLocation, text.Width);
            LoadFloat(EdgeLocation, text.Edge);

            LoadFloat(BorderWidthLocation, text.BorderWidth);
            LoadFloat(BorderEdgeLocation, text.BorderEdge);
            LoadVector(OutlineColorLocation, text.BorderColor);

            LoadVector(OffsetLocation, text.DropShadow);
        }
    }
}
