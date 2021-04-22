using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Albelli.Templates.Amazon.Core.Pipelines;
using Albelli.Templates.Amazon.Core.Routing;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.Lambda.Core;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Albelli.Templates.Amazon.Core.Handlers
{
    public abstract class SingleItemFunction<TEntity, TItem, TStartup> : PipelinedAspNetCoreFunction<TItem, StatusProxyResponse>
        where TStartup : class
    {
        private Action<IServiceCollection> _messageRouterConfigurator = s => s.AddSingleton<IMessageRouter, BodyTypeMessageRouter>();
        private IServiceProvider _serviceProvider;

        protected SingleItemFunction()
        {

        }

        protected SingleItemFunction(StartupMode startupMode)
            : base(startupMode)
        {

        }

        protected abstract string GetEntityJson(TItem item);

        /// <summary>
        /// If you have inheritance with TEntity type and want to handle descendants, you could describe type choosing logic in this method
        /// </summary>
        protected virtual Type GetEntityType(TItem item) => typeof(TEntity);

        protected void ConfigureMessageRouter<TMessageRouter>()
        where TMessageRouter : class, IMessageRouter
        {
            _messageRouterConfigurator = s => s.AddSingleton<IMessageRouter, TMessageRouter>();
        }

        protected override void Init(IWebHostBuilder builder) => builder
            .ConfigureServices(_messageRouterConfigurator)
            .UseStartup<TStartup>();

        protected override void PostCreateHost(IHost webHost)
        {
            base.PostCreateHost(webHost);

            _serviceProvider = webHost.Services;
        }

        protected override void MarshallRequest(InvokeFeatures features, TItem item, ILambdaContext lambdaContext)
        {
            var requestFeatures = (IHttpRequestFeature)features;
            requestFeatures.Scheme = "https";
            requestFeatures.Method = "POST";

            var messageRouter = _serviceProvider.GetService<IMessageRouter>();
            var path = $"/{messageRouter.GetPath(GetEntityType(item)).Trim('/')}/";

            requestFeatures.Path = path;
            requestFeatures.PathBase = string.Empty;

            requestFeatures.Headers["Content-Type"] = "application/json; charset=utf-8";

            requestFeatures.Body = new MemoryStream(Encoding.UTF8.GetBytes(GetEntityJson(item)));

            const string contentLengthHeaderName = "Content-Length";
            if (!requestFeatures.Headers.ContainsKey(contentLengthHeaderName))
            {
                requestFeatures.Headers[contentLengthHeaderName] = requestFeatures.Body.Length.ToString(CultureInfo.InvariantCulture);
            }

            PostMarshallRequestFeature(requestFeatures, item, lambdaContext);
        }

        protected override StatusProxyResponse MarshallResponse(IHttpResponseFeature responseFeatures, ILambdaContext lambdaContext, int statusCodeIfNotSet = 200)
        {
            var response = new StatusProxyResponse
            {
                StatusCode = (HttpStatusCode)(responseFeatures.StatusCode != 0 ? responseFeatures.StatusCode : statusCodeIfNotSet)
            };

            PostMarshallResponseFeature(responseFeatures, response, lambdaContext);
            return response;
        }

        [UsedImplicitly]
        public new virtual async Task FunctionHandlerAsync(TItem item, ILambdaContext lambdaContext)
        {
            SingleItemPipelineHandlers.Foreach(handler => handler.HookBefore(item, lambdaContext));
            var response = await base.FunctionHandlerAsync(item, lambdaContext).ConfigureAwait(false);
            SingleItemPipelineHandlers.ForeachReverse(handler => handler.HookAfter(item, lambdaContext));

            lambdaContext.Logger.Log($"{typeof(TItem)} handled with status: {response.StatusCode}");
        }

        public List<IPipelineHandler<TItem>> SingleItemPipelineHandlers { get; } = new List<IPipelineHandler<TItem>>();
    }
}