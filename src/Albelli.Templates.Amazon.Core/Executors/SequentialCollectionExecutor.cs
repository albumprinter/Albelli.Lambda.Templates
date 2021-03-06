﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Albelli.Templates.Amazon.Core.Executors
{
    public class SequentialCollectionExecutor : ICollectionExecutor
    {
        public async Task Execute<T>(IEnumerable<T> collection, Func<T, Task> itemExecutor)
        {
            foreach (var item in collection)
            {
                await itemExecutor(item).ConfigureAwait(false);
            }
        }
    }
}