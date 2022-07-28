namespace Sample.Service
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;
    using Sample.Components.Consumers;

    class Program
    {

        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
               Host.CreateDefaultBuilder(args)
               .ConfigureServices((services) =>
               {
                   services.AddMassTransit(x =>
                   {
                       x.SetKebabCaseEndpointNameFormatter();
                       x.SetInMemorySagaRepositoryProvider();

                       var entryAssembly = Assembly.GetEntryAssembly();

                       x.AddConsumer(entryAssembly);

                       x.UsingRabbitMq((cxt, cfg) =>
                       {
                           cfg.Host("localhost", "/", h =>

                            {
                                h.Username("guest");
                                h.Password("guest");

                            });

                           cfg.ConfigureEndpoints(cxt);
                       });

                   });

               });
    }
}



//static async Task Main(string[] args)
//{
//    var isService = !(Debugger.IsAttached || args.Contains("--console"));

//    var builder = new HostBuilder()
//        .ConfigureAppConfiguration((hostingContext, config) =>
//        {
//            config.AddJsonFile("appsettings.json", true);
//            config.AddEnvironmentVariables();

//            if (args != null)
//                config.AddCommandLine(args);
//        })
//            .ConfigureServices((hostContext, services) =>
//            {
//                services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
//                services.AddMassTransit(cfg =>
//                {
//                    cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();

//                    cfg.UsingRabbitMq(hostContext, cfg);
//                            //cfg.AddBus(ConfigureBus);
//                        });
//            })

//            .ConfigureLogging((hostingContext, logging) =>
//            {
//                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
//                logging.AddConsole();
//            });
//    if (isService)
//        await builder.UseWindowsService().Build().RunAsync();
//    else
//        await builder.RunConsoleAsync();

//    static IBusControl ConfigureBus(IBusRegistrationContext context)
//    {
//        return Bus.Factory.CreateUsingRabbitMq(cfg =>
//        {
//            cfg.ConfigureEndpoints(context);

//        });
//    }


//}



