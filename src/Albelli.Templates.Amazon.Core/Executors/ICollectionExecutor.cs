using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Albelli.Templates.Amazon.Core.Executors
{
    public interface ICollectionExecutor
    {
        Task Execute<T>(IEnumerable<T> collection, Func<T, Task> itemExecutor);
    }
}