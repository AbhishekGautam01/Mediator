using Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mediator.Sample
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                //.//AddTransient<PrintToConsoleHandler>()
                .AddMediator(ServiceLifetime.Scoped, typeof(Program))
                .BuildServiceProvider();

            

            var request = new PrintToConsoleRequest
            {
                Text = "hello from mediator"
            };

           // IMediator mediator = new Mediator(serviceProvider.GetRequiredService, handlerDetails);

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            await mediator.SendAsync(request);  

        }
    }
}
