using System.Collections.Generic;
using Albelli.Templates.Amazon.Core.Handlers;
using Amazon.Lambda.SQSEvents;
using JetBrains.Annotations;

namespace Albelli.Templates.Amazon.Core.Sqs
{
    [PublicAPI]
    public class SqsProxyFunction<TEntity, TStartup> : CollectionInputFunction<TEntity, SQSEvent, SQSEvent.SQSMessage, TStartup> where TStartup : class
    {
        protected override IEnumerable<SQSEvent.SQSMessage> GetItems(SQSEvent collection) => collection.Records;
        protected override string GetEntityJson(SQSEvent.SQSMessage item) => item.Body;
    }
}