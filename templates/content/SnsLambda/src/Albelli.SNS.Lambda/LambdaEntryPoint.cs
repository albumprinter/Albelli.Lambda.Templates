using Albelli.SNS.Lambda.Notifications;
using Albelli.Templates.Amazon.Core.Sns;
using JetBrains.Annotations;

namespace Albelli.SNS.Lambda
{
    [PublicAPI]
    public class LambdaEntryPoint : SnsProxyFunction<NotificationDto, Startup>
    {
        public LambdaEntryPoint()
        {
            ChooseSequentialExecutionMode();
        }
    }
}