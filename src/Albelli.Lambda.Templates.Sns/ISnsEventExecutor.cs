using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;

namespace Albelli.Lambda.Templates.Sns
{
    public interface ISnsEventExecutor
    {
        Task Execute(SNSEvent snsEvent, ILambdaContext lambdaContext, Func<SNSEvent.SNSRecord, ILambdaContext, Task> recordExecutor);
    }

    public class ParallelSnsEventExecutor : ISnsEventExecutor
    {
        private readonly int _batchSize;

        public ParallelSnsEventExecutor(int batchSize)
        {
            _batchSize = batchSize;
        }

        public async Task Execute(SNSEvent snsEvent, ILambdaContext lambdaContext, Func<SNSEvent.SNSRecord, ILambdaContext, Task> recordExecutor)
        {
            using var concurrencySemaphore = new SemaphoreSlim(_batchSize);
            var tasks = new List<Task>();
            foreach(var record in snsEvent.Records)
            {
                await concurrencySemaphore.WaitAsync();

                var task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        recordExecutor(record, lambdaContext);
                    }
                    finally
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        concurrencySemaphore.Release();
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray());
        }
    }

    public class SequentialSnsEventExecutor : ISnsEventExecutor
    {
        public async Task Execute(SNSEvent snsEvent, ILambdaContext lambdaContext, Func<SNSEvent.SNSRecord, ILambdaContext, Task> recordExecutor)
        {
            foreach (var snsRecord in snsEvent.Records)
            {
                await recordExecutor(snsRecord, lambdaContext);
            }
        }
    }
}