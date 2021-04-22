using System;

namespace Albelli.Templates.Amazon.Core.Routing
{
    public interface IMessageRouter
    {
        IMessageRouter AddMapping<TEntity>(string path);
        string GetPath(Type type);
    }
}