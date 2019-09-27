using ImageProcessing.Api.ViewModel;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ImagesProcessing.Service
{
    public class ResizeService
    {
        ConvertUnit convert = new ConvertUnit();
        public List<ResultUploadVM> CompressFullHD(HttpRequest httpRequest)
        {
            List<ResultUploadVM> listImage = new List<ResultUploadVM>();
            MemoryStream outStream = new MemoryStream();
            ISupportedImageFormat format = new JpegFormat { Quality = 80 };

            if (httpRequest.Files.Count > 0)
            {
                for (int file = 0; file < httpRequest.Files.Count; file++)
                {
                    var postedFile = httpRequest.Files[file];
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        imageFactory.Load(postedFile.InputStream);

                        Size size = convert.MaxPixel(imageFactory.Image.Width, imageFactory.Image.Height);

                        ResizeLayer layer = new ResizeLayer(size, ResizeMode.Stretch, AnchorPosition.Center);

                        imageFactory.Resize(layer)
                                    .Format(format)
                                    .Save(outStream);

                        listImage.Add(new ResultUploadVM
                        {
                            filename = postedFile.FileName,
                            contenttype = postedFile.ContentType,
                            real_size = convert.FromByte(postedFile.ContentLength),
                            convert_size = convert.FromByte(outStream.Length),
                            width = size.Width,
                            height = size.Height,
                            data_image = "data:" + postedFile.ContentType + ";base64," + Convert.ToBase64String(outStream.ToArray()),
                        });


                    }
                }
            }
            return listImage;
        }

        public List<ResultUploadVM> CustomSize(HttpRequest httpRequest, RequestConvertVM model)
        {
            List<ResultUploadVM> listImage = new List<ResultUploadVM>();
            MemoryStream outStream = new MemoryStream();
            ISupportedImageFormat format = new JpegFormat { Quality = 80 };

            if (httpRequest.Files.Count > 0)
            {
                for (int file = 0; file < httpRequest.Files.Count; file++)
                {
                    var postedFile = httpRequest.Files[file];
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        imageFactory.Load(postedFile.InputStream);

                        Size size = new Size(model.Width, model.Height);

                        ResizeLayer layer = ResizeModeService.Mode(size, model.ResizeMode);

                        imageFactory.Resize(layer)
                                    .Format(format)
                                    .Save(outStream);

                        listImage.Add(new ResultUploadVM
                        {
                            filename = postedFile.FileName,
                            contenttype = postedFile.ContentType,
                            real_size = convert.FromByte(postedFile.ContentLength),
                            convert_size = convert.FromByte(outStream.Length),
                            width = size.Width,
                            height = size.Height,
                            data_image = "data:" + postedFile.ContentType + ";base64," + Convert.ToBase64String(outStream.ToArray()),
                        });


                    }
                }
            }
            return listImage;
        }
    }
}
