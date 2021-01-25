using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using JetBrains.Annotations;

namespace Albelli.Lambda.Templates.Core.Handlers
{
    public abstract class CollectionInputFunction<TEntity, TCollection, TItem, TStartup> : SingleItemFunction<TEntity, TItem, TStartup>
        where TStartup : class
    {
        protected abstract IEnumerable<TItem> GetItems(TCollection collection);

        [UsedImplicitly]
        public async Task FunctionHandlerAsync(TCollection collection, ILambdaContext lambdaContext)
        {
            await SnsEventExecutor.Execute(GetItems(collection), item => FunctionHandlerAsync(item, lambdaContext));
        }
    }
}