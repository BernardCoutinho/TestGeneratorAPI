namespace TestGeneratorAPI.src.API.Config
{
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Linq;
    using Microsoft.AspNetCore.Http;

    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Encontra parâmetros do tipo IFormFile
            var fileParams = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) ||
                            p.ParameterType == typeof(IFormFileCollection))
                .Select(p => p.Name).ToList();

            // Adiciona suporte para upload de arquivo no Swagger se parâmetros forem encontrados
            if (fileParams.Any())
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = fileParams.ToDictionary(
                                name => name,
                                name => new OpenApiSchema { Type = "string", Format = "binary" }
                            ),
                            Required = (ISet<string>)fileParams
                        }
                    }
                }
                };
            }
        }
    }
}


