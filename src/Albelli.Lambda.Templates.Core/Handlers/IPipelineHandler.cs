using Amazon.Lambda.Core;

namespace Albelli.Lambda.Templates.Core.Handlers
{
    public interface IPipelineHandler<in TItem>
    {
        void HookBefore(TItem item, ILambdaContext lambdaContext);
        void HookAfter(TItem item, ILambdaContext lambdaContext);
    }
}