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
        ResizeService resize = new ResizeService();

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
        [Route("compressfullhd")]
        public IHttpActionResult CompressFullHD()
        {
            var result = new ResponseAPI<object>();
            var watch = ActionHandlingFilterAttribute.Watch;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var resultUpload = resize.CompressFullHD(httpRequest);

                watch.Stop();
                result.header = new HeaderOutput()
                {
                    process_time = watch.ElapsedMilliseconds,
                };
                result.data = resultUpload;

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
