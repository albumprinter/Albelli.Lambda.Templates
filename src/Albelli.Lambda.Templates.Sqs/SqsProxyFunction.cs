using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Albelli.Lambda.Templates.Core;
using Albelli.Lambda.Templates.Core.Executors;
using Albelli.Lambda.Templates.Core.Pipelines;
using Albelli.Lambda.Templates.Core.Routing;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Albelli.Lambda.Templates.Sqs
{
    public abstract class SqsProxyFunction<TEntity, TStartup> : PipelinedAspNetCoreFunction<SQSEvent.SQSMessage, StatusProxyResponse>
        where TStartup : class
    {
        private Action<IServiceCollection> _messageRouterConfigurator = s => s.AddSingleton<IMessageRouter, BodyTypeMessageRouter>();
        private IServiceProvider _serviceProvider;

        protected SqsProxyFunction()
        {

        }

        protected SqsProxyFunction(StartupMode startupMode)
            : base(startupMode)
        {

        }

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

        protected override void MarshallRequest(InvokeFeatures features, SQSEvent.SQSMessage message, ILambdaContext lambdaContext)
        {
            var requestFeatures = (IHttpRequestFeature)features;
            requestFeatures.Scheme = "https";
            requestFeatures.Method = "POST";

            var messageRouter = _serviceProvider.GetService<IMessageRouter>();
            var path = $"/{messageRouter.GetPath<TEntity>().Trim('/')}/";

            requestFeatures.Path = path;
            requestFeatures.PathBase = string.Empty;

            requestFeatures.Headers["Content-Type"] = "application/json; charset=utf-8";

            requestFeatures.Body = new MemoryStream(Encoding.UTF8.GetBytes(message.Body));

            const string contentLengthHeaderName = "Content-Length";
            if (!requestFeatures.Headers.ContainsKey(contentLengthHeaderName))
            {
                requestFeatures.Headers[contentLengthHeaderName] = requestFeatures.Body.Length.ToString(CultureInfo.InvariantCulture);
            }

            PostMarshallRequestFeature(requestFeatures, message, lambdaContext);
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
        public async Task FunctionHandlerAsync(SQSEvent @event, ILambdaContext lambdaContext)
        {
            await SqsEventExecutor.Execute(@event.Records, record => ExecuteRecord(record, lambdaContext));
        }

        private async Task ExecuteRecord(SQSEvent.SQSMessage message, ILambdaContext lambdaContext)
        {
            SqsRecordPipelineHandlers.Foreach(handler => handler.HookBefore(message, lambdaContext));
            var response = await base.FunctionHandlerAsync(message, lambdaContext);
            SqsRecordPipelineHandlers.ForeachReverse(handler => handler.HookAfter(message, lambdaContext));

            lambdaContext.Logger.Log($"SQS message {message.MessageId} handled with status: {response.StatusCode}");
        }

        public List<ISqsMessagePipelineHandler> SqsRecordPipelineHandlers { get; } = new List<ISqsMessagePipelineHandler>();
        public ICollectionExecutor SqsEventExecutor { get; protected set; } = new SequentialCollectionExecutor();

        public void ChooseSequentialExecutionMode()
        {
            SqsEventExecutor = new SequentialCollectionExecutor();
        }

        public void ChooseConcurrentExecutionMode(int batchSize)
        {
            SqsEventExecutor = new ConcurrentCollectionExecutor(batchSize);
        }
    }
}