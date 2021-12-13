using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Font
{
	public class Character
	{
		public int Id { get; private set; }
		public double XTextureCoord { get; private set; }
		public double YTextureCoord { get; private set; }
		public double XMaxTextureCoord { get; private set; }
		public double YMaxTextureCoord { get; private set; }
		public double XOffset { get; private set; }
		public double YOffset { get; private set; }
		public double SizeX { get; private set; }
		public double SizeY { get; private set; }
		public double XAdvance { get; private set; }

		public Character(int id, double xTextureCoord, double yTextureCoord, double xTexSize, double yTexSize,
			double xOffset, double yOffset, double sizeX, double sizeY, double xAdvance)
		{
			Id = id;
			XTextureCoord = xTextureCoord;
			YTextureCoord = yTextureCoord;
			XOffset = xOffset;
			YOffset = yOffset;
			SizeX = sizeX;
			SizeY = sizeY;
			XAdvance = xAdvance;

			XMaxTextureCoord = xTexSize + xTextureCoord;
			YMaxTextureCoord = yTexSize + yTextureCoord;
		}
	}
}
