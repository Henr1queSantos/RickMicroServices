using Mango.Services.EmailAPI.Messaging;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace Mango.Services.EmailAPI.Extension
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplcationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplcationLife.ApplicationStarted.Register(OnStart);
            hostApplcationLife.ApplicationStopping.Register(OnStop);

            return app;
        }

        private static void OnStop()
        {
            ServiceBusConsumer.Stop();
        }

        private static void OnStart()
        {
            ServiceBusConsumer.Start();
        }
    }
}
