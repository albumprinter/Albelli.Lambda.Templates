using System.Threading.Tasks;
using Albelli.SNS.Lambda;
using Amazon.Lambda.SNSEvents;
using EMG.Lambda.LocalRunner;
using JetBrains.Annotations;

namespace Albelli.LambdaSubscriptionTemplate.Local
{
    /// <summary>
    /// Use this to test whole lambda execution pipeline. For logic testing have a look at Lambda.LocalEntryPoint
    /// </summary>
    [PublicAPI]
    public sealed class LocalEntryPoint
    {
        private static async Task Main(string[] args)
        {
            await LambdaRunner.Create()
                .Receives<SNSEvent>()
                .UsesAsyncFunctionWithNoResult<LambdaEntryPoint>((function, input, context) =>
                    function.FunctionHandlerAsync(input, context))
                .Build()
                .RunAsync();
        }
    }
}