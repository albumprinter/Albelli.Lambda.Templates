using Albelli.LambdaSubscriptionTemplate.Lambda;
using Amazon.Lambda.SNSEvents;
using EMG.Lambda.LocalRunner;

namespace Albelli.LambdaSubscriptionTemplate.Local
{
    class Program
    {
        static void Main(string[] args)
        {
            LambdaRunner.Create()
                .Receives<SNSEvent>()
                .UsesAsyncFunctionWithNoResult<Function>((function, input, context) =>
                    function.FunctionHandlerAsync(input, context))
                .Build()
                .RunAsync()
                .Wait();
        }
    }
}