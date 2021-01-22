using System.Collections.Generic;
using System.Linq;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.Core;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.Features;

namespace Albelli.Lambda.Templates.Core.AspNet
{
    [PublicAPI]
    public abstract class PipelinedAspNetCoreFunction<TRequest, TResponse> : AbstractAspNetCoreFunction<TRequest, TResponse>
    {
        protected PipelinedAspNetCoreFunction()
            : base()
        {

        }

        protected PipelinedAspNetCoreFunction(StartupMode startupMode)
            : base(startupMode)
        {
        }

        public List<IAspNetRequestPipelineHandler<TRequest>> AspNetRequestPipelineHandlers { get; } = new List<IAspNetRequestPipelineHandler<TRequest>>();
        public List<IAspNetResponsePipelineHandler<TResponse>> AspNetResponsePipelineHandlers { get; } = new List<IAspNetResponsePipelineHandler<TResponse>>();

        protected override void PostMarshallRequestFeature(IHttpRequestFeature aspNetCoreRequestFeature, TRequest lambdaRequest, ILambdaContext lambdaContext)
        {
            AspNetRequestPipelineHandlers.ForEach(handler => handler.PostMarshallRequestFeature(aspNetCoreRequestFeature, lambdaRequest, lambdaContext));
            base.PostMarshallRequestFeature(aspNetCoreRequestFeature, lambdaRequest, lambdaContext);
        }

        protected override void PostMarshallResponseFeature(IHttpResponseFeature aspNetCoreResponseFeature, TResponse lambdaResponse, ILambdaContext lambdaContext)
        {
            base.PostMarshallResponseFeature(aspNetCoreResponseFeature, lambdaResponse, lambdaContext);
            AspNetResponsePipelineHandlers.ToArray().Reverse().ToList().ForEach(handler => handler.PostMarshallResponseFeature(aspNetCoreResponseFeature, lambdaResponse, lambdaContext));
        }
    }
}