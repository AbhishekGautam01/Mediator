using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediator.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services,ServiceLifetime lifetime, params Type[] markers)
        {
            var handlerInfo = new Dictionary<Type, Type>();
            foreach (var marker in markers)
            {
                var assembly = marker.Assembly;
                var requests = GetClasssesImplementingInterface(assembly, typeof(IRequest<>));
                var handlers = GetClasssesImplementingInterface(assembly, typeof(IHandler<,>));

                requests.ForEach(x =>
                {
                    handlerInfo[x] = handlers.SingleOrDefault(xx => x == xx.GetInterface("IHandler`2")!.GetGenericArguments()[0]);
                });
                var serviceDescriptors  = handlers.Select(x => new ServiceDescriptor(x, x, lifetime));
                services.TryAdd(serviceDescriptors);
            }

            services.AddSingleton<IMediator>(x => new Mediator(x.GetRequiredService, handlerInfo));
            return services;
        }

        private static List<Type> GetClasssesImplementingInterface(System.Reflection.Assembly assembly, Type typeToMatch)
        {
            var request = assembly.ExportedTypes
                                .Where(type =>
                                {
                                    var genericInterfaceTypes = type.GetInterfaces().Where(x => x.IsGenericType).ToList();
                                    var implementRequestType = genericInterfaceTypes.Any(x => x.GetGenericTypeDefinition() == typeToMatch);

                                    return !type.IsInterface && !type.IsAbstract && implementRequestType;
                                }).ToList();
            return request;
        }
    }
}
