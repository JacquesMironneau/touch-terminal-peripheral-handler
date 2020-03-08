using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjetS3.SwaggerCustom
{
    class CustomMethodIntrospection : IDocumentFilter
    {
        void IDocumentFilter.Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {


            System.Console.WriteLine("AAAAAAAAAAAAAAAAAa");
            System.Console.WriteLine(swaggerDoc.Paths);
            swaggerDoc.Paths.Add("test",null);

            foreach (var o in swaggerDoc.Paths)
            {
                System.Console.WriteLine(o);
            }
            swaggerDoc.Tags = new List<OpenApiTag> {
                new OpenApiTag {Name = "BrowserRequest", Description = "ALED2"}
            };


        }
    }
}
