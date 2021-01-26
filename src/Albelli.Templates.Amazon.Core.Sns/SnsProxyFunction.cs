using System.Collections.Generic;
using Albelli.Templates.Amazon.Core.Handlers;
using Amazon.Lambda.SNSEvents;
using JetBrains.Annotations;

namespace Albelli.Templates.Amazon.Core.Sns
{
    [PublicAPI]
    public class SnsProxyFunction<TEntity, TStartup> : CollectionInputFunction<TEntity, SNSEvent, SNSEvent.SNSRecord, TStartup> where TStartup : class
    {
        protected override IEnumerable<SNSEvent.SNSRecord> GetItems(SNSEvent collection) => collection.Records;
        protected override string GetEntityJson(SNSEvent.SNSRecord item) => item.Sns.Message;
    }
}