using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Http.Features;

namespace Albelli.Lambda.Templates.Core.Pipelines
{
    public interface IAspNetResponsePipelineHandler<in TResponse>
    {
        void PostMarshallResponseFeature(IHttpResponseFeature aspNetCoreResponseFeature, TResponse lambdaResponse, ILambdaContext lambdaContext);
    }
}