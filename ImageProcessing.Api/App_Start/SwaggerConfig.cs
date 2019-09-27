using System.Web.Http;
using WebActivatorEx;
using api;
using Swashbuckle.Application;
using api.Swagger;
using System.Configuration;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace api
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            if (ConfigurationManager.AppSettings["DisableSwagger"] == "True")
            {
                return;
            }
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "API Image Processing");
                        c.IncludeXmlComments(string.Format(@"{0}\bin\ImageProcessing.Api.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                        c.IgnoreObsoleteActions();
                        c.UseFullTypeNameInSchemaIds();
                        c.DescribeAllEnumsAsStrings();
                        c.BasicAuth("basic").Description("Basic HTTP Authentication");
                        c.OperationFilter<AuthorizationHeaderFilter>();
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocExpansion(DocExpansion.List);
                    });
        }
    }
    
}
