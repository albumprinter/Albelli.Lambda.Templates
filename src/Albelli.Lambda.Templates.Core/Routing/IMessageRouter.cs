namespace Albelli.Lambda.Templates.Core.Routing
{
    public interface IMessageRouter
    {
        IMessageRouter AddMapping<TEntity>(string path);
        string GetPath<TEntity>();
    }
}