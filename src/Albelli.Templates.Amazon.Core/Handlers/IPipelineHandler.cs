using Amazon.Lambda.Core;

namespace Albelli.Templates.Amazon.Core.Handlers
{
    public interface IPipelineHandler<in TItem>
    {
        void HookBefore(TItem item, ILambdaContext lambdaContext);
        void HookAfter(TItem item, ILambdaContext lambdaContext);
    }
}