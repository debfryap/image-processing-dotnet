using api.Filters;
using ImageProcessing.Api.ViewModel;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using ImagesProcessing.Service;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ImageProcessing.Api.Controllers
{
    [RoutePrefix("v1/images")]
    public class ImagesController : ApiController
    {
        ConvertUnit convert = new ConvertUnit();

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseAPI<string>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [ActionHandlingFilter]
        [HttpGet]
        [Route("ping")]
        public IHttpActionResult Ping()
        {
            return Ok(200);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseAPI<string>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [ActionHandlingFilter]
        [HttpPost]
        [Route("resize")]
        public IHttpActionResult Resize()
        {
            var result = new ResponseAPI<object>();
            var watch = ActionHandlingFilterAttribute.Watch;
            try
            {
                List<ResultUploadVM> listImage = new List<ResultUploadVM>();

                ISupportedImageFormat format = new JpegFormat { Quality = 80 };
                var httpRequest = HttpContext.Current.Request;


                MemoryStream outStream = new MemoryStream();

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

                watch.Stop();
                result.header = new HeaderOutput()
                {
                    process_time = watch.ElapsedMilliseconds,
                };
                result.data = listImage;

                return Ok(result);
            }
            catch (Exception e)
            {
                watch.Stop();
                result.header = new HeaderOutput()
                {
                    process_time = watch.ElapsedMilliseconds,
                    errors = new List<ErrorOutput>()
                        {
                            new ErrorOutput(){
                                message = e.Message,
                                cause = e.Message,
                                code = "99"
                            }
                        }
                };
                return Ok(result);
            }

        }

    }
}
