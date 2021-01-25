using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

namespace Albelli.Lambda.Templates.Sqs
{
    public interface ISqsMessagePipelineHandler
    {
        void HookBefore(SQSEvent.SQSMessage snsRecord, ILambdaContext lambdaContext);
        void HookAfter(SQSEvent.SQSMessage snsRecord, ILambdaContext lambdaContext);
    }
}