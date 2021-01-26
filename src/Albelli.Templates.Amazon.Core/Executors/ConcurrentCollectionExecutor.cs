using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Albelli.Templates.Amazon.Core.Executors
{
    public class ConcurrentCollectionExecutor : ICollectionExecutor
    {
        private readonly int _batchSize;

        public ConcurrentCollectionExecutor(int batchSize)
        {
            _batchSize = batchSize;
        }

        public async Task Execute<T>(IEnumerable<T> collection, Func<T, Task> itemExecutor)
        {
            using var concurrencySemaphore = new SemaphoreSlim(_batchSize);
            var tasks = new List<Task>();
            foreach(var item in collection)
            {
                await concurrencySemaphore.WaitAsync();

                var task = Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        await itemExecutor(item);
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
}