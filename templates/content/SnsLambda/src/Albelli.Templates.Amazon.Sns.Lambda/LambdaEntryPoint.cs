using Albelli.Templates.Amazon.Core.Sns;
using Albelli.Templates.Amazon.Sns.Lambda.Notifications;
using JetBrains.Annotations;

namespace Albelli.Templates.Amazon.Sns.Lambda
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