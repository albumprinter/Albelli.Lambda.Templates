using System.Collections.Generic;
using System.Threading.Tasks;
using Albelli.Templates.Amazon.Core.Executors;
using Amazon.Lambda.Core;
using JetBrains.Annotations;

namespace Albelli.Templates.Amazon.Core.Handlers
{
    public abstract class CollectionInputFunction<TEntity, TCollection, TItem, TStartup> : SingleItemFunction<TEntity, TItem, TStartup>
        where TStartup : class
    {
        protected abstract IEnumerable<TItem> GetItems(TCollection collection);

        [UsedImplicitly]
        public async Task FunctionHandlerAsync(TCollection collection, ILambdaContext lambdaContext)
        {
            await SnsEventExecutor.Execute(GetItems(collection), item => FunctionHandlerAsync(item, lambdaContext)).ConfigureAwait(false);
        }

        public ICollectionExecutor SnsEventExecutor { get; protected set; } = new SequentialCollectionExecutor();

        public void ChooseSequentialExecutionMode()
        {
            SnsEventExecutor = new SequentialCollectionExecutor();
        }

        public void ChooseConcurrentExecutionMode(int batchSize)
        {
            SnsEventExecutor = new ConcurrentCollectionExecutor(batchSize);
        }
    }
}