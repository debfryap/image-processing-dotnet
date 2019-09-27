using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesProcessing.Service
{
    public class ConvertUnit
    {
        private const long Unit = 1024;
        private const int MaxResolution = 1920; // Full HD
        public string FromByte(float Size)
        {
            string[] unit = new string[] { "KB", "MB", "GB" };
            int loop = 0;
            string Temp = "";

            while (Size > Unit)
            {
                Size /= Unit;

                Temp = string.Format("{0:0.##}", Size) + " " + unit[loop];
                loop++;
            }

            return Temp;
        }

        public Size MaxPixel(int Width, int Height)
        {
            int IsMax = Width > Height ? Width : Height;

            while (IsMax > MaxResolution)
            {
                IsMax /= 2;
                Width /= 2;
                Height /= 2;
            }

            return new Size(Width,Height);
        }
    }
}
