﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mediator
{
    public class Mediator : IMediator
    {
        private readonly Func<Type, object> _serviceResolver;
        private readonly IDictionary<Type, Type> _handlerDetails;
        public Mediator(Func<Type, object> serviceResolver, IDictionary<Type, Type> handlerDetails)
        {
            _serviceResolver = serviceResolver; 
            _handlerDetails = handlerDetails;   
        }
        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            var requestType = request.GetType();
            if (!_handlerDetails.ContainsKey(requestType))
            {
                throw new Exception($"No handler to handle request type: {requestType.Name}");
            }

            _handlerDetails.TryGetValue(requestType, out var requestHandlerType);
            var handler = _serviceResolver(requestHandlerType);

            return await (Task<TResponse>)handler
                .GetType()
                .GetMethod("HandleAsync")
                .Invoke(handler, new object[] { request });
        }
    }
}
