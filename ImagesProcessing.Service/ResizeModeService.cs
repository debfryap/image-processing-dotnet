using ImageProcessor.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesProcessing.Service
{
    public class ResizeModeService
    {
        public static ResizeLayer Mode(Size size, string mode)
        {
            ResizeMode resizeMode = new ResizeMode();
            switch (mode)
            {
                case "crop":
                    resizeMode = ResizeMode.BoxPad;
                    break;

                case "stretch":
                    resizeMode = ResizeMode.Stretch;
                    break;
            }
            return new ResizeLayer(size, resizeMode, AnchorPosition.Center);
        }
    }
}
