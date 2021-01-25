using System.Collections.Generic;
using Albelli.Lambda.Templates.Core;
using Amazon.Lambda.SNSEvents;

namespace Albelli.Lambda.Templates.Sns
{
    public class SnsProxyFunction<TEntity, TStartup> : LambdaProxyFunction<TEntity, SNSEvent, SNSEvent.SNSRecord, TStartup> where TStartup : class
    {
        protected override IEnumerable<SNSEvent.SNSRecord> GetItems(SNSEvent collection) => collection.Records;
        protected override string GetJson(SNSEvent.SNSRecord item) => item.Sns.Message;
    }
}