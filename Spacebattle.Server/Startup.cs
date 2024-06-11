using CoreWCF;
using CoreWCF.Configuration;

using Swashbuckle.AspNetCore.Swagger;
using System.Xml;

using System.Diagnostics.CodeAnalysis;
[assembly: ExcludeFromCodeCoverage]


namespace Spacebattle.Server;
internal sealed class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddServiceModelWebServices(o =>
        {
            o.Title = "Spacebattle API";
            o.Version = "1";
            o.Description = "Описание API";
        });

        services.AddSingleton(new SwaggerOptions());
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<SwaggerMiddleware>();
        app.UseSwaggerUI();

        app.UseServiceModel(builder =>
        {

            var readerQuoates = new XmlDictionaryReaderQuotas
            {
                MaxBytesPerRead = 4096,
                MaxDepth = 32,
                MaxArrayLength = 16384,
                MaxStringContentLength = 16384,
                MaxNameTableCharCount = 16384
            };

            builder.AddService<WebApi>();
            builder.AddServiceWebEndpoint<WebApi, IWebApi>(new WebHttpBinding
            {
                MaxReceivedMessageSize = 5242880,
                MaxBufferSize = 65536,
                ReaderQuotas = readerQuoates
            }, "spacebattleApi", behavior =>
            {
                behavior.HelpEnabled = true;
                behavior.AutomaticFormatSelectionEnabled = true;
            });

        });
    }
}
