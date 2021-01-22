using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Http.Features;

namespace Albelli.Lambda.Templates.Core.AspNet
{
    public interface IAspNetResponsePipelineHandler<in TResponse>
    {
        void PostMarshallResponseFeature(IHttpResponseFeature aspNetCoreResponseFeature, TResponse lambdaResponse, ILambdaContext lambdaContext);
    }
}