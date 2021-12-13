using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Font
{
	public class Word
	{
		public List<Character> Characters { get; private set; }
		public double Width { get; private set; }
		public double FontSize { get; private set; }

		public Word(double fontSize)
		{
			Characters = new List<Character>();
			Width = 0;
			FontSize = fontSize;
		}

		public void AddCharacter(Character character)
		{
			Characters.Add(character);
			Width += character.XAdvance * FontSize;
		}
	}
}
