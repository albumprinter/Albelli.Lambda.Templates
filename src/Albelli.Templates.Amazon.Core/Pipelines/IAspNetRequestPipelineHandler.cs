using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Http.Features;

namespace Albelli.Templates.Amazon.Core.Pipelines
{
    public interface IAspNetRequestPipelineHandler<in TRequest>
    {
        void PostMarshallRequestFeature(IHttpRequestFeature aspNetCoreRequestFeature, TRequest lambdaRequest, ILambdaContext lambdaContext);
    }
}