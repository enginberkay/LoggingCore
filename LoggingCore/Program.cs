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
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.RabbitMQ;
using Serilog.Sinks.SystemConsole.Themes;

namespace LoggingCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "LoggingCore.Green")
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                //.WriteTo.Elasticsearch(
                //    new ElasticsearchSinkOptions(
                //        new Uri("http://dockercompose7204030419852013118_elasticsearch_1:9200/"))
                //    {
                //        CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                //        AutoRegisterTemplate = true,
                //        TemplateName = "serilog-events-template",
                //        IndexFormat = "loggingcore-log-{0:yyyy.MM.dd}"
                //    })
                .WriteTo.File(@"lifeincore_log.log", LogEventLevel.Debug)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                .WriteTo.RabbitMQ((clientConfiguration, sinkConfiguration) =>
                {
                    clientConfiguration.Username = config.GetSection("Serilog:RabbitMq:Username").Value;
                    clientConfiguration.Password = config.GetSection("Serilog:RabbitMq:Password").Value;
                    clientConfiguration.Exchange = config.GetSection("Serilog:RabbitMq:Exchange").Value;
                    clientConfiguration.ExchangeType = config.GetSection("Serilog:RabbitMq:ExchangeType").Value;
                    clientConfiguration.DeliveryMode = RabbitMQDeliveryMode.Durable;
                    clientConfiguration.RouteKey = config.GetSection("Serilog:RabbitMq:RouteKey").Value;
                    clientConfiguration.Port = 5672;
                    clientConfiguration.Hostnames.Add(config.GetSection("Serilog:RabbitMq:Hostname").Value);
                    clientConfiguration.VHost = config.GetSection("Serilog:RabbitMq:VHost").Value;
                    sinkConfiguration.TextFormatter = new JsonFormatter();
                })
                .MinimumLevel.Verbose()
                .CreateLogger();
            try
            {
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
