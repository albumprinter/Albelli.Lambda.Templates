using Albelli.Templates.Amazon.Core.Sqs;
using Albelli.Templates.Amazon.Sqs.Lambda.Notifications;
using JetBrains.Annotations;

namespace Albelli.Templates.Amazon.Sqs.Lambda
{
    [PublicAPI]
    public class LambdaEntryPoint : SqsProxyFunction<NotificationDto, Startup>
    {
        public LambdaEntryPoint()
        {
            ChooseSequentialExecutionMode();
        }
    }
}