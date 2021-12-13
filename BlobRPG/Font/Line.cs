using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Font
{
    class Line
    {
        public double MaxLength { get; }
        public double SpaceSize { get; }

        public List<Word> Words { get; }
        public double CurrentLineLength { get; private set; }

        public Line(double spaceWidth, double fontSize, double maxLength)
        {
            Words = new List<Word>();
            CurrentLineLength = 0;

            SpaceSize = fontSize * spaceWidth;
            MaxLength = maxLength;
        }

        public bool TryAddWord(Word word)
        {
            double length = word.Width;
            length += Words.Count == 0 ? 0 : SpaceSize;
            if (length + CurrentLineLength <= MaxLength)
            {
                Words.Add(word);
                CurrentLineLength += length;
                return true;
            }
            return false;
        }
    }
}
