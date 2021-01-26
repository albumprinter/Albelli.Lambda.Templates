using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Http.Features;

namespace Albelli.Templates.Amazon.Core.Pipelines
{
    public interface IAspNetResponsePipelineHandler<in TResponse>
    {
        void PostMarshallResponseFeature(IHttpResponseFeature aspNetCoreResponseFeature, TResponse lambdaResponse, ILambdaContext lambdaContext);
    }
}