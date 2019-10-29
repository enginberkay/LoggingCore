using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

namespace LoggingCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "LoggingCore.Green")
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(
                        new Uri("http://dockercompose7204030419852013118_elasticsearch_1:9200/"))
                    {
                        CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                        AutoRegisterTemplate = true,
                        TemplateName = "serilog-events-template",
                        IndexFormat = "loggingcore-log-{0:yyyy.MM.dd}"
                    })
                //.WriteTo.File(@"lifeincore_log.log", LogEventLevel.Debug)
                //.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                .MinimumLevel.Verbose()
                .CreateLogger();
            Log.Information("WebApi Starting...");
            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Log.Error(e, "@e");
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
