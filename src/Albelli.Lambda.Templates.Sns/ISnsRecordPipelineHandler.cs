using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;

namespace Albelli.Lambda.Templates.Sns
{
    public interface ISnsRecordPipelineHandler
    {
        void HookBefore(SNSEvent.SNSRecord snsRecord, ILambdaContext lambdaContext);
        void HookAfter(SNSEvent.SNSRecord snsRecord, ILambdaContext lambdaContext);
    }
}