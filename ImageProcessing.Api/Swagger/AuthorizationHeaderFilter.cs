using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Filters;

namespace api.Swagger
{
    public class AuthorizationHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var filterPipeline = apiDescription.ActionDescriptor.GetFilterPipeline();

            if (filterPipeline.Count > 0)
            {
                if (operation.parameters == null)
                    operation.parameters = new List<Parameter>();

                operation.parameters.Add(new Parameter
                {
                    name = "Authorization",
                    @in = "header",
                    description = "Basic HTTP Base64 encoded Header Authorization",
                    required = true,
                    type = "string"
                });

                //operation.parameters.Add(new Parameter
                //{
                //    name = "Password",
                //    @in = "header",
                //    description = "Basic HTTP Base64 encoded Header Authorization",
                //    required = true,
                //    type = "string"
                //});
            }
        }
    }
}