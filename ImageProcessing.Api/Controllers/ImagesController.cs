using api.Filters;
using ImageProcessing.Api.ViewModel;
using ImagesProcessing.Service;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
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
        [Route("compress")]
        public IHttpActionResult Compress()
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

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseAPI<string>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [ActionHandlingFilter]
        [HttpPost]
        [Route("resize")]
        public IHttpActionResult Resize([FromUri]int Height = 100, int Width = 100, string ResizeMode = "crop")
        {
            var result = new ResponseAPI<object>();
            var watch = ActionHandlingFilterAttribute.Watch;
            var paramRequest = new RequestConvertVM()
            {
                Height = Height,
                Width = Width,
                ResizeMode = ResizeMode
            };
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var resultUpload = resize.CustomSize(httpRequest, paramRequest);

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
