using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Albelli.Lambda.Templates.Core.Executors
{
    public interface ICollectionExecutor
    {
        Task Execute<T>(IEnumerable<T> collection, Func<T, Task> itemExecutor);
    }
}