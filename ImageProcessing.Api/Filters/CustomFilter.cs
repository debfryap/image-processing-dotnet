using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Net.Http;
using NLog;
using System.Web.Http.Controllers;
using System.Diagnostics;
using System.Web.Http.Filters;
using ImageProcessing.Api.ViewModel;

namespace api.Filters
{
    public class ExceptionHandlingFilterAttribute : ExceptionFilterAttribute
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private HttpActionContext FilterContext { get; set; }

        private string ActionName
        {
            get { return FilterContext.ActionDescriptor.ActionName; }
        }

        private string ControllerName
        {
            get { return FilterContext.ActionDescriptor.ControllerDescriptor.ControllerName; }
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            FilterContext = context.ActionContext;

            var result = new ResponseAPI<string>();

            var exception = context.Exception;
            if (exception != null)
            {
                result.header = new HeaderOutput()
                {
                    process_time = ActionHandlingFilterAttribute.Watch.ElapsedMilliseconds,
                    errors = new List<ErrorOutput>()
                    {
                        new ErrorOutput(){
                            message = context.Exception.Message
                        }
                    }
                };

                context.Response = context.Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, result);
                
            }

            ActionHandlingFilterAttribute.Watch.Stop();
            logger.Error("timeMillis:[" + ActionHandlingFilterAttribute.Watch.Elapsed.TotalMilliseconds + "] data:" + context.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName + "/" + context.ActionContext.ActionDescriptor.ActionName + "||" + "END||" + context.Exception.Message);
        }
    }

    public class ActionHandlingFilterAttribute : ActionFilterAttribute
    {
        public static Stopwatch Watch { get; set; }
        private Logger logger = LogManager.GetCurrentClassLogger();

        private HttpActionContext ActionContext { get; set; }

        private string ActionName
        {
            get { return ActionContext.ActionDescriptor.ActionName; }
        }

        private string ControllerName
        {
            get { return ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName; }
        }

        private string Url
        {
            get { return ActionContext.Request.RequestUri.Authority; }
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            var result = new ResponseAPI<string>();

            try
            {
                ActionContext = actionContext;

                Watch = Stopwatch.StartNew();
                
                logger.Info("timeMillis:[" + Watch.Elapsed.TotalMilliseconds + "] data:" + ControllerName + "/" + ActionName + "||" + "START");

                if (actionContext.Request.Headers.Authorization == null)
                {
                    result.header = new HeaderOutput()
                    {
                        process_time = Watch.ElapsedMilliseconds,
                        errors = new List<ErrorOutput>()
                        {
                            new ErrorOutput(){
                                message = "Forbidden",
                                cause = "Authorization header is null",
                                code = "99"
                            }
                        }
                    };

                    actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Forbidden, result);
                }
                else
                {
                    bool isUserValid = ParseRequestHeaders(actionContext);
                    if (!isUserValid)
                    { 
                        result.header = new HeaderOutput()
                        {
                            process_time = Watch.ElapsedMilliseconds,
                            errors = new List<ErrorOutput>()
                            {
                                new ErrorOutput(){
                                    message = "Unauthorized",
                                    cause = "Credential invalid",
                                    code = "99"
                                }
                            }
                        };

                        actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized, result);
                    }
                    else
                    {
                        if (!actionContext.ModelState.IsValid)
                        {
                            var errors = new List<ErrorOutput>();
                            var errorLog = new StringBuilder();

                            foreach (var state in actionContext.ModelState)
                            {
                                foreach (var error in state.Value.Errors)
                                {
                                    if (error.ErrorMessage != "")
                                    {
                                        errors.Add(new ErrorOutput()
                                        {
                                            message = error.ErrorMessage
                                        });

                                        if (errors.Count > 1)
                                        {
                                            errorLog.Append(", ");
                                        }
                                        errorLog.Append(error.ErrorMessage);
                                    }
                                    else
                                    {
                                        errors.Add(new ErrorOutput()
                                        {
                                            message = error.Exception.Message
                                        });

                                        if (errors.Count > 1)
                                        {
                                            errorLog.Append(", ");
                                        }
                                        errorLog.Append(error.ErrorMessage);
                                    }
                                }
                            }

                            Watch.Stop();

                            logger.Error("timeMillis:[" + Watch.Elapsed.TotalMilliseconds + "] data:" + ControllerName + "/" + ActionName + "||" + "END||ModelState is not valid, {0}", errorLog);

                            result.header = new HeaderOutput()
                            {
                                process_time = ActionHandlingFilterAttribute.Watch.ElapsedMilliseconds,
                                errors = errors
                            };

                            actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ActionHandlingFilterAttribute.Watch.Stop();
                logger.Error("timeMillis:[" + Watch.Elapsed.TotalMilliseconds + "] data:" + ControllerName + "/" + ActionName + "||" + "END||" + ex.Message);

                result.header = new HeaderOutput()
                {
                    process_time = Watch.ElapsedMilliseconds,
                    errors = new List<ErrorOutput>()
                    {
                        new ErrorOutput(){
                            message = ex.Message,
                            code = "99"
                        }
                    }
                };

                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, result);
            }
        }

        //public override void OnActionExecuted(HttpActionExecutedContext actionContext)
        //{
        //    base.OnActionExecuted(actionContext);

        //    try
        //    {
        //        var exception = actionContext.Exception;

        //        if (exception == null)
        //        {
        //            Watch.Stop();

        //            logger.Info("timeMillis:[" + Watch.Elapsed.TotalMilliseconds + "] data:" + ControllerName + "/" + ActionName + "||" + "END");

        //            var objectContent = actionContext.Response.Content as ObjectContent;

        //            if (objectContent != null)
        //            {
        //                var result = ((dynamic)objectContent.Value);

        //                if (result.header == null)
        //                {
        //                    result.header = new HeaderOutput()
        //                    {
        //                        process_time = ActionHandlingFilterAttribute.Watch.ElapsedMilliseconds,
        //                        errors = new List<ErrorOutput>()
        //                    };

        //                    actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.OK, (object)result);
        //                }
        //                else
        //                {
        //                    result.header.process_time = ActionHandlingFilterAttribute.Watch.ElapsedMilliseconds;

        //                    if (result.header.errors != null)
        //                    {
        //                        result.Data = null;
        //                        logger.Error("timeMillis:[" + Watch.Elapsed.TotalMilliseconds + "] data:" + ControllerName + "/" + ActionName + "||" + "END||" + result.Header.Errors[0].Message);
        //                        actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, (object)result);
        //                    }
        //                    else
        //                    {
        //                        actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.OK, (object)result);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var result = new ResponseAPI<string>();

        //        Watch.Stop();
        //        logger.Error("timeMillis:[" + Watch.Elapsed.TotalMilliseconds + "] data:" + ControllerName + "/" + ActionName + "||" + "END||" + ex.Message);

        //        result.header = new HeaderOutput()
        //        {
        //            process_time = Watch.ElapsedMilliseconds,
        //            errors = new List<ErrorOutput>()
        //            {
        //                new ErrorOutput(){
        //                    message = ex.Message,
        //                    code = "99"
        //                }
        //            }
        //        };

        //        actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, result);
        //    }
        //}

        private Boolean ParseRequestHeaders(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var requestHeaderAuth = actionContext.Request.Headers.GetValues("Authorization").FirstOrDefault();
            var requestHeaderAuthType = requestHeaderAuth.Substring(0, 5);
            if (requestHeaderAuthType == "Basic")
            {
                requestHeaderAuth = Encoding.UTF8.GetString(Convert.FromBase64String(requestHeaderAuth.Remove(0, 6)));
            }
            else
            {
                return false;
            }

            string[] requestHeaderAuthValues = requestHeaderAuth.Split(':');
            string username = requestHeaderAuthValues[0];
            string password = requestHeaderAuthValues[1];

            //UserRepository userRepository = new UserRepository();
            //var user = userRepository.GetUser(username);
            //if (user == null)
            //{
            //    return false;
            //}

            //bool isVerify = BCrypt.Net.BCrypt.Verify(password, user.password);
            //if (isVerify)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

            if (username == "apiImages" && password == "bijikud4")
            {
                return true;
            }
            return false;
        }       
    }
}
